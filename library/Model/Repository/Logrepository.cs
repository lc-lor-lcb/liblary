using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using LibrarySystem.Infrastructure;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;

namespace LibrarySystem.Infrastructure.Repositories;

/// <summary>
/// 貸出ログデータアクセス実装（Dapper）
/// </summary>
public sealed class LogRepository : ILogRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public LogRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // -------------------------------------------------------------------------
    // InsertAsync
    // -------------------------------------------------------------------------
    public async Task<long> InsertAsync(Log log)
    {
        const string sql = @"
            INSERT INTO logs (User_id, Book_id, LoanDate, ReturnDue, ReturnDate)
            OUTPUT INSERTED.ID
            VALUES (@UserId, @BookId, @LoanDate, @ReturnDue, @ReturnDate)";

        using var conn = _connectionFactory.Create();
        return await conn.ExecuteScalarAsync<long>(sql, new
        {
            UserId = log.UserId,
            BookId = log.BookId,
            LoanDate = log.LoanDate,
            ReturnDue = log.ReturnDue,
            ReturnDate = log.ReturnDate
        });
    }

    // -------------------------------------------------------------------------
    // GetActiveLoanAsync
    // -------------------------------------------------------------------------
    public async Task<Log?> GetActiveLoanAsync(int userId, int bookId)
    {
        const string sql = @"
            SELECT ID, User_id AS UserId, Book_id AS BookId,
                   LoanDate, ReturnDue, ReturnDate
            FROM   logs
            WHERE  User_id    = @UserId
              AND  Book_id    = @BookId
              AND  ReturnDate IS NULL";

        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<Log>(sql, new { UserId = userId, BookId = bookId });
    }

    // -------------------------------------------------------------------------
    // GetActiveLoanCountAsync
    // -------------------------------------------------------------------------
    public async Task<int> GetActiveLoanCountAsync(int userId)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM   logs
            WHERE  User_id    = @UserId
              AND  ReturnDate IS NULL";

        using var conn = _connectionFactory.Create();
        return await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId });
    }

    // -------------------------------------------------------------------------
    // GetActiveLoansAsync
    // -------------------------------------------------------------------------
    public async Task<IList<Log>> GetActiveLoansAsync(int userId)
    {
        const string sql = @"
            SELECT ID, User_id AS UserId, Book_id AS BookId,
                   LoanDate, ReturnDue, ReturnDate
            FROM   logs
            WHERE  User_id    = @UserId
              AND  ReturnDate IS NULL
            ORDER BY LoanDate DESC";

        using var conn = _connectionFactory.Create();
        var result = await conn.QueryAsync<Log>(sql, new { UserId = userId });
        return result.AsList();
    }

    // -------------------------------------------------------------------------
    // HasOverdueAsync
    // -------------------------------------------------------------------------
    public async Task<bool> HasOverdueAsync(int userId)
    {
        const string sql = @"
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM   logs
                WHERE  User_id    = @UserId
                  AND  ReturnDate IS NULL
                  AND  ReturnDue  < CAST(GETDATE() AS DATE)
            ) THEN 1 ELSE 0 END";

        using var conn = _connectionFactory.Create();
        return await conn.ExecuteScalarAsync<bool>(sql, new { UserId = userId });
    }

    // -------------------------------------------------------------------------
    // ReturnAsync
    // -------------------------------------------------------------------------
    public async Task ReturnAsync(long logId, DateTime returnDate)
    {
        const string sql = @"
            UPDATE logs
            SET    ReturnDate = @ReturnDate
            WHERE  ID = @LogId";

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { LogId = logId, ReturnDate = returnDate });
    }

    // -------------------------------------------------------------------------
    // GetReturnDueByBookIdAsync  ★ v1.1追加メソッド
    // ReservationService.GetReturnDueByBookAsync から呼び出される。
    // 指定蔵書の貸出中レコード（ReturnDate IS NULL）の返却期限日を返す。
    // 複数レコードが存在する場合は最新の LoanDate のものを返す。
    // -------------------------------------------------------------------------
    public async Task<DateOnly?> GetReturnDueByBookIdAsync(int bookId)
    {
        const string sql = @"
            SELECT TOP 1 ReturnDue
            FROM   logs
            WHERE  Book_id    = @BookId
              AND  ReturnDate IS NULL
            ORDER BY LoanDate DESC";

        using var conn = _connectionFactory.Create();
        // SQL Server の DATE 型は Dapper で DateOnly にマップされる (.NET 6+)
        var result = await conn.ExecuteScalarAsync<DateOnly?>(sql, new { BookId = bookId });
        return result;
    }
}