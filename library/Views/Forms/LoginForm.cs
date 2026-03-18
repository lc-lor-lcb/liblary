using LibrarySystem.Infrastructure;
using LibrarySystem.Model.Repositories;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter;
using LibrarySystem.Presenter.Views;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace library.View;

public partial class LoginForm : Form, ILoginView
{
    // -------------------------------------------------------
    // ILoginView プロパティ実装
    // -------------------------------------------------------
    public string UserName => txtUserName.Text.Trim();
    public string Password => txtPassword.Text;

    // -------------------------------------------------------
    // ILoginView イベント実装
    // -------------------------------------------------------
    public event EventHandler? LoginClicked;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------
    public LoginForm()
    {
        InitializeComponent();
        btnLogin.Click += (s, e) => LoginClicked?.Invoke(this, EventArgs.Empty);
    }

    // -------------------------------------------------------
    // ILoginView メソッド実装
    // -------------------------------------------------------
    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public void NavigateToMain()
    {
        var main = new MainForm();
        main.Show();
        this.Hide();
    }

    // -------------------------------------------------------
    // フォームイベント（デザイナ生成）
    // -------------------------------------------------------
    private void LoginForm_Load(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;
        var factory = new SqlConnectionFactory(connStr);
        var librarianRepo = new LibrarianRepository(factory);
        var authService = new AuthService(librarianRepo, SessionManager.Instance);

        _ = new LoginPresenter(this, authService);
    }

    private void label1_Click(object sender, EventArgs e) { }
    private void label2_Click(object sender, EventArgs e) { }
    private void btnLogin_Click(object sender, EventArgs e) { }
}