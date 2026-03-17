using library.Views.Interfaces;
using System;
using System.Windows.Forms;

namespace library.View;

public partial class CompletionForm : Form, ICompletionView
{
    private System.Windows.Forms.Timer _timer = new();
    private int _remaining;

    public event EventHandler? BackToTopClicked;

    public CompletionForm()
    {
        InitializeComponent();
        btnBack.Click += (s, e) => BackToTopClicked?.Invoke(this, EventArgs.Empty);
    }

    public void StartCountdown(int seconds)
    {
        _remaining = seconds;
        UpdateCountdownLabel();
        _timer.Interval = 1000;
        _timer.Tick += (s, e) =>
        {
            _remaining--;
            UpdateCountdownLabel();
            if (_remaining <= 0) { _timer.Stop(); NavigateToTop(); }
        };
        _timer.Start();
    }

    public void NavigateToTop()
    {
        _timer.Stop();
        this.Close();
    }

    private void UpdateCountdownLabel() =>
        lblCountdown.Text = $"※{_remaining}秒後に自動で戻ります";

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        StartCountdown(30);
    }

    private void CompletionForm_Load(object sender, EventArgs e) { }
    private void lblMessage_Click(object sender, EventArgs e) { }
    private void lblCountdown_Click(object sender, EventArgs e) { }
}