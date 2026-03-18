using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;
using System.Text.RegularExpressions;

namespace LibrarySystem.Presenter;

/// <summary>蔵書新規登録画面Presenter</summary>
public class BookRegisterPresenter
{
    private readonly IBookRegisterView _view;
    private readonly IBookService _bookService;

    public BookRegisterPresenter(IBookRegisterView view, IBookService bookService)
    {
        _view = view;
        _bookService = bookService;

        _view.RegisterClicked += async (s, e) => await OnRegisterClickedAsync();
        _view.CancelClicked += (s, e) => _view.Close();
    }

    private async Task OnRegisterClickedAsync()
    {
        try
        {
            var error = Validate();
            if (error != null)
            {
                _view.ShowError(error);
                return;
            }

            var dto = new BookRegisterDto
            {
                BookName = _view.BookName.Trim(),
                Author = _view.Author.Trim(),
                Publisher = _view.Publisher.Trim(),
                Genre = _view.Genre.Trim(),
                ISBN = _view.ISBN.Trim()
            };

            var result = await _bookService.RegisterAsync(dto);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage!);
                return;
            }

            _view.NavigateToCompletion(new CompletionViewModel { Message = "操作が完了しました" });
        }
        catch (Exception ex)
        {
            AppLogger.Error("蔵書登録処理中にエラーが発生しました", ex);
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private string? Validate()
    {
        if (string.IsNullOrWhiteSpace(_view.BookName) || _view.BookName.Trim().Length > 500)
            return "図書名を1〜500文字で入力してください。";
        if (string.IsNullOrWhiteSpace(_view.Author) || _view.Author.Trim().Length > 200)
            return "著者名を1〜200文字で入力してください。";
        if (string.IsNullOrWhiteSpace(_view.Publisher) || _view.Publisher.Trim().Length > 200)
            return "出版社を1〜200文字で入力してください。";
        if (string.IsNullOrWhiteSpace(_view.Genre) || _view.Genre.Trim().Length > 100)
            return "ジャンルを1〜100文字で入力してください。";
        if (string.IsNullOrWhiteSpace(_view.ISBN))
            return "ISBNを入力してください。";
        if (!Regex.IsMatch(_view.ISBN.Trim(), @"^\d{10}$|^\d{13}$"))
            return "ISBNは10桁または13桁の数字で入力してください。";
        return null;
    }
}
