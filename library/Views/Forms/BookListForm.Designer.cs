namespace library.View
{
    partial class BookListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtTitle = new TextBox();
            txtAuthor = new TextBox();
            txtPublisher = new TextBox();
            txtGenre = new TextBox();
            txtBookId = new TextBox();
            btnSearch = new Button();
            dgvBooks = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            clbStatus = new CheckedListBox();
            ((System.ComponentModel.ISupportInitialize)dgvBooks).BeginInit();
            SuspendLayout();
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(23, 52);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(100, 23);
            txtTitle.TabIndex = 0;
            // 
            // txtAuthor
            // 
            txtAuthor.Location = new Point(23, 115);
            txtAuthor.Name = "txtAuthor";
            txtAuthor.Size = new Size(100, 23);
            txtAuthor.TabIndex = 1;
            // 
            // txtPublisher
            // 
            txtPublisher.Location = new Point(23, 185);
            txtPublisher.Name = "txtPublisher";
            txtPublisher.Size = new Size(100, 23);
            txtPublisher.TabIndex = 2;
            // 
            // txtGenre
            // 
            txtGenre.Location = new Point(153, 52);
            txtGenre.Name = "txtGenre";
            txtGenre.Size = new Size(100, 23);
            txtGenre.TabIndex = 3;
            // 
            // txtBookId
            // 
            txtBookId.Location = new Point(153, 117);
            txtBookId.Name = "txtBookId";
            txtBookId.Size = new Size(100, 23);
            txtBookId.TabIndex = 4;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(295, 52);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(120, 23);
            btnSearch.TabIndex = 6;
            btnSearch.Text = "検索";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // dgvBooks
            // 
            dgvBooks.AllowUserToAddRows = false;
            dgvBooks.AllowUserToDeleteRows = false;
            dgvBooks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBooks.Location = new Point(12, 229);
            dgvBooks.Name = "dgvBooks";
            dgvBooks.ReadOnly = true;
            dgvBooks.Size = new Size(776, 209);
            dgvBooks.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 34);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 8;
            label1.Text = "タイトル";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 97);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 9;
            label2.Text = "著者";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 167);
            label3.Name = "label3";
            label3.Size = new Size(43, 15);
            label3.TabIndex = 10;
            label3.Text = "出版社";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(142, 34);
            label4.Name = "label4";
            label4.Size = new Size(44, 15);
            label4.TabIndex = 11;
            label4.Text = "ジャンル";
            label4.Click += label4_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(142, 97);
            label5.Name = "label5";
            label5.Size = new Size(18, 15);
            label5.TabIndex = 12;
            label5.Text = "ID";
            // 
            // clbStatus
            // 
            clbStatus.FormattingEnabled = true;
            clbStatus.Items.AddRange(new object[] { "在庫", "貸出中", "予約済", "除籍" });
            clbStatus.Location = new Point(295, 132);
            clbStatus.Name = "clbStatus";
            clbStatus.Size = new Size(120, 76);
            clbStatus.TabIndex = 13;
            // 
            // BookListForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(clbStatus);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dgvBooks);
            Controls.Add(btnSearch);
            Controls.Add(txtBookId);
            Controls.Add(txtGenre);
            Controls.Add(txtPublisher);
            Controls.Add(txtAuthor);
            Controls.Add(txtTitle);
            Name = "BookListForm";
            Text = "BookListForm";
            Load += BookListForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvBooks).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtTitle;
        private TextBox txtAuthor;
        private TextBox txtPublisher;
        private TextBox txtGenre;
        private TextBox txtBookId;
        private Button btnSearch;
        private DataGridView dgvBooks;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private CheckedListBox clbStatus;
    }
}