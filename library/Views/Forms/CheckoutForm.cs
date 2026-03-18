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

public partial class CheckoutForm : Form, ICheckoutView
{
    // -------------------------------------------------------
    // ICheckoutView プロパティ実装
    // -------------------------------------------------------
    public string UserId => txtUserId.Text.Trim();
    public string BookId => txtBookId.Text.Trim();

    // -------------------------------------------------------
    // ICheckoutView イベント実装
    // -------------------------------------------------------
    public event EventHandler? CheckoutClicked;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------
    public CheckoutForm()
    {
        InitializeComponent();
        btnCheckout.Click += (s, e) => CheckoutClicked?.Invoke(this, EventArgs.Empty);
    }

    // -------------------------------------------------------
    // ICheckoutView メソッド実装
    // -------------------------------------------------------
    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    /// <summary>延滞中警告（ShowWarning → ShowOverdueWarning に改名）</summary>
    public void ShowOverdueWarning() =>
        MessageBox.Show(
            "現在延滞中の蔵書があります。早めにご返却ください。",
            "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public void NavigateToCompletion(CompletionViewModel vm)
    {
        new CompletionForm(vm).Show();
        this.Hide();
    }

    public void NavigateToReservation(int bookId)
    {
        new ReservationForm(bookId).Show();
        this.Hide();
    }

    // -------------------------------------------------------
    // フォームイベント（デザイナ生成）
    // -------------------------------------------------------
    private void CheckoutForm_Load(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;
        var factory = new SqlConnectionFactory(connStr);
        var bookRepo = new BookRepository(factory);
        var logRepo = new LogRepository(factory);
        var reservationRepo = new ReservationRepository(factory);
        var userRepo = new UserRepository(factory);
        var loanService = new LoanService(bookRepo, logRepo, reservationRepo, userRepo);

        _ = new CheckoutPresenter(this, loanService);
    }
}