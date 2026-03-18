using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;

namespace LibrarySystem.Presenter;

/// <summary>蔵書貸し出し画面Presenter</summary>
public class CheckoutPresenter
{
    private readonly ICheckoutView _view;
    private readonly ILoanService _loanService;

    public CheckoutPresenter(ICheckoutView view, ILoanService loanService)
    {
        _view = view;
        _loanService = loanService;
        _view.CheckoutClicked += async (s, e) => await OnCheckoutClickedAsync();
    }

    private async Task OnCheckoutClickedAsync()
    {
        try
        {
            // 入力バリデーション
            if (!TryParseIds(out int userId, out int bookId, out string? parseError))
            {
                _view.ShowError(parseError!);
                return;
            }

            var result = await _loanService.CheckoutAsync(bookId, userId);

            if (!result.IsSuccess)
            {
                // 貸出中・予約済みの場合は予約画面への誘導も行う
                bool shouldNavigateToReservation =
                    result.ErrorMessage!.Contains("貸出中") || result.ErrorMessage.Contains("予約");

                _view.ShowError(result.ErrorMessage);

                if (shouldNavigateToReservation)
                    _view.NavigateToReservation(bookId);
                return;
            }

            // 延滞警告（処理は続行済みのため、完了前に警告表示）
            if (result.Value!.HasOverdueWarning)
                _view.ShowOverdueWarning();

            _view.NavigateToCompletion(new CompletionViewModel { Message = "操作が完了しました" });
        }
        catch (Exception ex)
        {
            AppLogger.Error("貸出処理中にエラーが発生しました", ex);
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
