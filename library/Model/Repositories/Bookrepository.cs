using System;
using System.Collections.Generic;
using System.Text;
using System.Text;
using Dapper;
using library.Model;
using library.Model.Entities;
using library.Model.Repositories;

namespace library.Model.Repositories;

/// <summary>
/// 蔵書データアクセス実装（Dapper）
/// </summary>
public sealed class BookRepository : IBookRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BookRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // -------------------------------------------------------------------------
    // SearchAsync
    // -------------------------------------------------------------------------
    public async Task<IList<Book>> SearchAsync(BookSearchCriteria criteria)
    {
        var sql = new StringBuilder(@"
            SELECT ID, BookName, Author, Publisher, Genre, ISBN,
                   Status, IsLoaned, IsReserved
            FROM   books
            WHERE  1 = 1");

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(criteria.BookName))
        {
            sql.Append(" AND BookName LIKE @BookName");
            parameters.Add("BookName", $"%{criteria.BookName}%");
        }

        if (!string.IsNullOrWhiteSpace(criteria.Author))
        {
            sql.Append(" AND Author LIKE @Author");
            parameters.Add("Author", $"%{criteria.Author}%");
        }

        if (!string.IsNullOrWhiteSpace(criteria.Publisher))
        {
            sql.Append(" AND Publisher LIKE @Publisher");
            parameters.Add("Publisher", $"%{criteria.Publisher}%");
        }

        if (!string.IsNullOrWhiteSpace(criteria.Genre))
        {
            sql.Append(" AND Genre LIKE @Genre");
            parameters.Add("Genre", $"%{criteria.Genre}%");
        }

        if (criteria.BookId.HasValue)
        {
            sql.Append(" AND ID = @BookId");
            parameters.Add("BookId", criteria.BookId.Value);
        }

        if (criteria.Statuses is { Count: > 0 })
        {
            sql.Append(" AND Status IN @Statuses");
            parameters.Add("Statuses", criteria.Statuses);
        }

        // ページング（OFFSET / FETCH）
        int offset = (criteria.Page - 1) * criteria.PageSize;
        sql.Append(@"
            ORDER BY ID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", criteria.PageSize);

        using var conn = _connectionFactory.Create();
        var result = await conn.QueryAsync<Book>(sql.ToString(), parameters);
        return result.AsList();
    }

    // -------------------------------------------------------------------------
    // GetByIdAsync
    // -------------------------------------------------------------------------
    public async Task<Book?> GetByIdAsync(int bookId)
    {
        const string sql = @"
            SELECT ID, BookName, Author, Publisher, Genre, ISBN,
                   Status, IsLoaned, IsReserved
            FROM   books
            WHERE  ID = @BookId";

        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<Book>(sql, new { BookId = bookId });
    }

    // -------------------------------------------------------------------------
    // GetByIsbnAsync
    // -------------------------------------------------------------------------
    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        const string sql = @"
            SELECT ID, BookName, Author, Publisher, Genre, ISBN,
                   Status, IsLoaned, IsReserved
            FROM   books
            WHERE  ISBN = @ISBN";

        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<Book>(sql, new { ISBN = isbn });
    }

    // -------------------------------------------------------------------------
    // InsertAsync
    // -------------------------------------------------------------------------
    public async Task<int> InsertAsync(Book book)
    {
        const string sql = @"
            INSERT INTO books (BookName, Author, Publisher, Genre, ISBN, Status, IsLoaned, IsReserved)
            OUTPUT INSERTED.ID
            VALUES (@BookName, @Author, @Publisher, @Genre, @ISBN, @Status, @IsLoaned, @IsReserved)";

        using var conn = _connectionFactory.Create();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            book.BookName,
            book.Author,
            book.Publisher,
            book.Genre,
            book.ISBN,
            book.Status,
            book.IsLoaned,
            book.IsReserved
        });
    }

    // -------------------------------------------------------------------------
    // UpdateStatusAsync
    // -------------------------------------------------------------------------
    public async Task UpdateStatusAsync(int bookId, byte status, bool isLoaned, bool isReserved)
    {
        const string sql = @"
            UPDATE books
            SET    Status     = @Status,
                   IsLoaned   = @IsLoaned,
                   IsReserved = @IsReserved
            WHERE  ID = @BookId";

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new
        {
            BookId = bookId,
            Status = status,
            IsLoaned = isLoaned,
            IsReserved = isReserved
        });
    }
}