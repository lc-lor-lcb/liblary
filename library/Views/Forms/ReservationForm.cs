using library.Views.Interfaces;
using System;
using System.Windows.Forms;

namespace library.View;

public partial class ReservationForm : Form, IReservationView
{
    public string UserId => txtUserId.Text.Trim();
    public string BookId => txtBookId.Text.Trim();

    public event EventHandler? ReserveClicked;

    public ReservationForm(int? prefilledBookId = null)
    {
        InitializeComponent();
        if (prefilledBookId.HasValue)
            txtBookId.Text = prefilledBookId.Value.ToString();

        btnReserve.Click += (s, e) => ReserveClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ShowReturnDueDate(DateTime? returnDue) =>
        lblReturnDue.Text = returnDue.HasValue
            ? $"現在の返却期限：{returnDue.Value:yyyy/MM/dd}"
            : "返却期限：未設定";

    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public void NavigateToCompletion()
    {
        new CompletionForm().Show();
        this.Hide();
    }

    private void ReservationForm_Load(object sender, EventArgs e) { }
}