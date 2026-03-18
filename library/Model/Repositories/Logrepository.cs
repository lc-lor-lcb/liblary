using Dapper;
using LibrarySystem.Infrastructure;
using LibrarySystem.Model.Entities;

namespace LibrarySystem.Model.Repositories;

public class LogRepository : ILogRepository
{
    private readonly IDbConnectionFactory _factory;
    public LogRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<Log?> GetActiveLoanAsync(int bookId, int userId)
    {
        const string sql = @"
            SELECT ID, User_id, Book_id, LoanDate, ReturnDue, ReturnDate
            FROM logs WHERE Book_id = @BookId AND User_id = @UserId AND ReturnDate IS NULL";
        using var conn = _factory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<LogRow>(sql, new { BookId = bookId, UserId = userId });
        return row?.ToLog();
    }

    public async Task<IList<Log>> GetActiveLoansByUserAsync(int userId)
    {
        const string sql = @"
            SELECT ID, User_id, Book_id, LoanDate, ReturnDue, ReturnDate
            FROM logs WHERE User_id = @UserId AND ReturnDate IS NULL";
        using var conn = _factory.CreateConnection();
        var rows = await conn.QueryAsync<LogRow>(sql, new { UserId = userId });
        return rows.Select(r => r.ToLog()).ToList();
    }

    public async Task<int> CountActiveLoansByUserAsync(int userId)
    {
        const string sql = "SELECT COUNT(*) FROM logs WHERE User_id = @UserId AND ReturnDate IS NULL";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId });
    }

    public async Task<bool> HasOverdueLoanAsync(int userId)
    {
        const string sql = @"
            SELECT COUNT(1) FROM logs
            WHERE User_id = @UserId AND ReturnDate IS NULL AND ReturnDue < CAST(GETDATE() AS DATE)";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId }) > 0;
    }

    public async Task<long> InsertAsync(Log log)
    {
        const string sql = @"
            INSERT INTO logs (User_id, Book_id, LoanDate, ReturnDue)
            VALUES (@User_id, @Book_id, @LoanDate, @ReturnDue);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<long>(sql, new
        {
            log.User_id, log.Book_id, log.LoanDate,
            ReturnDue = log.ReturnDue.ToDateTime(TimeOnly.MinValue)
        });
    }

    public async Task SetReturnDateAsync(long logId, DateTime returnDate)
    {
        const string sql = "UPDATE logs SET ReturnDate = @ReturnDate WHERE ID = @ID";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { ReturnDate = returnDate, ID = logId });
    }

    public async Task<DateOnly?> GetCurrentReturnDueAsync(int bookId)
    {
        const string sql = "SELECT TOP 1 ReturnDue FROM logs WHERE Book_id = @BookId AND ReturnDate IS NULL";
        using var conn = _factory.CreateConnection();
        var dt = await conn.ExecuteScalarAsync<DateTime?>(sql, new { BookId = bookId });
        return dt.HasValue ? DateOnly.FromDateTime(dt.Value) : null;
    }

    private class LogRow
    {
        public long ID { get; set; }
        public int User_id { get; set; }
        public int Book_id { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime ReturnDue { get; set; }
        public DateTime? ReturnDate { get; set; }

        public Log ToLog() => new()
        {
            ID = ID, User_id = User_id, Book_id = Book_id,
            LoanDate = LoanDate,
            ReturnDue = DateOnly.FromDateTime(ReturnDue),
            ReturnDate = ReturnDate
        };
    }
}
