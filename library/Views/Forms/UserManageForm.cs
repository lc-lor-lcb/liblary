using library.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace library.View;

public partial class UserManageForm : Form, IUserManageView
{
    public string SearchName => txtSearchName.Text.Trim();
    public string InputName => txtName.Text.Trim();
    public int InputGender => rbMale.Checked ? 0 : 1;
    public string InputBirth => txtBirth.Text.Trim();
    public string InputMail => txtMail.Text.Trim();
    public string InputPhone => txtPhone.Text.Trim();
    public string InputAddress => txtAddress.Text.Trim();

    public event EventHandler? SearchClicked;
    public event EventHandler? RegisterClicked;
    public event EventHandler? UpdateClicked;

    public UserManageForm()
    {
        InitializeComponent();
        btnSearch.Click += (s, e) => SearchClicked?.Invoke(this, EventArgs.Empty);
        btnRegister.Click += (s, e) => RegisterClicked?.Invoke(this, EventArgs.Empty);
        btnUpdate.Click += (s, e) => UpdateClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ShowUsers(IEnumerable<UserListItemViewModel> users)
    {
        dgvUsers.Rows.Clear();
        foreach (var u in users)
            dgvUsers.Rows.Add(u.Id, u.Name, u.Gender, u.Birth, u.Mail, u.Phone, u.IsActive ? "有効" : "無効");
    }

    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public void ClearInputs()
    {
        txtName.Clear(); txtBirth.Clear(); txtMail.Clear();
        txtPhone.Clear(); txtAddress.Clear();
        rbMale.Checked = true;
    }

    public void SetInputs(UserListItemViewModel u)
    {
        txtName.Text = u.Name;
        rbMale.Checked = u.Gender == "男性";
        rbFemale.Checked = u.Gender == "女性";
        txtBirth.Text = u.Birth;
        txtMail.Text = u.Mail;
        txtPhone.Text = u.Phone;
        txtAddress.Text = u.Address;
    }

    private void label4_Click(object sender, EventArgs e) { }
}