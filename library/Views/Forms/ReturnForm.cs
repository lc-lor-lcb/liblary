using library.Views.Interfaces;
using System;
using System.Windows.Forms;

namespace library.View;

public partial class ReturnForm : Form, IReturnView
{
    public string UserId => txtUserId.Text.Trim();
    public string BookId => txtBookId.Text.Trim();

    public event EventHandler? ReturnClicked;

    public ReturnForm()
    {
        InitializeComponent();
        btnReturn.Click += (s, e) => ReturnClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public void NavigateToCompletion()
    {
        new CompletionForm().Show();
        this.Hide();
    }

    private void ReturnForm_Load(object sender, EventArgs e) { }
}