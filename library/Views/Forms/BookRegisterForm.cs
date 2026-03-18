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

public partial class BookRegisterForm : Form, IBookRegisterView
{
    // -------------------------------------------------------
    // IBookRegisterView プロパティ実装
    // -------------------------------------------------------
    public string BookName => txtBookName.Text.Trim();
    public string Author => txtAuthor.Text.Trim();
    public string Publisher => txtPublisher.Text.Trim();
    public string Genre => txtGenre.Text.Trim();
    public string ISBN => txtISBN.Text.Trim();

    // -------------------------------------------------------
    // IBookRegisterView イベント実装
    // -------------------------------------------------------
    public event EventHandler? RegisterClicked;
    public event EventHandler? CancelClicked;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------
    public BookRegisterForm()
    {
        InitializeComponent();
        btnRegister.Click += (s, e) => RegisterClicked?.Invoke(this, EventArgs.Empty);
        btnCancel.Click += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);
    }

    // -------------------------------------------------------
    // IBookRegisterView メソッド実装
    // -------------------------------------------------------
    public void ShowError(string message) =>
        MessageBox.Show(message, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    /// <summary>
    /// Presenterから呼ばれる完了画面への遷移。
    /// CompletionViewModel を受け取り CompletionForm に渡す。
    /// </summary>
    public void NavigateToCompletion(CompletionViewModel vm)
    {
        var form = new CompletionForm(vm);
        form.Show();
        this.Close();
    }

    // Close() は Form の基底実装をそのまま使用するため明示実装不要

    // -------------------------------------------------------
    // フォームイベント（デザイナ生成）
    // -------------------------------------------------------
    private void BookRegisterForm_Load(object sender, EventArgs e)
    {
        string connStr = ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;
        var factory = new SqlConnectionFactory(connStr);
        var bookRepo = new BookRepository(factory);
        var bookService = new BookService(bookRepo);

        _ = new BookRegisterPresenter(this, bookService);
    }
}