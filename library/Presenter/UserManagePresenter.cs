using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;

namespace LibrarySystem.Presenter;

/// <summary>利用者管理画面Presenter</summary>
public class UserManagePresenter
{
    private readonly IUserManageView _view;
    private readonly IUserService _userService;

    public UserManagePresenter(IUserManageView view, IUserService userService)
    {
        _view = view;
        _userService = userService;

        _view.SearchClicked += async (s, e) => await OnSearchClickedAsync();
        _view.RegisterClicked += async (s, e) => await OnRegisterClickedAsync();
        _view.UpdateClicked += async (s, e) => await OnUpdateClickedAsync();
        _view.DeactivateClicked += async (s, e) => await OnDeactivateClickedAsync();
    }

    private async Task OnSearchClickedAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_view.SearchName))
            {
                _view.ShowError("氏名を入力してください。");
                return;
            }
            var users = await _userService.SearchAsync(_view.SearchName);
            _view.ShowUsers(users);
        }
        catch (Exception ex)
        {
            AppLogger.Error("利用者検索中にエラーが発生しました", ex);
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private async Task OnRegisterClickedAsync()
    {
        try
        {
            var dto = BuildDto();
            if (dto == null) return;

            var result = await _userService.CreateUserAsync(dto);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage!);
                return;
            }

            _view.ShowSuccess($"利用者「{result.Value!.Name}」（ID: {result.Value.ID}）を登録しました。");
            _view.ClearInputs();
        }
        catch (Exception ex)
        {
            AppLogger.Error("利用者登録中にエラーが発生しました", ex);
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private async Task OnUpdateClickedAsync()
    {
        try
        {
            if (_view.SelectedUserId == null)
            {
                _view.ShowError("更新する利用者を選択してください。");
                return;
            }

            var dto = BuildDto();
            if (dto == null) return;

            var result = await _userService.UpdateUserAsync(_view.SelectedUserId.Value, dto);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage!);
                return;
            }

            _view.ShowSuccess("利用者情報を更新しました。");
        }
        catch (Exception ex)
        {
            AppLogger.Error("利用者更新中にエラーが発生しました", ex);
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    private async Task OnDeactivateClickedAsync()
    {
        try
        {
            if (_view.SelectedUserId == null)
            {
                _view.ShowError("削除する利用者を選択してください。");
                return;
            }

            var result = await _userService.DeactivateAsync(_view.SelectedUserId.Value);
            if (!result.IsSuccess)
            {
                _view.ShowError(result.ErrorMessage!);
                return;
            }

            _view.ShowSuccess("利用者を無効化しました。");
            await OnSearchClickedAsync();
        }
        catch (Exception ex)
        {
            AppLogger.Error("利用者無効化中にエラーが発生しました", ex);
            _view.ShowError("システムエラーが発生しました。管理者へご連絡ください。");
        }
    }

    /// <summary>入力フィールドからDTOを組み立てる（バリデーションエラー時はnullを返しViewにエラー表示）</summary>
    private UserDto? BuildDto()
    {
        if (!DateOnly.TryParseExact(_view.InputBirth, "yyyy/MM/dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out DateOnly birth))
        {
            _view.ShowError("生年月日はYYYY/MM/DD形式で入力してください。");
            return null;
        }

        return new UserDto
        {
            Name = _view.InputName,
            Gender = _view.InputGender,
            Birth = birth,
            Mail = _view.InputMail,
            Phone = _view.InputPhone,
            Address = _view.InputAddress
        };
    }
}
