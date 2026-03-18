using Dapper;
using LibrarySystem.Infrastructure;
using LibrarySystem.Model.Entities;

namespace LibrarySystem.Model.Repositories;

// ---- ReservationRepository ----
public class ReservationRepository : IReservationRepository
{
    private readonly IDbConnectionFactory _factory;
    public ReservationRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<Reservation?> GetActiveByBookAsync(int bookId)
    {
        const string sql = @"
            SELECT ID, User_id, Book_id, ReservationDate, Status, Notified
            FROM reservations WHERE Book_id = @BookId AND Status = 0";
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Reservation>(sql, new { BookId = bookId });
    }

    public async Task<bool> ExistsActiveByUserAndBookAsync(int userId, int bookId)
    {
        const string sql = "SELECT COUNT(1) FROM reservations WHERE User_id=@UserId AND Book_id=@BookId AND Status=0";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId, BookId = bookId }) > 0;
    }

    public async Task<int> InsertAsync(Reservation reservation)
    {
        const string sql = @"
            INSERT INTO reservations (User_id, Book_id, ReservationDate, Status, Notified)
            VALUES (@User_id, @Book_id, @ReservationDate, @Status, 0);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            reservation.User_id, reservation.Book_id,
            reservation.ReservationDate,
            Status = (byte)reservation.Status
        });
    }

    public async Task UpdateStatusAsync(int reservationId, ReservationStatus status)
    {
        const string sql = "UPDATE reservations SET Status=@Status WHERE ID=@ID";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Status = (byte)status, ID = reservationId });
    }

    public async Task SetNotifiedAsync(int reservationId)
    {
        const string sql = "UPDATE reservations SET Notified=1 WHERE ID=@ID";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { ID = reservationId });
    }

    public async Task FulfillByBookAndUserAsync(int bookId, int userId)
    {
        const string sql = @"
            UPDATE reservations SET Status=1
            WHERE Book_id=@BookId AND User_id=@UserId AND Status=0";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { BookId = bookId, UserId = userId });
    }
}

// ---- UserRepository ----
public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _factory;
    public UserRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<User?> GetByIdAsync(int userId)
    {
        const string sql = "SELECT ID, Name, Gender, Birth, Mail, Phone, Address, IsActive, CreatedAt FROM users WHERE ID=@ID";
        using var conn = _factory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<UserRow>(sql, new { ID = userId });
        return row?.ToUser();
    }

    public async Task<IList<User>> SearchByNameAsync(string name)
    {
        const string sql = "SELECT ID, Name, Gender, Birth, Mail, Phone, Address, IsActive, CreatedAt FROM users WHERE Name LIKE @Name AND IsActive=1";
        using var conn = _factory.CreateConnection();
        var rows = await conn.QueryAsync<UserRow>(sql, new { Name = $"%{name}%" });
        return rows.Select(r => r.ToUser()).ToList();
    }

    public async Task<bool> ExistsByMailAsync(string mail, int? excludeUserId = null)
    {
        string sql = "SELECT COUNT(1) FROM users WHERE Mail=@Mail";
        if (excludeUserId.HasValue) sql += " AND ID <> @ExcludeId";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new { Mail = mail, ExcludeId = excludeUserId }) > 0;
    }

    public async Task<int> InsertAsync(User user)
    {
        const string sql = @"
            INSERT INTO users (Name, Gender, Birth, Mail, Phone, Address, IsActive)
            VALUES (@Name, @Gender, @Birth, @Mail, @Phone, @Address, 1);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            user.Name, user.Gender,
            Birth = user.Birth.ToDateTime(TimeOnly.MinValue),
            user.Mail, user.Phone, user.Address
        });
    }

    public async Task UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE users SET Name=@Name, Gender=@Gender, Birth=@Birth, Mail=@Mail,
                Phone=@Phone, Address=@Address WHERE ID=@ID";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new
        {
            user.Name, user.Gender,
            Birth = user.Birth.ToDateTime(TimeOnly.MinValue),
            user.Mail, user.Phone, user.Address, user.ID
        });
    }

    public async Task SetActiveAsync(int userId, bool isActive)
    {
        const string sql = "UPDATE users SET IsActive=@IsActive WHERE ID=@ID";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { IsActive = isActive, ID = userId });
    }

    private class UserRow
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Gender { get; set; }
        public DateTime Birth { get; set; }
        public string Mail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public User ToUser() => new()
        {
            ID = ID, Name = Name, Gender = Gender,
            Birth = DateOnly.FromDateTime(Birth),
            Mail = Mail, Phone = Phone, Address = Address,
            IsActive = IsActive, CreatedAt = CreatedAt
        };
    }
}

// ---- LibrarianRepository ----
public class LibrarianRepository : ILibrarianRepository
{
    private readonly IDbConnectionFactory _factory;
    public LibrarianRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<Librarian?> GetByUserNameAsync(string userName)
    {
        const string sql = "SELECT ID, UserName, Name, Mail, Password, IsActive, CreatedAt, UpdatedAt FROM librarians WHERE UserName=@UserName";
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Librarian>(sql, new { UserName = userName });
    }

    public async Task<Librarian?> GetByIdAsync(int id)
    {
        const string sql = "SELECT ID, UserName, Name, Mail, Password, IsActive, CreatedAt, UpdatedAt FROM librarians WHERE ID=@ID";
        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Librarian>(sql, new { ID = id });
    }
}
