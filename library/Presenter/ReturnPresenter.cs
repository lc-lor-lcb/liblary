using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;

namespace LibrarySystem.Presenter;

/// <summary>蔵書返却画面Presenter</summary>
public class ReturnPresenter
{
    private readonly IReturnView _view;
    private readonly ILoanService _loanService;

    public ReturnPresenter(IReturnView view, ILoanService loanService)
    {
        _view = view;
        _loanService = loanService;
        _view.ReturnClicked += async (s, e) => await OnReturnClickedAsync();
    }

    private async Task OnReturnClickedAsync()
    {
        try
        {
            if (!TryParseIds(out int userId, out int bookId, out string? parseError))
            {
                _view.ShowError(parseError!);
                return;
            }

            var result = await _loanService.ReturnAsync(bookId, userId);

            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage!);
                return;
            }

            _view.NavigateToCompletion(new CompletionViewModel { Message = "操作が完了しました" });
        }
        catch (Exception ex)
        {
            AppLogger.Error("返却処理中にエラーが発生しました", ex);
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private bool TryParseIds(out int userId, out int bookId, out string? error)
    {
        userId = 0; bookId = 0; error = null;

        if (string.IsNullOrWhiteSpace(_view.UserId) || string.IsNullOrWhiteSpace(_view.BookId))
        {
            error = "利用者IDと蔵書IDを入力してください。";
            return false;
        }
        if (!int.TryParse(_view.UserId, out userId))
        {
            error = "利用者IDは数値で入力してください。";
            return false;
        }
        if (!int.TryParse(_view.BookId, out bookId))
        {
            error = "蔵書IDは数値で入力してください。";
            return false;
        }
        return true;
    }
}
