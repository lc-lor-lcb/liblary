using System;
using System.Collections.Generic;
using System.Text;
using library.Model.Entities;

namespace library.Model.Repositories;

/// <summary>
/// 蔵書データアクセスインターフェース
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// 検索条件に一致する蔵書一覧を取得する
    /// </summary>
    Task<IList<Book>> SearchAsync(BookSearchCriteria criteria);

    /// <summary>
    /// 蔵書IDで蔵書を取得する
    /// </summary>
    Task<Book?> GetByIdAsync(int bookId);

    /// <summary>
    /// ISBNで蔵書を取得する（重複チェック用）
    /// </summary>
    Task<Book?> GetByIsbnAsync(string isbn);

    /// <summary>
    /// 蔵書を新規登録する
    /// </summary>
    Task<int> InsertAsync(Book book);

    /// <summary>
    /// 蔵書のステータスを更新する
    /// </summary>
    Task UpdateStatusAsync(int bookId, byte status, bool isLoaned, bool isReserved);
}

/// <summary>
/// 蔵書検索条件
/// </summary>
public sealed class BookSearchCriteria
{
    public string? BookName { get; init; }
    public string? Author { get; init; }
    public string? Publisher { get; init; }
    public string? Genre { get; init; }
    public int? BookId { get; init; }
    public IList<byte>? Statuses { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 100;
}