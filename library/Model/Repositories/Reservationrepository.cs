using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using library.Model;
using library.Model.Entities;
using library.Model.Repositories;

namespace library.Model.Repositories;

/// <summary>
/// 予約データアクセス実装（Dapper）
/// </summary>
public sealed class ReservationRepository : IReservationRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReservationRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // -------------------------------------------------------------------------
    // InsertAsync
    // -------------------------------------------------------------------------
    public async Task<int> InsertAsync(Reservation reservation)
    {
        const string sql = @"
            INSERT INTO reservations (User_id, Book_id, ReservationDate, Status, Notified)
            OUTPUT INSERTED.ID
            VALUES (@UserId, @BookId, @ReservationDate, @Status, @Notified)";

        using var conn = _connectionFactory.Create();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            UserId = reservation.UserId,
            BookId = reservation.BookId,
            ReservationDate = reservation.ReservationDate,
            Status = reservation.Status,
            Notified = reservation.Notified
        });
    }

    // -------------------------------------------------------------------------
    // GetActiveByBookIdAsync
    // -------------------------------------------------------------------------
    public async Task<Reservation?> GetActiveByBookIdAsync(int bookId)
    {
        const string sql = @"
            SELECT ID, User_id AS UserId, Book_id AS BookId,
                   ReservationDate, Status, Notified
            FROM   reservations
            WHERE  Book_id = @BookId
              AND  Status  = 0
            ORDER BY ReservationDate ASC";

        // 予約は先着順で先頭の1件を返す
        using var conn = _connectionFactory.Create();
        var results = await conn.QueryAsync<Reservation>(sql, new { BookId = bookId });
        return results.FirstOrDefault();
    }

    // -------------------------------------------------------------------------
    // ExistsActiveAsync
    // -------------------------------------------------------------------------
    public async Task<bool> ExistsActiveAsync(int userId, int bookId)
    {
        const string sql = @"
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM   reservations
                WHERE  User_id = @UserId
                  AND  Book_id = @BookId
                  AND  Status  = 0
            ) THEN 1 ELSE 0 END";

        using var conn = _connectionFactory.Create();
        return await conn.ExecuteScalarAsync<bool>(sql, new { UserId = userId, BookId = bookId });
    }

    // -------------------------------------------------------------------------
    // UpdateStatusAsync
    // -------------------------------------------------------------------------
    public async Task UpdateStatusAsync(int reservationId, byte status)
    {
        const string sql = @"
            UPDATE reservations
            SET    Status = @Status
            WHERE  ID = @ReservationId";

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { ReservationId = reservationId, Status = status });
    }

    // -------------------------------------------------------------------------
    // SetNotifiedAsync
    // -------------------------------------------------------------------------
    public async Task SetNotifiedAsync(int reservationId)
    {
        const string sql = @"
            UPDATE reservations
            SET    Notified = 1
            WHERE  ID = @ReservationId";

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { ReservationId = reservationId });
    }

    // -------------------------------------------------------------------------
    // GetByIdAsync
    // -------------------------------------------------------------------------
    public async Task<Reservation?> GetByIdAsync(int reservationId)
    {
        const string sql = @"
            SELECT ID, User_id AS UserId, Book_id AS BookId,
                   ReservationDate, Status, Notified
            FROM   reservations
            WHERE  ID = @ReservationId";

        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<Reservation>(sql, new { ReservationId = reservationId });
    }

    // -------------------------------------------------------------------------
    // GetByUserIdAsync
    // -------------------------------------------------------------------------
    public async Task<IList<Reservation>> GetByUserIdAsync(int userId)
    {
        const string sql = @"
            SELECT ID, User_id AS UserId, Book_id AS BookId,
                   ReservationDate, Status, Notified
            FROM   reservations
            WHERE  User_id = @UserId
            ORDER BY ReservationDate DESC";

        using var conn = _connectionFactory.Create();
        var result = await conn.QueryAsync<Reservation>(sql, new { UserId = userId });
        return result.AsList();
    }
}