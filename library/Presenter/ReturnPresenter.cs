using library.Model.Services;
using library.Presenter.Views;
using NLog;

namespace library.Presenter;

public class ReturnPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IReturnView _view;
    private readonly ILoanService _loanService;

    public ReturnPresenter(IReturnView view, ILoanService loanService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
    }

    public async void OnReturnClicked()
    {
        try
        {
            if (!TryParseIds(out int userId, out int bookId))
                return;

            var result = await _loanService.ReturnAsync(bookId, userId);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "返却処理に失敗しました。");
                return;
            }

            Logger.Info("返却成功: UserId={UserId}, BookId={BookId}, HasReservation={HasReservation}",
                userId, bookId, result.HasNextReservation);

            _view.NavigateToCompletion();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "返却処理中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private bool TryParseIds(out int userId, out int bookId)
    {
        userId = 0; bookId = 0;

        if (string.IsNullOrWhiteSpace(_view.UserId))
        { _view.ShowError("利用者IDを入力してください。"); return false; }
        if (!int.TryParse(_view.UserId, out userId) || userId <= 0)
        { _view.ShowError("利用者IDは正の整数で入力してください。"); return false; }

        if (string.IsNullOrWhiteSpace(_view.BookId))
        { _view.ShowError("蔵書IDを入力してください。"); return false; }
        if (!int.TryParse(_view.BookId, out bookId) || bookId <= 0)
        { _view.ShowError("蔵書IDは正の整数で入力してください。"); return false; }

        return true;
    }
}