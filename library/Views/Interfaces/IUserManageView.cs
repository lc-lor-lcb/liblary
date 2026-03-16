using System;
using System.Collections.Generic;
using System.Text;

namespace library.UI.Interfaces;

public interface IUserManageView
{
    // 検索
    string SearchName { get; }
    event EventHandler SearchClicked;
    void ShowUsers(IEnumerable<UserListItemViewModel> users);

    // 新規登録・編集入力値
    string InputName { get; }
    int InputGender { get; }         // 0:男性 1:女性
    string InputBirth { get; }
    string InputMail { get; }
    string InputPhone { get; }
    string InputAddress { get; }

    event EventHandler RegisterClicked;
    event EventHandler UpdateClicked;

    void ShowError(string message);
    void ClearInputs();
    void SetInputs(UserListItemViewModel user);
}

public record UserListItemViewModel(
    int Id,
    string Name,
    string Gender,
    string Birth,
    string Mail,
    string Phone,
    string Address,
    bool IsActive
);