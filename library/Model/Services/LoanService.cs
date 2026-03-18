using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;

namespace LibrarySystem.Model.Services;

public class LoanService : ILoanService
{
    private const int MaxLoansPerUser = 5;
    private const int LoanDays = 14;

    private readonly IBookRepository _bookRepo;
    private readonly ILogRepository _logRepo;
    private readonly IReservationRepository _reservationRepo;
    private readonly IUserRepository _userRepo;

    public LoanService(IBookRepository bookRepo, ILogRepository logRepo,
        IReservationRepository reservationRepo, IUserRepository userRepo)
    {
        _bookRepo = bookRepo;
        _logRepo = logRepo;
        _reservationRepo = reservationRepo;
        _userRepo = userRepo;
    }

    public async Task<Result<LoanResult>> CheckoutAsync(int bookId, int userId)
    {
        // 利用者確認
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
            return Result<LoanResult>.Fail("利用者IDが見つかりません。");

        // 蔵書確認
        var book = await _bookRepo.GetByIdAsync(bookId);
        if (book == null)
            return Result<LoanResult>.Fail("蔵書IDが見つかりません。");

        // 貸出可否チェック
        switch (book.Status)
        {
            case BookStatus.Loaned:
                return Result<LoanResult>.Fail("この蔵書は現在貸出中です。予約を行ってください。");

            case BookStatus.Reserved:
                var reservation = await _reservationRepo.GetActiveByBookAsync(bookId);
                if (reservation == null || reservation.User_id != userId)
                    return Result<LoanResult>.Fail("この蔵書は別の方が予約しています。");
                // 自分の予約なら貸出可能 → 予約を「貸出済」に更新
                await _reservationRepo.FulfillByBookAndUserAsync(bookId, userId);
                break;

            case BookStatus.Retired:
                return Result<LoanResult>.Fail("この蔵書は除籍済みです。");

            case BookStatus.InStock:
                // 貸出可能
                break;
        }

        // 貸出上限チェック
        int activeCount = await _logRepo.CountActiveLoansByUserAsync(userId);
        if (activeCount >= MaxLoansPerUser)
            return Result<LoanResult>.Fail($"貸出上限（{MaxLoansPerUser}冊）に達しています。返却後に再度お試しください。");

        // 延滞チェック（警告のみ、処理は続行）
        bool hasOverdue = await _logRepo.HasOverdueLoanAsync(userId);

        // 貸出処理
        var now = DateTime.Now;
        var returnDue = DateOnly.FromDateTime(now.AddDays(LoanDays));
        var log = new Log
        {
            User_id = userId,
            Book_id = bookId,
            LoanDate = now,
            ReturnDue = returnDue
        };

        long logId = await _logRepo.InsertAsync(log);
        await _bookRepo.UpdateStatusAsync(bookId, BookStatus.Loaned, isLoaned: true, isReserved: book.IsReserved);

        return Result<LoanResult>.Ok(new LoanResult
        {
            LogId = (int)logId,
            ReturnDue = returnDue,
            HasOverdueWarning = hasOverdue
        });
    }

    public async Task<Result<ReturnResult>> ReturnAsync(int bookId, int userId)
    {
        // 対象の貸出レコード取得
        var activeLog = await _logRepo.GetActiveLoanAsync(bookId, userId);
        if (activeLog == null)
            return Result<ReturnResult>.Fail("該当する貸出記録が見つかりません。利用者IDと蔵書IDを確認してください。");

        // 予約確認
        var reservation = await _reservationRepo.GetActiveByBookAsync(bookId);
        bool hasPendingReservation = reservation != null;

        // 返却処理
        await _logRepo.SetReturnDateAsync(activeLog.ID, DateTime.Now);

        // 蔵書ステータス更新
        if (hasPendingReservation)
        {
            await _bookRepo.UpdateStatusAsync(bookId, BookStatus.Reserved, isLoaned: false, isReserved: true);
            await _reservationRepo.SetNotifiedAsync(reservation!.ID);
        }
        else
        {
            await _bookRepo.UpdateStatusAsync(bookId, BookStatus.InStock, isLoaned: false, isReserved: false);
        }

        return Result<ReturnResult>.Ok(new ReturnResult
        {
            HasPendingReservation = hasPendingReservation,
            ReservationUserId = reservation?.User_id
        });
    }

    public Task<IList<Log>> GetActiveLoansAsync(int userId)
        => _logRepo.GetActiveLoansByUserAsync(userId);
}
