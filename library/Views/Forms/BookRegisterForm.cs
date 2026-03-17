using library.Views.Interfaces;
using System;
using System.Windows.Forms;

namespace library.View;

public partial class BookRegisterForm : Form, IBookRegisterView
{
    public string BookName => txtBookName.Text.Trim();
    public string Author => txtAuthor.Text.Trim();
    public string Publisher => txtPublisher.Text.Trim();
    public string Genre => txtGenre.Text.Trim();
    public string ISBN => txtISBN.Text.Trim();

    public event EventHandler? RegisterClicked;
    public event EventHandler? CancelClicked;

    public BookRegisterForm()
    {
        InitializeComponent();
        btnRegister.Click += (s, e) => RegisterClicked?.Invoke(this, EventArgs.Empty);
        btnCancel.Click += (s, e) => CancelClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ShowError(string message) =>
        MessageBox.Show(message, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public void NavigateToCompletion()
    {
        new CompletionForm().Show();
        this.Close();
    }

    private void BookRegisterForm_Load(object sender, EventArgs e) { }
}