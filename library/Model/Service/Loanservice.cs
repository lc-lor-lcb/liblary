using library.Model.Dto;
using library.Model.Entities;
using library.Model.Repositories;

namespace library.Model.Services;

public interface ILoanService
{
    Task<LoanResult> CheckoutAsync(int bookId, int userId);
    Task<ReturnResult> ReturnAsync(int bookId, int userId);
    Task<IList<Log>> GetActiveLoansAsync(int userId);
    Task<Log?> GetActiveLoanByBookAsync(int bookId);
}

public class LoanService : ILoanService
{
    private const int LoanDays = 14;
    private const int MaxLoanCount = 5;

    private readonly IUserRepository _userRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ILogRepository _logRepository;
    private readonly IReservationRepository _reservationRepository;

    public LoanService(
        IUserRepository userRepository,
        IBookRepository bookRepository,
        ILogRepository logRepository,
        IReservationRepository reservationRepository)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _logRepository = logRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<LoanResult> CheckoutAsync(int bookId, int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
            return Fail("利用者IDが存在しないか、無効な利用者です。");

        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
            return Fail("蔵書IDが存在しません。");

        var (canLoan, cannotReason) = await CheckLoanable(book, userId);
        if (!canLoan)
            return Fail(cannotReason!);

        var activeCount = await _logRepository.GetActiveLoanCountAsync(userId);
        if (activeCount >= MaxLoanCount)
            return Fail($"貸出上限（{MaxLoanCount}冊）に達しているため、貸し出しできません。");

        var hasOverdue = await _logRepository.HasOverdueAsync(userId);

        var loanDate = DateTime.Now;
        var returnDue = DateOnly.FromDateTime(loanDate.AddDays(LoanDays));

        await _logRepository.InsertAsync(new Log
        {
            UserId = userId,
            BookId = bookId,
            LoanDate = loanDate,
            ReturnDue = returnDue,
            ReturnDate = null,
        });

        bool isReserved = book.IsReserved;
        if (book.Status == BookStatus.Reserved && book.IsReserved)
        {
            // ★ ExistsActiveAsync は bool を返すので bool で受ける
            bool reservationExists = await _reservationRepository.ExistsActiveAsync(userId, bookId);
            if (reservationExists)
            {
                // 予約レコードをIDで取得してステータス更新
                var reservation = await _reservationRepository.GetByIdAsync(
                    await GetActiveReservationIdAsync(bookId, userId));
                if (reservation != null)
                    await _reservationRepository.UpdateStatusAsync(reservation.Id, 1);
            }
            isReserved = false;
        }

        await _bookRepository.UpdateStatusAsync(bookId, (byte)BookStatus.Loaned,
            isLoaned: true, isReserved: isReserved);

        return new LoanResult { IsSuccess = true, ReturnDue = returnDue, HasOverdue = hasOverdue };
    }

    public async Task<ReturnResult> ReturnAsync(int bookId, int userId)
    {
        var log = await _logRepository.GetActiveLoanAsync(bookId, userId);
        if (log == null)
            return ReturnFail("該当する貸出記録が見つかりません。利用者IDと蔵書IDを確認してください。");

        var nextReservation = await _reservationRepository.GetActiveByBookIdAsync(bookId);
        bool hasReservation = nextReservation != null;

        if (nextReservation != null)
            await _reservationRepository.SetNotifiedAsync(nextReservation.Id);

        await _logRepository.ReturnAsync(log.Id, DateTime.Now);

        if (hasReservation)
            await _bookRepository.UpdateStatusAsync(bookId, (byte)BookStatus.Reserved,
                isLoaned: false, isReserved: true);
        else
            await _bookRepository.UpdateStatusAsync(bookId, (byte)BookStatus.Available,
                isLoaned: false, isReserved: false);

        return new ReturnResult
        {
            IsSuccess = true,
            HasNextReservation = hasReservation,
            NextReservationUserId = nextReservation?.UserId,
        };
    }

    public async Task<IList<Log>> GetActiveLoansAsync(int userId)
        => await _logRepository.GetActiveLoansAsync(userId);

    public async Task<Log?> GetActiveLoanByBookAsync(int bookId)
        => await _logRepository.GetActiveLoanByBookAsync(bookId);

    // ----------------------------------------------------------------
    // ヘルパー
    // ----------------------------------------------------------------

    private async Task<(bool, string?)> CheckLoanable(Book book, int userId)
    {
        return book.Status switch
        {
            BookStatus.Available => (true, null),
            BookStatus.Loaned => (false, "この蔵書は現在貸出中です。予約をご利用ください。"),
            BookStatus.Reserved => await CheckReservedBook(book, userId),
            BookStatus.Discarded => (false, "この蔵書は除籍済みのため貸し出しできません。"),
            _ => (false, "蔵書の状態が不明です。"),
        };
    }

    private async Task<(bool, string?)> CheckReservedBook(Book book, int userId)
    {
        // ★ ExistsActiveAsync(userId, bookId) の引数順に注意
        bool exists = await _reservationRepository.ExistsActiveAsync(userId, book.Id);
        return exists
            ? (true, null)
            : (false, "この蔵書は別の利用者に予約されています。");
    }

    /// <summary>
    /// ExistsActiveAsync が bool のみ返すため、予約IDを特定する際に
    /// GetByUserIdAsync 経由で取得する。
    /// </summary>
    private async Task<int> GetActiveReservationIdAsync(int bookId, int userId)
    {
        var userReservations = await _reservationRepository.GetByUserIdAsync(userId);
        var target = userReservations.FirstOrDefault(r => r.BookId == bookId && r.Status == 0);
        return target?.Id ?? 0;
    }

    private static LoanResult Fail(string m) => new() { IsSuccess = false, ErrorMessage = m };
    private static ReturnResult ReturnFail(string m) => new() { IsSuccess = false, ErrorMessage = m };
}