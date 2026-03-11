using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;
using NLog;

namespace LibrarySystem.Presenter;

/// <summary>
/// 利用者管理画面のPresenter（司書操作）。
/// 利用者CRUD・入力検証を担当する。
/// </summary>
public class UserManagePresenter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    // RFC5322簡易チェック用正規表現
    private static readonly Regex MailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    // 電話番号：数字・ハイフンのみ、実質的な桁数は 7〜15桁
    private static readonly Regex PhoneRegex =
        new(@"^[\d\-]{7,15}$", RegexOptions.Compiled);

    private readonly IUserManageView _view;
    private readonly IUserService _userService;

    public UserManagePresenter(IUserManageView view, IUserService userService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    // -------------------------------------------------------
    // 検索
    // -------------------------------------------------------

    /// <summary>
    /// 検索ボタン押下時に呼び出す。
    /// </summary>
    public void OnSearchClicked()
    {
        try
        {
            var users = _userService.Search(_view.SearchName?.Trim() ?? string.Empty);
            _view.ShowUsers(users);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "利用者検索中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    // -------------------------------------------------------
    // 新規登録
    // -------------------------------------------------------

    /// <summary>
    /// 登録ボタン押下時に呼び出す。
    /// </summary>
    public void OnCreateClicked()
    {
        try
        {
            if (!ValidateInputs())
                return;

            var dto = BuildDto();
            var result = _userService.CreateUser(dto);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "利用者の登録に失敗しました。");
                return;
            }

            Logger.Info("利用者登録成功: UserId={UserId}", result.Value!.ID);
            _view.ClearInputs();
            OnSearchClicked();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "利用者登録中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    // -------------------------------------------------------
    // 更新
    // -------------------------------------------------------

    /// <summary>
    /// 更新ボタン押下時に呼び出す。
    /// </summary>
    public void OnUpdateClicked()
    {
        try
        {
            if (_view.SelectedUserId == null)
            {
                _view.ShowError("更新する利用者を選択してください。");
                return;
            }

            if (!ValidateInputs())
                return;

            var dto = BuildDto();
            var result = _userService.UpdateUser(_view.SelectedUserId.Value, dto);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage ?? "利用者の更新に失敗しました。");
                return;
            }

            Logger.Info("利用者更新成功: UserId={UserId}", _view.SelectedUserId.Value);
            _view.ClearInputs();
            OnSearchClicked();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "利用者更新中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    // -------------------------------------------------------
    // 論理削除
    // -------------------------------------------------------

    /// <summary>
    /// 削除（論理削除）ボタン押下時に呼び出す。
    /// </summary>
    public void OnDeleteClicked()
    {
        try
        {
            if (_view.SelectedUserId == null)
            {
                _view.ShowError("削除する利用者を選択してください。");
                return;
            }

            var result = _userService.DeactivateUser(_view.SelectedUserId.Value);
            if (!result)
            {
                _view.ShowError("利用者の削除に失敗しました。");
                return;
            }

            Logger.Info("利用者論理削除成功: UserId={UserId}", _view.SelectedUserId.Value);
            _view.ClearInputs();
            OnSearchClicked();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "利用者削除中にシステムエラーが発生しました。");
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    // -------------------------------------------------------
    // バリデーション
    // -------------------------------------------------------

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(_view.InputName))
        {
            _view.ShowError("氏名を入力してください。");
            return false;
        }
        if (_view.InputName.Trim().Length > 100)
        {
            _view.ShowError("氏名は100文字以内で入力してください。");
            return false;
        }

        if (_view.InputBirth == default || _view.InputBirth >= DateTime.Today)
        {
            _view.ShowError("生年月日は過去の日付を入力してください。");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_view.InputMail))
        {
            _view.ShowError("メールアドレスを入力してください。");
            return false;
        }
        if (!MailRegex.IsMatch(_view.InputMail.Trim()))
        {
            _view.ShowError("メールアドレスの形式が正しくありません。");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_view.InputPhone))
        {
            _view.ShowError("電話番号を入力してください。");
            return false;
        }
        if (!PhoneRegex.IsMatch(_view.InputPhone.Trim()))
        {
            _view.ShowError("電話番号は数字・ハイフンのみ、7〜15桁で入力してください。");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_view.InputAddress))
        {
            _view.ShowError("住所を入力してください。");
            return false;
        }
        if (_view.InputAddress.Trim().Length > 500)
        {
            _view.ShowError("住所は500文字以内で入力してください。");
            return false;
        }

        return true;
    }

    private UserDto BuildDto() => new()
    {
        Name = _view.InputName.Trim(),
        Gender = _view.InputGender,
        Birth = DateOnly.FromDateTime(_view.InputBirth),
        Mail = _view.InputMail.Trim(),
        Phone = _view.InputPhone.Trim(),
        Address = _view.InputAddress.Trim()
    };
}