using library.Model.Common;
using library.Model.Dto;
using library.Model.Entities;
using library.Model.Repositories;

namespace library.Model.Services;

public interface IBookService
{
    Task<IList<Book>> SearchAsync(BookSearchCriteria criteria);
    Task<Book?> GetByIdAsync(int bookId);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<Result<Book>> RegisterAsync(BookRegisterDto dto);
}

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IList<Book>> SearchAsync(BookSearchCriteria criteria)
        => await _bookRepository.SearchAsync(criteria);

    public async Task<Book?> GetByIdAsync(int bookId)
        => await _bookRepository.GetByIdAsync(bookId);

    public async Task<Book?> GetByIsbnAsync(string isbn)
        => await _bookRepository.GetByIsbnAsync(isbn);

    public async Task<Result<Book>> RegisterAsync(BookRegisterDto dto)
    {
        var validationError = ValidateBookRegisterDto(dto);
        if (validationError != null)
            return Result<Book>.Failure(validationError);

        if (await _bookRepository.GetByIsbnAsync(dto.ISBN) != null)
            return Result<Book>.Failure($"ISBN「{dto.ISBN}」は既に登録されています。");

        var book = new Book
        {
            BookName = dto.BookName.Trim(),
            Author = dto.Author.Trim(),
            Publisher = dto.Publisher.Trim(),
            Genre = dto.Genre.Trim(),
            Isbn = dto.ISBN.Trim(),
            Status = BookStatus.Available,
            IsLoaned = false,
            IsReserved = false,
        };

        // ★ InsertAsync は int(ID) を返すので、IDで取得し直す
        var insertedId = await _bookRepository.InsertAsync(book);
        var inserted = await _bookRepository.GetByIdAsync(insertedId);
        if (inserted == null)
            return Result<Book>.Failure("蔵書の登録に失敗しました。");

        return Result<Book>.Success(inserted);
    }

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
        if (!isbn.All(char.IsDigit) || isbn.Length is not (10 or 13))
            return "ISBNは10桁または13桁の数字で入力してください。";
        return null;
    }
}