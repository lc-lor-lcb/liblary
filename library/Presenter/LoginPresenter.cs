using System;
using System.Collections.Generic;
using System.Text;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;
using NLog;

namespace LibrarySystem.Presenter;

/// <summary>
/// ログイン画面のPresenter。
/// 入力バリデーション・認証呼び出し・画面遷移を担当する。
/// </summary>
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

    /// <summary>
    /// ログインボタン押下時に呼び出す。
    /// </summary>
    public void OnLoginClicked()
    {
        try
        {
            // --- バリデーション ---
            if (string.IsNullOrWhiteSpace(_view.UserName))
            {
                _view.ShowError("ログインIDを入力してください。");
                return;
            }

            if (string.IsNullOrWhiteSpace(_view.Password))
            {
                _view.ShowError("パスワードを入力してください。");
                return;
            }

            // --- 認証 ---
            var librarian = _authService.Login(_view.UserName, _view.Password);

            if (librarian == null)
            {
                // どちらが誤りか特定できないメッセージにする（セキュリティ要件）
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