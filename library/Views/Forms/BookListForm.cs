using library.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace library.View;

public partial class BookListForm : Form, IBookListView
{
    public string SearchTitle => txtTitle.Text.Trim();
    public string SearchAuthor => txtAuthor.Text.Trim();
    public string SearchPublisher => txtPublisher.Text.Trim();
    public string SearchGenre => txtGenre.Text.Trim();
    public string SearchBookId => txtBookId.Text.Trim();
    public IReadOnlyList<int> SelectedStatuses =>
        clbStatus.CheckedIndices.Cast<int>().ToList();

    public event EventHandler? SearchClicked;

    public BookListForm()
    {
        InitializeComponent();
        btnSearch.Click += (s, e) => SearchClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ShowBooks(IEnumerable<BookListItemViewModel> books)
    {
        dgvBooks.Rows.Clear();
        foreach (var b in books)
        {
            var rowIdx = dgvBooks.Rows.Add(b.Id, b.BookName, b.Author, b.Publisher, b.Genre, b.StatusLabel, b.ReturnDue ?? "");
            if (b.IsOverdue)
                dgvBooks.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(0xFF, 0xE4, 0xE4);
        }
    }

    public void ShowError(string message) =>
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void BookListForm_Load(object sender, EventArgs e) { }
    private void btnSearch_Click(object sender, EventArgs e) { }
    private void label4_Click(object sender, EventArgs e) { }
}