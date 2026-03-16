using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace library.View
{
    public partial class CheckoutForm : Form, ICheckoutView
    {
        public string UserId => txtUserId.Text.Trim();
        public string BookId => txtBookId.Text.Trim();

        public event EventHandler? CheckoutClicked;

        public CheckoutForm() { InitializeComponent(); btnCheckout.Click += (s, e) => CheckoutClicked?.Invoke(this, EventArgs.Empty); }

        public void ShowError(string message) =>
            MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public void ShowWarning(string message) =>
            MessageBox.Show(message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public void NavigateToCompletion()
        {
            new CompletionForm().Show();
            this.Hide();
        }

        public void NavigateToReservation(int bookId)
        {
            var form = new ReservationForm(bookId);
            form.Show();
            this.Hide();
        }
    }
}
