using library.Model.Dto;
using library.Model.Services;
using library.Views.Interfaces;  // ← 修正
using NLog;

namespace library.Presenter;

public class BookRegisterPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IBookRegisterView _view;
    private readonly IBookService _bookService;

    public BookRegisterPresenter(IBookRegisterView view, IBookService bookService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
    }

    public async void OnRegisterClicked()
    {
        try
        {
            if (!ValidateInputs())
                return;

            var dto = new BookRegisterDto
            {
                BookName = _view.BookName.Trim(),
                Author = _view.Author.Trim(),
                Publisher = _view.Publisher.Trim(),
                Genre = _view.Genre.Trim(),
                ISBN = _view.ISBN.Trim()
            };

            // ISBN重複チェック（RegisterAsync内でも行うが、先にユーザーへ通知）
            var existing = await _bookService.GetByIsbnAsync(dto.ISBN);
            if (existing != null)
            {
                _view.ShowError("入力されたISBNはすでに登録されています。");
                return;
            }

            var result = await _bookService.RegisterAsync(dto);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "蔵書の登録に失敗しました。");
                return;
            }

            Logger.Info("蔵書登録成功: BookId={BookId}, ISBN={ISBN}", result.Value!.Id, dto.ISBN);
            _view.NavigateToCompletion();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "蔵書登録中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    public void OnCancelClicked() => _view.Close();

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(_view.BookName))
        { _view.ShowError("図書名を入力してください。"); return false; }
        if (_view.BookName.Trim().Length > 500)
        { _view.ShowError("図書名は500文字以内で入力してください。"); return false; }

        if (string.IsNullOrWhiteSpace(_view.Author))
        { _view.ShowError("著者名を入力してください。"); return false; }
        if (_view.Author.Trim().Length > 200)
        { _view.ShowError("著者名は200文字以内で入力してください。"); return false; }

        if (string.IsNullOrWhiteSpace(_view.Publisher))
        { _view.ShowError("出版社を入力してください。"); return false; }
        if (_view.Publisher.Trim().Length > 200)
        { _view.ShowError("出版社は200文字以内で入力してください。"); return false; }

        if (string.IsNullOrWhiteSpace(_view.Genre))
        { _view.ShowError("ジャンルを入力してください。"); return false; }
        if (_view.Genre.Trim().Length > 100)
        { _view.ShowError("ジャンルは100文字以内で入力してください。"); return false; }

        if (string.IsNullOrWhiteSpace(_view.ISBN))
        { _view.ShowError("ISBNを入力してください。"); return false; }

        var isbnDigits = _view.ISBN.Trim().Replace("-", "");
        if (!System.Text.RegularExpressions.Regex.IsMatch(isbnDigits, @"^\d{10}$|^\d{13}$"))
        { _view.ShowError("ISBNは10桁または13桁の数字で入力してください。"); return false; }

        return true;
    }
}