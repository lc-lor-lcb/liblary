using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Dto;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;

namespace LibrarySystem.Model.Services;

/// <summary>貸出・返却処理サービスインターフェース</summary>
public interface ILoanService
{
    /// <summary>
    /// 蔵書を貸し出す。
    /// 利用者操作のため認証不要。利用者ID・蔵書IDのみで処理する。
    /// </summary>
    Task<LoanResult> CheckoutAsync(int bookId, int userId);

    /// <summary>蔵書を返却する。</summary>
    Task<ReturnResult> ReturnAsync(int bookId, int userId);

    /// <summary>指定利用者の現在貸出中ログ一覧を取得する。</summary>
    Task<IList<Log>> GetActiveLoansAsync(int userId);
}

/// <summary>貸出・返却サービス実装</summary>
public class LoanService : ILoanService
{
    // 設計書5.4: 貸出期間14日、上限5件
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

    /// <inheritdoc/>
    public async Task<LoanResult> CheckoutAsync(int bookId, int userId)
    {
        // ---- 1. 利用者の存在確認・IsActiveチェック ----
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null || !user.IsActive)
            return Fail("利用者IDが存在しないか、無効な利用者です。");

        // ---- 2. 蔵書の存在確認・ステータスチェック ----
        var book = await _bookRepository.FindByIdAsync(bookId);
        if (book == null)
            return Fail("蔵書IDが存在しません。");

        var (canLoan, cannotReason) = await CheckLoanable(book, userId);
        if (!canLoan)
            return Fail(cannotReason!);

        // ---- 3. 貸出上限チェック ----
        var activeCount = await _logRepository.CountActiveLoansAsync(userId);
        if (activeCount >= MaxLoanCount)
            return Fail($"貸出上限（{MaxLoanCount}冊）に達しているため、貸し出しできません。");

        // ---- 4. 延滞チェック（警告のみ・処理続行可能） ----
        var hasOverdue = await _logRepository.HasOverdueAsync(userId);

        // ---- 5. 貸出処理 ----
        var loanDate = DateTime.Now;
        var returnDue = DateOnly.FromDateTime(loanDate.AddDays(LoanDays));

        var log = new Log
        {
            UserId = userId,
            BookId = bookId,
            LoanDate = loanDate,
            ReturnDue = returnDue,
            ReturnDate = null,
        };
        await _logRepository.InsertAsync(log);

        // booksテーブル更新（Status=1:貸出中）
        // 予約済みで予約者本人が借りる場合は予約をクリアする
        byte newStatus = 1;
        bool isReserved = book.IsReserved;

        if (book.Status == 2 && book.IsReserved)
        {
            // 予約者本人であることは CheckLoanable で確認済み
            var reservation = await _reservationRepository.FindActiveByBookAndUserAsync(bookId, userId);
            if (reservation != null)
                await _reservationRepository.UpdateStatusAsync(reservation.Id, 1); // 貸出済

            isReserved = false;
        }

        await _bookRepository.UpdateStatusAsync(bookId, newStatus, isLoaned: true, isReserved: isReserved);

        return new LoanResult
        {
            IsSuccess = true,
            ReturnDue = returnDue,
            HasOverdue = hasOverdue,
        };
    }

    /// <inheritdoc/>
    public async Task<ReturnResult> ReturnAsync(int bookId, int userId)
    {
        // ---- 1. 貸出中レコード確認 ----
        var log = await _logRepository.FindActiveLoanAsync(bookId, userId);
        if (log == null)
            return ReturnFail("該当する貸出記録が見つかりません。利用者IDと蔵書IDを確認してください。");

        // ---- 2. 予約確認 ----
        var hasReservation = await _reservationRepository.ExistsActiveByBookAsync(bookId);
        int? nextUserId = null;

        if (hasReservation)
        {
            var nextReservation = await _reservationRepository.FindOldestActiveByBookAsync(bookId);
            nextUserId = nextReservation?.UserId;
            // 返却通知フラグを立てる（通知メール送信は外部サービス等で別途実施）
            if (nextReservation != null)
                await _reservationRepository.SetNotifiedAsync(nextReservation.Id);
        }

        // ---- 3. ログ更新（返却日時を設定） ----
        await _logRepository.ReturnAsync(log.Id, DateTime.Now);

        // ---- 4. 蔵書ステータス更新 ----
        if (hasReservation)
        {
            // 予約あり: Status=2(予約済), IsLoaned=0, IsReserved=1
            await _bookRepository.UpdateStatusAsync(bookId, status: 2, isLoaned: false, isReserved: true);
        }
        else
        {
            // 予約なし: Status=0(在庫), IsLoaned=0, IsReserved=0
            await _bookRepository.UpdateStatusAsync(bookId, status: 0, isLoaned: false, isReserved: false);
        }

        return new ReturnResult
        {
            IsSuccess = true,
            HasNextReservation = hasReservation,
            NextReservationUserId = nextUserId,
        };
    }

    /// <inheritdoc/>
    public async Task<IList<Log>> GetActiveLoansAsync(int userId)
    {
        return await _logRepository.GetActiveLoansByUserAsync(userId);
    }

    // ----------------------------------------------------------------
    // ヘルパー
    // ----------------------------------------------------------------

    /// <summary>
    /// 貸出可否を判定する（設計書5.4.2）。
    /// </summary>
    private async Task<(bool canLoan, string? reason)> CheckLoanable(Book book, int userId)
    {
        return book.Status switch
        {
            0 => (true, null),   // 在庫: 貸出可
            1 => (false, "この蔵書は現在貸出中です。予約をご利用ください。"),
            2 => await CheckReservedBook(book, userId),
            3 => (false, "この蔵書は除籍済みのため貸し出しできません。"),
            _ => (false, "蔵書の状態が不明です。"),
        };
    }

    private async Task<(bool canLoan, string? reason)> CheckReservedBook(Book book, int userId)
    {
        var reservation = await _reservationRepository.FindActiveByBookAndUserAsync(book.Id, userId);
        if (reservation != null)
            return (true, null);   // 予約者本人: 貸出可

        return (false, "この蔵書は別の利用者に予約されています。");
    }

    private static LoanResult Fail(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };

    private static ReturnResult ReturnFail(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}