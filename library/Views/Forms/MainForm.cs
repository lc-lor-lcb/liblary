using library.UI.Interfaces;
using System;
using System.Windows.Forms;

namespace library.View;

public partial class MainForm : Form, IMainView
{
    public event EventHandler? BookListClicked;
    public event EventHandler? BookRegisterClicked;
    public event EventHandler? UserManageClicked;
    public event EventHandler? LogoutClicked;

    public MainForm()
    {
        InitializeComponent();
        btnBookList.Click += (s, e) => BookListClicked?.Invoke(this, EventArgs.Empty);
        btnBookRegister.Click += (s, e) => BookRegisterClicked?.Invoke(this, EventArgs.Empty);
        btnUserManage.Click += (s, e) => UserManageClicked?.Invoke(this, EventArgs.Empty);
        btnLogout.Click += (s, e) => LogoutClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ShowLibrarianName(string name) =>
        lblLibrarianName.Text = name;
}