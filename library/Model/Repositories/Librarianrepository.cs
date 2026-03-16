using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using library.Model;
using library.Model.Entities;
using library.Model.Repositories;

namespace library.Model.Repositories;

/// <summary>
/// 司書データアクセス実装（Dapper）
/// </summary>
public sealed class LibrarianRepository : ILibrarianRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public LibrarianRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // -------------------------------------------------------------------------
    // GetByUserNameAsync
    // -------------------------------------------------------------------------
    public async Task<Librarian?> GetByUserNameAsync(string userName)
    {
        const string sql = @"
            SELECT ID, UserName, Name, Mail, Password, IsActive, CreatedAt, UpdatedAt
            FROM   librarians
            WHERE  UserName = @UserName
              AND  IsActive = 1";

        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<Librarian>(sql, new { UserName = userName });
    }

    // -------------------------------------------------------------------------
    // GetByIdAsync
    // -------------------------------------------------------------------------
    public async Task<Librarian?> GetByIdAsync(int librarianId)
    {
        const string sql = @"
            SELECT ID, UserName, Name, Mail, Password, IsActive, CreatedAt, UpdatedAt
            FROM   librarians
            WHERE  ID = @LibrarianId";

        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<Librarian>(sql, new { LibrarianId = librarianId });
    }

    // -------------------------------------------------------------------------
    // InsertAsync
    // -------------------------------------------------------------------------
    public async Task<int> InsertAsync(Librarian librarian)
    {
        const string sql = @"
            INSERT INTO librarians (UserName, Name, Mail, Password, IsActive)
            OUTPUT INSERTED.ID
            VALUES (@UserName, @Name, @Mail, @Password, @IsActive)";

        using var conn = _connectionFactory.Create();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            librarian.UserName,
            librarian.Name,
            librarian.Mail,
            librarian.Password,
            librarian.IsActive
        });
    }

    // -------------------------------------------------------------------------
    // UpdatePasswordAsync
    // -------------------------------------------------------------------------
    public async Task UpdatePasswordAsync(int librarianId, string passwordHash)
    {
        const string sql = @"
            UPDATE librarians
            SET    Password  = @PasswordHash,
                   UpdatedAt = GETDATE()
            WHERE  ID = @LibrarianId";

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { LibrarianId = librarianId, PasswordHash = passwordHash });
    }

    // -------------------------------------------------------------------------
    // SetActiveAsync
    // -------------------------------------------------------------------------
    public async Task SetActiveAsync(int librarianId, bool isActive)
    {
        const string sql = @"
            UPDATE librarians
            SET    IsActive  = @IsActive,
                   UpdatedAt = GETDATE()
            WHERE  ID = @LibrarianId";

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { LibrarianId = librarianId, IsActive = isActive });
    }
}