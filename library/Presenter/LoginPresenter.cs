using library.Model.Services;
using library.Presenter.Views;
using library.Views.Interfaces;  // ← 修正
using NLog;

namespace library.Presenter;

public class LoginPresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly ILoginView _view;
    private readonly IAuthService _authService;

    public LoginPresenter(ILoginView view, IAuthService authService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async void OnLoginClicked()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_view.UserName))
            { _view.ShowError("ログインIDを入力してください。"); return; }

            if (string.IsNullOrWhiteSpace(_view.Password))
            { _view.ShowError("パスワードを入力してください。"); return; }

            var librarian = await _authService.LoginAsync(_view.UserName, _view.Password);

            if (librarian == null)
            {
                _view.ShowError("IDまたはパスワードが正しくありません。");
                Logger.Warn("ログイン失敗: UserName={UserName}", _view.UserName);
                return;
            }

            Logger.Info("ログイン成功: UserName={UserName}", librarian.UserName);
            _view.NavigateToMain();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "ログイン処理中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }
}