using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;

namespace LibrarySystem.Model.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepo;
    public BookService(IBookRepository bookRepo) => _bookRepo = bookRepo;

    public Task<IList<Book>> SearchAsync(BookSearchCriteria criteria)
        => _bookRepo.SearchAsync(criteria);

    public async Task<Result<Book>> GetByIdAsync(int bookId)
    {
        var book = await _bookRepo.GetByIdAsync(bookId);
        return book != null
            ? Result<Book>.Ok(book)
            : Result<Book>.Fail($"蔵書ID {bookId} は存在しません");
    }

    public async Task<Result<Book>> RegisterAsync(BookRegisterDto dto)
    {
        // ISBN重複チェック
        var existing = await _bookRepo.GetByIsbnAsync(dto.ISBN);
        if (existing != null)
            return Result<Book>.Fail($"ISBN「{dto.ISBN}」は既に登録されています（蔵書ID: {existing.ID}）");

        var book = new Book
        {
            BookName = dto.BookName,
            Author = dto.Author,
            Publisher = dto.Publisher,
            Genre = dto.Genre,
            ISBN = dto.ISBN,
            Status = BookStatus.InStock
        };

        var id = await _bookRepo.InsertAsync(book);
        book.ID = id;
        return Result<Book>.Ok(book);
    }
}
