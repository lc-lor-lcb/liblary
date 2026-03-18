using LibrarySystem.Infrastructure;
using LibrarySystem.Model.DTOs;
using LibrarySystem.Model.Repositories;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter;
using LibrarySystem.Presenter.Views;
using System;
using System.Windows.Forms;

namespace library.View;

public partial class ReservationForm : Form, IReservationView
{
    // -------------------------------------------------------
    // IReservationView プロパティ実装
    // -------------------------------------------------------
    public string UserId => txtUserId.Text.Trim();
    public string BookId => txtBookId.Text.Trim();

    // -------------------------------------------------------
    // IReservationView イベント実装
    // -------------------------------------------------------
    public event EventHandler? ReserveClicked;
    public event EventHandler? CancelClicked;

    // 貸出画面から引き継いだ蔵書ID
    private readonly int? _prefilledBookId;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------
    public ReservationForm(int? prefilledBookId = null)
    {
        InitializeComponent();

        _prefilledBookId = prefilledBookId;
        if (prefilledBookId.HasValue)
        {
            txtBookId.Text = prefilledBookId.Value.ToString();
            txtBookId.ReadOnly = true;  // 引き継ぎIDは編集不可
        }

        btnReserve.Click += (s, e) => ReserveClicked?.Invoke(this, EventArgs.Empty);

        // キャンセルボタンをデザイナに追加した場合は以下を有効にする
        // btnCancel.Click += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);
    }

    // -------------------------------------------------------
    // IReservationView メソッド実装
    // -------------------------------------------------------
    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    /// <summary>
    /// 返却期限日をラベルに表示する。
    /// ShowReturnDueDate(DateTime?) → ShowReturnDue(DateOnly?) に変更。
    /// </summary>
    public void ShowReturnDue(DateOnly? returnDue) =>
        lblReturnDue.Text = returnDue.HasValue
            ? $"現在の返却期限：{returnDue.Value:yyyy/MM/dd}"
            : "返却期限：未設定";

    public void NavigateToCompletion(CompletionViewModel vm)
    {
        new CompletionForm(vm).Show();
        this.Hide();
    }

    // Close() は Form 基底実装をそのまま使用（明示実装不要）

    // -------------------------------------------------------
    // フォームイベント（デザイナ生成）
    // -------------------------------------------------------
    private void ReservationForm_Load(object sender, EventArgs e)
    {
        string connStr = ConnectionConfig.ConnectionString;
        var factory = new SqlConnectionFactory(connStr);
        var bookRepo = new BookRepository(factory);
        var logRepo = new LogRepository(factory);
        var reservationRepo = new ReservationRepository(factory);
        var userRepo = new UserRepository(factory);
        var reservationService = new ReservationService(reservationRepo, bookRepo, logRepo, userRepo);

        _ = new ReservationPresenter(this, reservationService, _prefilledBookId);
    }
}   