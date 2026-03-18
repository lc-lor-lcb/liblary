using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;

namespace LibrarySystem.Presenter;

/// <summary>ログイン画面Presenter</summary>
public class LoginPresenter
{
    private readonly ILoginView _view;
    private readonly IAuthService _authService;

    public LoginPresenter(ILoginView view, IAuthService authService)
    {
        _view = view;
        _authService = authService;
        _view.LoginClicked += OnLoginClicked;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        try
        {
            // 空チェック
            if (string.IsNullOrWhiteSpace(_view.UserName) || string.IsNullOrWhiteSpace(_view.Password))
            {
                _view.ShowError("ログインIDとパスワードを入力してください。");
                return;
            }

            var result = await _authService.LoginAsync(_view.UserName, _view.Password);

            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage!);
                return;
            }

            _view.NavigateToMain();
        }
        catch (Exception ex)
        {
            AppLogger.Error("ログイン処理中にエラーが発生しました", ex);
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }
}
