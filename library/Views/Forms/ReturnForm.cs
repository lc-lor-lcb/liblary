using LibrarySystem.Infrastructure;
using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Repositories;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter;
using LibrarySystem.Presenter.Views;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace library.View;

public partial class ReturnForm : Form, IReturnView
{
    // -------------------------------------------------------
    // IReturnView プロパティ実装
    // -------------------------------------------------------
    public string UserId => txtUserId.Text.Trim();
    public string BookId => txtBookId.Text.Trim();

    // -------------------------------------------------------
    // IReturnView イベント実装
    // -------------------------------------------------------
    public event EventHandler? ReturnClicked;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------
    public ReturnForm()
    {
        InitializeComponent();
        btnReturn.Click += (s, e) => ReturnClicked?.Invoke(this, EventArgs.Empty);
    }

    // -------------------------------------------------------
    // IReturnView メソッド実装
    // -------------------------------------------------------
    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public void NavigateToCompletion(CompletionViewModel vm)
    {
        new CompletionForm(vm).Show();
        this.Hide();
    }

    // -------------------------------------------------------
    // フォームイベント（デザイナ生成）
    // -------------------------------------------------------
    private void ReturnForm_Load(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;
        var factory = new SqlConnectionFactory(connStr);
        var bookRepo = new BookRepository(factory);
        var logRepo = new LogRepository(factory);
        var reservationRepo = new ReservationRepository(factory);
        var userRepo = new UserRepository(factory);
        var loanService = new LoanService(bookRepo, logRepo, reservationRepo, userRepo);

        _ = new ReturnPresenter(this, loanService);
    }
}