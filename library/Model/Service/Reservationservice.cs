using System;
using System.Collections.Generic;
using System.Text;
using library.Model.Dto;
using library.Model.Entities;
using library.Model.Repositories;

namespace library.Model.Services;

/// <summary>予約管理サービスインターフェース</summary>
public interface IReservationService
{
    /// <summary>蔵書を予約する。</summary>
    Task<ReservationResult> ReserveAsync(int bookId, int userId);

    /// <summary>予約をキャンセルする。成功時は true。</summary>
    Task<bool> CancelReservationAsync(int reservationId);

    /// <summary>指定蔵書の最古の有効予約を取得する。存在しない場合は null。</summary>
    Task<Reservation?> GetByBookAsync(int bookId);
}

/// <summary>予約サービス実装</summary>
public class ReservationService : IReservationService
{
    private readonly IUserRepository _userRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ILogRepository _logRepository;
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(
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
    public async Task<ReservationResult> ReserveAsync(int bookId, int userId)
    {
        // ---- 1. 利用者確認 ----
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
            return Fail("利用者IDが存在しないか、無効な利用者です。");

        // ---- 2. 蔵書確認 ----
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
            return Fail("蔵書IDが存在しません。");

        if (book.Status == 3)
            return Fail("この蔵書は除籍済みのため予約できません。");

        if (book.Status == 0)
            return Fail("この蔵書は現在在庫あります。予約不要です。");

        // ---- 3. 重複予約チェック ----
        var existing = await _reservationRepository.FindActiveByBookAndUserAsync(bookId, userId);
        if (existing != null)
            return Fail("この蔵書は既に予約済みです。");

        // ---- 4. 現在の返却期限日取得（貸出中の場合） ----
        DateOnly? currentReturnDue = null;
        if (book.IsLoaned)
        {
            var activeLogs = await _logRepository.GetActiveLoansByUserAsync(0); // 全貸出中取得は別メソッド推奨
            // 蔵書の返却期限をlogsテーブルから取得（本実装ではリポジトリ拡張を想定）
            currentReturnDue = await GetReturnDueByBookAsync(bookId);
        }

        // ---- 5. 予約レコード挿入 ----
        var reservation = new Reservation
        {
            UserId = userId,
            BookId = bookId,
            ReservationDate = DateTime.Now,
            Status = 0,
            Notified = false,
        };
        await _reservationRepository.InsertAsync(reservation);

        // ---- 6. 蔵書ステータス更新（Status=2, IsReserved=1） ----
        await _bookRepository.UpdateStatusAsync(
            bookId,
            status: 2,
            isLoaned: book.IsLoaned,
            isReserved: true);

        return new ReservationResult
        {
            IsSuccess = true,
            CurrentReturnDue = currentReturnDue,
        };
    }

    /// <inheritdoc/>
    public async Task<bool> CancelReservationAsync(int reservationId)
    {
        // キャンセル（Status=2）に更新するのみ。蔵書ステータスの整合性は
        // 別途バッチまたはReturnService側で維持する。
        await _reservationRepository.UpdateStatusAsync(reservationId, status: 2);
        return true;
    }

    /// <inheritdoc/>
    public async Task<Reservation?> GetByBookAsync(int bookId)
    {
        return await _reservationRepository.FindOldestActiveByBookAsync(bookId);
    }

    // ----------------------------------------------------------------
    // ヘルパー
    // ----------------------------------------------------------------

    /// <summary>
    /// 指定蔵書の現在貸出中レコードから返却期限日を取得する。
    /// ILogRepository に GetReturnDueByBookId を追加するか、
    /// リポジトリ実装側で直接クエリする。
    /// ここではインターフェース拡張を想定した呼び出しを示す。
    /// </summary>
    private async Task<DateOnly?> GetReturnDueByBookAsync(int bookId)
    {
        // 実装例: SELECT TOP 1 ReturnDue FROM logs
        //         WHERE Book_id = @bookId AND ReturnDate IS NULL
        // リポジトリ拡張後にここを差し替える。
        await Task.CompletedTask;
        return null;
    }

    private static ReservationResult Fail(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}