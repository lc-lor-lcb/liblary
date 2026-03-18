using LibrarySystem.Infrastructure;
using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;

namespace LibrarySystem.Presenter;

/// <summary>蔵書一覧・検索画面Presenter</summary>
public class BookListPresenter
{
    private readonly IBookListView _view;
    private readonly IBookService _bookService;
    private readonly SessionManager _session;

    public BookListPresenter(IBookListView view, IBookService bookService, SessionManager session)
    {
        _view = view;
        _bookService = bookService;
        _session = session;

        _view.SearchClicked += async (s, e) => await SearchAsync(resetPage: true);
        _view.PageNextClicked += async (s, e) => await SearchAsync(pageOffset: +1);
        _view.PagePrevClicked += async (s, e) => await SearchAsync(pageOffset: -1);
    }

    /// <summary>画面起動時に全件検索して表示</summary>
    public async Task InitializeAsync()
    {
        if (!_session.ValidateSession())
        {
            // セッション切れ→ログイン画面へは MainPresenter が制御するため、ここでは何もしない
            return;
        }
        await SearchAsync(resetPage: true);
    }

    private async Task SearchAsync(bool resetPage = false, int pageOffset = 0)
    {
        try
        {
            if (resetPage)
                _view.CurrentPage = 1;
            else
                _view.CurrentPage = Math.Max(1, _view.CurrentPage + pageOffset);

            var criteria = BuildCriteria();
            var books = await _bookService.SearchAsync(criteria);
            _view.ShowBooks(books);
        }
        catch (Exception ex)
        {
            AppLogger.Error("蔵書検索中にエラーが発生しました", ex);
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private BookSearchCriteria BuildCriteria()
    {
        int? bookId = null;
        if (!string.IsNullOrWhiteSpace(_view.SearchBookId) && int.TryParse(_view.SearchBookId, out int parsedId))
            bookId = parsedId;

        var statuses = _view.SelectedStatuses
            .Select(s => (BookStatus)s)
            .ToList();

        return new BookSearchCriteria
        {
            BookName = _view.SearchBookName,
            Author = _view.SearchAuthor,
            Publisher = _view.SearchPublisher,
            Genre = _view.SearchGenre,
            BookId = bookId,
            Statuses = statuses,
            PageNumber = _view.CurrentPage,
            PageSize = 100
        };
    }
}
