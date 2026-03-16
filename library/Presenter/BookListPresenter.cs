using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using library.Model.Services;
using library.Presenter.Views;
using NLog;
using library.Model.Dto;
using library.Model.Entities;
using library.Model.Repositories;

namespace library.Presenter;

/// <summary>
/// 蔵書一覧・検索画面のPresenter。
/// 検索条件の構築・一覧表示・ステータス表示を担当する。
/// </summary>
public class BookListPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IBookListView _view;
    private readonly IBookService _bookService;

    public BookListPresenter(IBookListView view, IBookService bookService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
    }

    /// <summary>
    /// 画面初期表示時に呼び出す。全件を表示する。
    /// </summary>
    public async void OnLoad()
    {
        await ExecuteSearch();
    }

    /// <summary>
    /// 検索ボタン押下時に呼び出す。
    /// </summary>
    public async void OnSearchClicked()
    {
        await ExecuteSearch();
    }

    private async Task ExecuteSearch()
    {
        try
        {
            // 蔵書IDが入力されている場合は完全一致でID検索を優先する
            if (!string.IsNullOrWhiteSpace(_view.SearchBookId))
            {
                if (!int.TryParse(_view.SearchBookId, out int bookId))
                {
                    _view.ShowError("蔵書IDは数値で入力してください。");
                    return;
                }

                var book = await _bookService.GetByIdAsync(bookId);
                _view.ShowBooks(book != null ? new List<Book> { book } : new List<Book>());
                return;
            }

            var criteria = new BookSearchCriteria
            {
                BookName = _view.SearchBookName?.Trim(),
                Author = _view.SearchAuthor?.Trim(),
                Publisher = _view.SearchPublisher?.Trim(),
                Genre = _view.SearchGenre?.Trim(),
                Statuses = _view.SearchStatuses?.Select(s => (byte)s).ToList()
            };

            var books = await _bookService.SearchAsync(criteria);
            _view.ShowBooks(books);

            Logger.Info("蔵書検索実行: 件数={Count}", books.Count);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "蔵書検索中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }
}