using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Common;
using LibrarySystem.Model.Dto;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;

namespace LibrarySystem.Model.Services;

/// <summary>蔵書検索・取得・新規登録サービスインターフェース</summary>
public interface IBookService
{
    /// <summary>検索条件に一致する蔵書一覧を返す。</summary>
    Task<IList<Book>> SearchAsync(BookSearchCriteria criteria);

    /// <summary>蔵書IDで1件取得する。存在しない場合は null を返す。</summary>
    Task<Book?> GetByIdAsync(int bookId);

    /// <summary>蔵書を新規登録する。</summary>
    Task<Result<Book>> RegisterAsync(BookRegisterDto dto);
}

/// <summary>蔵書サービス実装</summary>
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    /// <inheritdoc/>
    public async Task<IList<Book>> SearchAsync(BookSearchCriteria criteria)
    {
        return await _bookRepository.SearchAsync(criteria);
    }

    /// <inheritdoc/>
    public async Task<Book?> GetByIdAsync(int bookId)
    {
        return await _bookRepository.FindByIdAsync(bookId);
    }

    /// <inheritdoc/>
    public async Task<Result<Book>> RegisterAsync(BookRegisterDto dto)
    {
        // --- バリデーション ---
        var validationError = ValidateBookRegisterDto(dto);
        if (validationError != null)
            return Result<Book>.Failure(validationError);

        // ISBN重複チェック
        if (await _bookRepository.ExistsISBNAsync(dto.ISBN))
            return Result<Book>.Failure($"ISBN「{dto.ISBN}」は既に登録されています。");

        var book = new Book
        {
            BookName = dto.BookName.Trim(),
            Author = dto.Author.Trim(),
            Publisher = dto.Publisher.Trim(),
            Genre = dto.Genre.Trim(),
            ISBN = dto.ISBN.Trim(),
            Status = 0,   // 在庫
            IsLoaned = false,
            IsReserved = false,
        };

        var inserted = await _bookRepository.InsertAsync(book);
        return Result<Book>.Success(inserted);
    }

    // ----------------------------------------------------------------
    // 入力バリデーション（設計書5.3.3準拠）
    // ----------------------------------------------------------------
    private static string? ValidateBookRegisterDto(BookRegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.BookName) || dto.BookName.Trim().Length > 500)
            return "図書名を1〜500文字で入力してください。";
        if (string.IsNullOrWhiteSpace(dto.Author) || dto.Author.Trim().Length > 200)
            return "著者名を1〜200文字で入力してください。";
        if (string.IsNullOrWhiteSpace(dto.Publisher) || dto.Publisher.Trim().Length > 200)
            return "出版社を1〜200文字で入力してください。";
        if (string.IsNullOrWhiteSpace(dto.Genre) || dto.Genre.Trim().Length > 100)
            return "ジャンルを1〜100文字で入力してください。";

        var isbn = dto.ISBN?.Trim() ?? string.Empty;
        if (!IsValidIsbn(isbn))
            return "ISBNは10桁または13桁の数字で入力してください。";

        return null;
    }

    /// <summary>
    /// ISBNが10桁または13桁の数字であるか検証する。
    /// 設計書5.3.3: 10桁または13桁の数字
    /// </summary>
    private static bool IsValidIsbn(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn)) return false;
        if (!isbn.All(char.IsDigit)) return false;
        return isbn.Length is 10 or 13;
    }
}