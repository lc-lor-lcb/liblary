using LibrarySystem.Infrastructure;
using LibrarySystem.Model.Entities;
using LibrarySystem.Model.Repositories;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter;
using LibrarySystem.Presenter.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace library.View;

public partial class BookListForm : Form, IBookListView
{
    // -------------------------------------------------------
    // IBookListView プロパティ実装
    // -------------------------------------------------------
    public string SearchBookName => txtTitle.Text.Trim();
    public string SearchAuthor => txtAuthor.Text.Trim();
    public string SearchPublisher => txtPublisher.Text.Trim();
    public string SearchGenre => txtGenre.Text.Trim();
    public string SearchBookId => txtBookId.Text.Trim();

    public IList<int> SelectedStatuses =>
        clbStatus.CheckedIndices.OfType<int>().ToList();

    // インターフェースの int CurrentPage { get; set; } はフィールドで保持する
    // [DesignerSerializationVisibility(Hidden)] でデザイナのシリアル化対象から除外
    private int _currentPage = 1;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CurrentPage
    {
        get => _currentPage;
        set => _currentPage = value;
    }

    // -------------------------------------------------------
    // IBookListView イベント実装
    // -------------------------------------------------------
    public event EventHandler? SearchClicked;
    // ページングボタンがデザイナにない場合も、イベント宣言は必須（インターフェース要件）
    public event EventHandler? PageNextClicked;
    public event EventHandler? PagePrevClicked;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------
    public BookListForm()
    {
        InitializeComponent();

        btnSearch.Click += (s, e) => SearchClicked?.Invoke(this, EventArgs.Empty);

        // ページングボタンはデザイナに存在しないため、
        // 追加する場合は以下のコメントを外してボタン名を合わせること
        // btnNext.Click += (s, e) => PageNextClicked?.Invoke(this, EventArgs.Empty);
        // btnPrev.Click += (s, e) => PagePrevClicked?.Invoke(this, EventArgs.Empty);
    }

    // -------------------------------------------------------
    // IBookListView メソッド実装
    // -------------------------------------------------------
    public void ShowBooks(IList<Book> books)
    {
        dgvBooks.Rows.Clear();

        var today = DateOnly.FromDateTime(DateTime.Today);

        foreach (var b in books)
        {
            string statusLabel = b.Status switch
            {
                BookStatus.InStock => "在庫",
                BookStatus.Loaned => "貸出中",
                BookStatus.Reserved => "予約済",
                BookStatus.Retired => "除籍",
                _ => b.Status.ToString()
            };

            string returnDueText = b.ReturnDue.HasValue
                ? b.ReturnDue.Value.ToString("yyyy/MM/dd")
                : "";

            bool isOverdue = b.ReturnDue.HasValue && b.ReturnDue.Value < today;

            int rowIdx = dgvBooks.Rows.Add(
                b.ID,
                b.BookName,
                b.Author,
                b.Publisher,
                b.Genre,
                statusLabel,
                returnDueText
            );

            if (isOverdue)
                dgvBooks.Rows[rowIdx].DefaultCellStyle.BackColor =
                    Color.FromArgb(0xFF, 0xE4, 0xE4);
        }
    }

    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    // -------------------------------------------------------
    // フォームイベント（デザイナ生成）
    // -------------------------------------------------------
    private async void BookListForm_Load(object sender, EventArgs e)
    {
        // ── DataGridView の列定義 ──────────────────────────────
        dgvBooks.AutoGenerateColumns = false;
        dgvBooks.Columns.Clear();

        dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colId",
            HeaderText = "ID",
            Width = 50,
            ReadOnly = true,
        });
        dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colBookName",
            HeaderText = "タイトル",
            Width = 180,
            ReadOnly = true,
        });
        dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colAuthor",
            HeaderText = "著者",
            Width = 120,
            ReadOnly = true,
        });
        dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colPublisher",
            HeaderText = "出版社",
            Width = 120,
            ReadOnly = true,
        });
        dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colGenre",
            HeaderText = "ジャンル",
            Width = 100,
            ReadOnly = true,
        });
        dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colStatus",
            HeaderText = "状態",
            Width = 70,
            ReadOnly = true,
        });
        dgvBooks.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colReturnDue",
            HeaderText = "返却期限",
            Width = 90,
            ReadOnly = true,
        });

        // ── Presenter 初期化（ここで ShowBooks が呼ばれる）──────
        string connStr = ConnectionConfig.ConnectionString;
        var factory = new SqlConnectionFactory(connStr);
        var bookRepo = new BookRepository(factory);
        var bookService = new BookService(bookRepo);

        var presenter = new BookListPresenter(this, bookService, SessionManager.Instance);
        await presenter.InitializeAsync();
    }

    // デザイナが生成するイベントハンドラ（本体は不要・空のまま残す）
    private void btnSearch_Click(object sender, EventArgs e) { }
    private void label4_Click(object sender, EventArgs e) { }
}