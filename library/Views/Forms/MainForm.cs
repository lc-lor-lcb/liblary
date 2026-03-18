using LibrarySystem.Infrastructure;
using LibrarySystem.Presenter.Views;
using System;
using System.Windows.Forms;

namespace library.View;

public partial class MainForm : Form, IMainView
{
    // -------------------------------------------------------
    // IMainView イベント実装
    // インターフェース定義名に合わせる（～MenuClicked）
    // -------------------------------------------------------
    public event EventHandler? BookListMenuClicked;
    public event EventHandler? BookRegisterMenuClicked;
    public event EventHandler? UserManageMenuClicked;
    public event EventHandler? LogoutMenuClicked;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------
    public MainForm()
    {
        InitializeComponent();
        btnBookList.Click += (s, e) => BookListMenuClicked?.Invoke(this, EventArgs.Empty);
        btnBookRegister.Click += (s, e) => BookRegisterMenuClicked?.Invoke(this, EventArgs.Empty);
        btnUserManage.Click += (s, e) => UserManageMenuClicked?.Invoke(this, EventArgs.Empty);
        btnLogout.Click += (s, e) => LogoutMenuClicked?.Invoke(this, EventArgs.Empty);

        // ボタンクリックで各画面を直接開く
        BookListMenuClicked += (s, e) => OpenBookList();
        BookRegisterMenuClicked += (s, e) => OpenBookRegister();
        UserManageMenuClicked += (s, e) => OpenUserManage();
        LogoutMenuClicked += (s, e) => NavigateToLogin();
    }

    // -------------------------------------------------------
    // IMainView メソッド実装
    // -------------------------------------------------------
    public void ShowLibrarianName(string name) =>
        lblLibrarianName.Text = name;

    public void OpenBookList()
    {
        new BookListForm().Show();
    }

    public void OpenBookRegister()
    {
        new BookRegisterForm().Show();
    }

    public void OpenUserManage()
    {
        new UserManageForm().Show();
    }

    public void NavigateToLogin()
    {
        SessionManager.Instance.Logout();
        new LoginForm().Show();
        this.Close();
    }
}