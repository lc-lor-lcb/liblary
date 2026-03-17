using library.Views.Interfaces;
using System;
using System.Windows.Forms;

namespace library.View;

public partial class LoginForm : Form, ILoginView
{
    public string UserName => txtUserName.Text.Trim();
    public string Password => txtPassword.Text;

    public event EventHandler? LoginClicked;

    public LoginForm()
    {
        InitializeComponent();
        btnLogin.Click += (s, e) => LoginClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public void NavigateToMain()
    {
        var main = new MainForm();
        main.Show();
        this.Hide();
    }

    private void LoginForm_Load(object sender, EventArgs e) { }
    private void label1_Click(object sender, EventArgs e) { }
    private void label2_Click(object sender, EventArgs e) { }
    private void btnLogin_Click(object sender, EventArgs e) { }
}