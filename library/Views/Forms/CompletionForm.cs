using LibrarySystem.Model.DTOs;
using LibrarySystem.Presenter;
using LibrarySystem.Presenter.Views;
using System;
using System.Windows.Forms;

namespace library.View;

public partial class CompletionForm : Form, ICompletionView
{
    // -------------------------------------------------------
    // ICompletionView イベント実装
    // -------------------------------------------------------
    public event EventHandler? BackToTopClicked;

    // -------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------
    public CompletionForm(CompletionViewModel vm)
    {
        InitializeComponent();

        var presenter = new CompletionPresenter(this);
        presenter.Initialize(vm);

        btnBack.Click += (s, e) => BackToTopClicked?.Invoke(this, EventArgs.Empty);
    }

    // -------------------------------------------------------
    // ICompletionView メソッド実装
    // -------------------------------------------------------
    public void ShowMessage(string message)
    {
        lblMessage.Text = message;
    }

    public void StartAutoReturnTimer(int seconds)
    {
        var timer = new System.Windows.Forms.Timer { Interval = seconds * 1000 };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            NavigateToTop();
        };
        timer.Start();

        // カウントダウン表示（任意）
        lblCountdown.Text = $"※{seconds}秒後に自動で戻ります";
    }

    public void NavigateToTop()
    {
        this.Close();
    }

    // -------------------------------------------------------
    // フォームイベント（デザイナ生成）
    // -------------------------------------------------------
    private void CompletionForm_Load(object sender, EventArgs e) { }
    private void lblMessage_Click(object sender, EventArgs e) { }
    private void lblCountdown_Click(object sender, EventArgs e) { }
}