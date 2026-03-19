using Dapper;
using LibrarySystem.Infrastructure;
using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Entities;
using System.Text;

namespace LibrarySystem.Model.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IDbConnectionFactory _factory;
    public BookRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IList<Book>> SearchAsync(BookSearchCriteria criteria)
    {
        var sb = new StringBuilder(@"
            SELECT b.ID, b.BookName, b.Author, b.Publisher, b.Genre, b.ISBN,
                   b.Status, b.IsLoaned, b.IsReserved,
                   (SELECT TOP 1 l.ReturnDue FROM logs l
                    WHERE l.Book_id = b.ID AND l.ReturnDate IS NULL) AS ReturnDue
            FROM books b
            WHERE 1=1");

        var p = new DynamicParameters();

        if (criteria.BookId.HasValue)
        {
            sb.Append(" AND b.ID = @BookId");
            p.Add("BookId", criteria.BookId.Value);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(criteria.BookName))
            {
                sb.Append(" AND b.BookName LIKE @BookName");
                p.Add("BookName", $"%{criteria.BookName}%");
            }
            if (!string.IsNullOrWhiteSpace(criteria.Author))
            {
                sb.Append(" AND b.Author LIKE @Author");
                p.Add("Author", $"%{criteria.Author}%");
            }
            if (!string.IsNullOrWhiteSpace(criteria.Publisher))
            {
                sb.Append(" AND b.Publisher LIKE @Publisher");
                p.Add("Publisher", $"%{criteria.Publisher}%");
            }
            if (!string.IsNullOrWhiteSpace(criteria.Genre))
            {
                sb.Append(" AND b.Genre LIKE @Genre");
                p.Add("Genre", $"%{criteria.Genre}%");
            }
            if (criteria.Statuses.Count > 0)
            {
                sb.Append(" AND b.Status IN @Statuses");
                p.Add("Statuses", criteria.Statuses.Select(s => (byte)s).ToList());
            }
        }

        int offset = (criteria.PageNumber - 1) * criteria.PageSize;
        sb.Append(" ORDER BY b.ID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
        p.Add("Offset", offset);
        p.Add("PageSize", criteria.PageSize);

        using var conn = _factory.CreateConnection();
        var rows = await conn.QueryAsync<BookRow>(sb.ToString(), p);
        return rows.Select(r => r.ToBook()).ToList();
    }

    public async Task<Book?> GetByIdAsync(int bookId)
    {
        const string sql = @"
            SELECT b.ID, b.BookName, b.Author, b.Publisher, b.Genre, b.ISBN,
                   b.Status, b.IsLoaned, b.IsReserved,
                   (SELECT TOP 1 l.ReturnDue FROM logs l
                    WHERE l.Book_id = b.ID AND l.ReturnDate IS NULL) AS ReturnDue
            FROM books b WHERE b.ID = @ID";
        using var conn = _factory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<BookRow>(sql, new { ID = bookId });
        return row?.ToBook();
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        const string sql = "SELECT ID, BookName, Author, Publisher, Genre, ISBN, Status, IsLoaned, IsReserved FROM books WHERE ISBN = @ISBN";
        using var conn = _factory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<BookRow>(sql, new { ISBN = isbn });
        return row?.ToBook();
    }

    public async Task<int> InsertAsync(Book book)
    {
        const string sql = @"
            INSERT INTO books (BookName, Author, Publisher, Genre, ISBN, Status, IsLoaned, IsReserved)
            VALUES (@BookName, @Author, @Publisher, @Genre, @ISBN, @Status, 0, 0);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        using var conn = _factory.CreateConnection();
        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            book.BookName, book.Author, book.Publisher, book.Genre, book.ISBN,
            Status = (byte)book.Status
        });
    }

    public async Task UpdateStatusAsync(int bookId, BookStatus status, bool isLoaned, bool isReserved)
    {
        const string sql = "UPDATE books SET Status = @Status, IsLoaned = @IsLoaned, IsReserved = @IsReserved WHERE ID = @ID";
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Status = (byte)status, IsLoaned = isLoaned, IsReserved = isReserved, ID = bookId });
    }

    // Dapper用の中間クラス（DateOnly変換）
    private class BookRow
    {
        public int ID { get; set; }
        public string BookName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public byte Status { get; set; }
        public bool IsLoaned { get; set; }
        public bool IsReserved { get; set; }
        public DateTime? ReturnDue { get; set; }

        public Book ToBook() => new()
        {
            ID = ID, BookName = BookName, Author = Author, Publisher = Publisher,
            Genre = Genre, ISBN = ISBN, Status = (BookStatus)Status,
            IsLoaned = IsLoaned, IsReserved = IsReserved,
            ReturnDue = ReturnDue.HasValue ? DateOnly.FromDateTime(ReturnDue.Value) : null
        };
    }
}
