using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using LibrarySystem.Infrastructure;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;

namespace LibrarySystem.Infrastructure.Repositories;

/// <summary>
/// 利用者データアクセス実装（Dapper）
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // -------------------------------------------------------------------------
    // GetByIdAsync
    // -------------------------------------------------------------------------
    public async Task<User?> GetByIdAsync(int userId)
    {
        const string sql = @"
            SELECT ID, Name, Gender, Birth, Mail, Phone, Address, IsActive, CreatedAt
            FROM   users
            WHERE  ID = @UserId";

        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<User>(sql, new { UserId = userId });
    }

    // -------------------------------------------------------------------------
    // SearchByNameAsync
    // -------------------------------------------------------------------------
    public async Task<IList<User>> SearchByNameAsync(string name)
    {
        const string sql = @"
            SELECT ID, Name, Gender, Birth, Mail, Phone, Address, IsActive, CreatedAt
            FROM   users
            WHERE  Name LIKE @Name
            ORDER BY Name";

        using var conn = _connectionFactory.Create();
        var result = await conn.QueryAsync<User>(sql, new { Name = $"%{name}%" });
        return result.AsList();
    }

    // -------------------------------------------------------------------------
    // GetByMailAsync
    // -------------------------------------------------------------------------
    public async Task<User?> GetByMailAsync(string mail)
    {
        const string sql = @"
            SELECT ID, Name, Gender, Birth, Mail, Phone, Address, IsActive, CreatedAt
            FROM   users
            WHERE  Mail = @Mail";

        using var conn = _connectionFactory.Create();
        return await conn.QuerySingleOrDefaultAsync<User>(sql, new { Mail = mail });
    }

    // -------------------------------------------------------------------------
    // InsertAsync
    // -------------------------------------------------------------------------
    public async Task<int> InsertAsync(User user)
    {
        const string sql = @"
            INSERT INTO users (Name, Gender, Birth, Mail, Phone, Address, IsActive)
            OUTPUT INSERTED.ID
            VALUES (@Name, @Gender, @Birth, @Mail, @Phone, @Address, @IsActive)";

        using var conn = _connectionFactory.Create();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            user.Name,
            user.Gender,
            user.Birth,
            user.Mail,
            user.Phone,
            user.Address,
            user.IsActive
        });
    }

    // -------------------------------------------------------------------------
    // UpdateAsync
    // -------------------------------------------------------------------------
    public async Task UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE users
            SET    Name     = @Name,
                   Gender   = @Gender,
                   Birth    = @Birth,
                   Mail     = @Mail,
                   Phone    = @Phone,
                   Address  = @Address,
                   IsActive = @IsActive
            WHERE  ID = @Id";

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new
        {
            user.Id,
            user.Name,
            user.Gender,
            user.Birth,
            user.Mail,
            user.Phone,
            user.Address,
            user.IsActive
        });
    }

    // -------------------------------------------------------------------------
    // DeactivateAsync
    // -------------------------------------------------------------------------
    public async Task DeactivateAsync(int userId)
    {
        const string sql = @"
            UPDATE users
            SET    IsActive = 0
            WHERE  ID = @UserId";

        using var conn = _connectionFactory.Create();
        await conn.ExecuteAsync(sql, new { UserId = userId });
    }
}