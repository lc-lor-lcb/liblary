using System;
using System.Collections.Generic;
using System.Text;
using library.Model.Entities;

namespace library.Model.Repositories;

/// <summary>
/// 予約データアクセスインターフェース
/// </summary>
public interface IReservationRepository
{
    /// <summary>
    /// 予約を新規挿入する
    /// </summary>
    Task<int> InsertAsync(Reservation reservation);

    /// <summary>
    /// 指定蔵書の有効な予約（Status=0）を取得する
    /// </summary>
    Task<Reservation?> GetActiveByBookIdAsync(int bookId);

    /// <summary>
    /// 指定利用者・蔵書の有効な予約が存在するか確認する（重複予約チェック）
    /// </summary>
    Task<bool> ExistsActiveAsync(int userId, int bookId);

    /// <summary>
    /// 予約ステータスを更新する
    /// </summary>
    Task UpdateStatusAsync(int reservationId, byte status);

    /// <summary>
    /// 返却通知済フラグを立てる
    /// </summary>
    Task SetNotifiedAsync(int reservationId);

    /// <summary>
    /// 予約IDで予約を取得する
    /// </summary>
    Task<Reservation?> GetByIdAsync(int reservationId);

    /// <summary>
    /// 指定利用者の予約一覧を取得する
    /// </summary>
    Task<IList<Reservation>> GetByUserIdAsync(int userId);
}