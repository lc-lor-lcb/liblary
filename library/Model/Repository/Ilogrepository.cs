using LibrarySystem.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Entities;

namespace LibrarySystem.Model.Repositories;

/// <summary>
/// 貸出ログデータアクセスインターフェース
/// </summary>
public interface ILogRepository
{
    /// <summary>
    /// 貸出ログを新規挿入する
    /// </summary>
    Task<long> InsertAsync(Log log);

    /// <summary>
    /// 指定利用者・蔵書の貸出中レコードを取得する（ReturnDate IS NULL）
    /// </summary>
    Task<Log?> GetActiveLoanAsync(int userId, int bookId);

    /// <summary>
    /// 指定利用者の貸出中件数を取得する
    /// </summary>
    Task<int> GetActiveLoanCountAsync(int userId);

    /// <summary>
    /// 指定利用者の貸出中ログ一覧を取得する
    /// </summary>
    Task<IList<Log>> GetActiveLoansAsync(int userId);

    /// <summary>
    /// 指定利用者に延滞中の蔵書があるか確認する
    /// （ReturnDue &lt; 本日 かつ ReturnDate IS NULL）
    /// </summary>
    Task<bool> HasOverdueAsync(int userId);

    /// <summary>
    /// 返却処理：ReturnDate を現在日時に更新する
    /// </summary>
    Task ReturnAsync(long logId, DateTime returnDate);

    /// <summary>
    /// 蔵書IDで貸出中レコードの返却期限日（ReturnDue）を取得する
    /// （ReturnDate IS NULL の最新レコード）
    /// </summary>
    Task<DateOnly?> GetReturnDueByBookIdAsync(int bookId);
}