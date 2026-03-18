using LibrarySystem.Infrastructure;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter;
using LibrarySystem.Presenter.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace library.View;

public partial class UserManageForm : Form, IUserManageView
{
    // -------------------------------------------------------
    // IUserManageView プロパティ実装
    // -------------------------------------------------------
    public string SearchName => txtSearchName.Text.Trim();
    public string InputName => txtName.Text.Trim();

    /// <summary>
    /// IUserManageView は bool（false:男性 / true:女性）を要求する。
    /// ラジオボタンの選択を bool に変換する。
    /// </summary>
    public bool InputGender => rbFemale.Checked;

    public string InputBirth => txtBirth.Text.Trim();
    public string InputMail => txtMail.Text.Trim();
    public string InputPhone => txtPhone.Text.Trim();
    public string InputAddress => txtAddress.Text.Trim();

    /// <summary>
    /// DataGridView で選択中の行の利用者ID。
    /// 未選択または変換失敗時は null。
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? SelectedUserId
    {
        get
        {
            if (dgvUsers.CurrentRow == null) return null;
            var val = dgvUsers.CurrentRow.Cells[0].Value;
            return val != null && int.TryParse(val.ToString(), out int id) ? id : null;
        }
    }

    // -------------------------------------------------------
    // IUserManageView イベント実装
    // -------------------------------------------------------
    public event EventHandler? SearchClicked;
    public event EventHandler? RegisterClicked;
    public event EventHandler? UpdateClicked;
    public event EventHandler? DeactivateClicked;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------
    public UserManageForm()
    {
        InitializeComponent();
        btnSearch.Click += (s, e) => SearchClicked?.Invoke(this, EventArgs.Empty);
        btnRegister.Click += (s, e) => RegisterClicked?.Invoke(this, EventArgs.Empty);
        btnUpdate.Click += (s, e) => UpdateClicked?.Invoke(this, EventArgs.Empty);

        // 無効化ボタンをデザイナに追加した場合は以下を有効にする
        // btnDeactivate.Click += (s, e) => DeactivateClicked?.Invoke(this, EventArgs.Empty);
    }

    // -------------------------------------------------------
    // IUserManageView メソッド実装
    // -------------------------------------------------------

    /// <summary>
    /// ShowUsers(IEnumerable<UserListItemViewModel>) →
    /// ShowUsers(IList<User>) に変更。
    /// エンティティから直接表示項目を取り出す。
    /// </summary>
    public void ShowUsers(IList<User> users)
    {
        dgvUsers.Rows.Clear();
        foreach (var u in users)
        {
            dgvUsers.Rows.Add(
                u.ID,
                u.Name,
                u.Gender ? "女性" : "男性",
                u.Birth.ToString("yyyy/MM/dd"),
                u.Mail,
                u.Phone,
                u.IsActive ? "有効" : "無効"
            );
        }
    }

    /// <summary>選択した利用者の情報を入力フィールドに反映する</summary>
    public void ShowUserDetail(User user)
    {
        txtName.Text = user.Name;
        rbMale.Checked = !user.Gender;
        rbFemale.Checked = user.Gender;
        txtBirth.Text = user.Birth.ToString("yyyy/MM/dd");
        txtMail.Text = user.Mail;
        txtPhone.Text = user.Phone;
        txtAddress.Text = user.Address;
    }

    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public void ShowSuccess(string message) =>
        MessageBox.Show(message, "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public void ClearInputs()
    {
        txtName.Clear();
        txtBirth.Clear();
        txtMail.Clear();
        txtPhone.Clear();
        txtAddress.Clear();
        rbMale.Checked = true;
    }

    // -------------------------------------------------------
    // フォームイベント（デザイナ生成）
    // -------------------------------------------------------
    private void UserManageForm_Load(object sender, EventArgs e)
    {
        string connStr = ConnectionConfig.ConnectionString;
        var factory = new SqlConnectionFactory(connStr);
        var userRepo = new UserRepository(factory);
        var userService = new UserService(userRepo);

        _ = new UserManagePresenter(this, userService);
    }

    private void label4_Click(object sender, EventArgs e) { }
}