namespace library.View
{
    partial class BookRegisterForm
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
            txtBookName = new TextBox();
            txtAuthor = new TextBox();
            txtPublisher = new TextBox();
            txtGenre = new TextBox();
            txtISBN = new TextBox();
            btnRegister = new Button();
            btnCancel = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            SuspendLayout();
            // 
            // txtBookName
            // 
            txtBookName.Location = new Point(253, 117);
            txtBookName.Name = "txtBookName";
            txtBookName.Size = new Size(100, 23);
            txtBookName.TabIndex = 0;
            txtBookName.Text = "txtBookName";
            // 
            // txtAuthor
            // 
            txtAuthor.Location = new Point(253, 173);
            txtAuthor.Name = "txtAuthor";
            txtAuthor.Size = new Size(100, 23);
            txtAuthor.TabIndex = 1;
            txtAuthor.Text = "txtAuthor";
            txtAuthor.TextChanged += txtAuthor_TextChanged;
            // 
            // txtPublisher
            // 
            txtPublisher.Location = new Point(253, 234);
            txtPublisher.Name = "txtPublisher";
            txtPublisher.Size = new Size(100, 23);
            txtPublisher.TabIndex = 2;
            txtPublisher.Text = "txtPublisher";
            txtPublisher.TextChanged += txtPublisher_TextChanged;
            // 
            // txtGenre
            // 
            txtGenre.Location = new Point(372, 117);
            txtGenre.Name = "txtGenre";
            txtGenre.Size = new Size(100, 23);
            txtGenre.TabIndex = 3;
            txtGenre.Text = "txtGenre";
            // 
            // txtISBN
            // 
            txtISBN.Location = new Point(372, 172);
            txtISBN.Name = "txtISBN";
            txtISBN.Size = new Size(100, 23);
            txtISBN.TabIndex = 4;
            txtISBN.Text = "txtISBN";
            txtISBN.TextChanged += txtISBN_TextChanged;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(275, 322);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(75, 23);
            btnRegister.TabIndex = 5;
            btnRegister.Text = "登録";
            btnRegister.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(372, 322);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "キャンセル";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(244, 99);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 7;
            label1.Text = "タイトル";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(244, 155);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 8;
            label2.Text = "著者";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(244, 216);
            label3.Name = "label3";
            label3.Size = new Size(43, 15);
            label3.TabIndex = 9;
            label3.Text = "出版社";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(359, 99);
            label4.Name = "label4";
            label4.Size = new Size(44, 15);
            label4.TabIndex = 10;
            label4.Text = "ジャンル";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(359, 155);
            label5.Name = "label5";
            label5.Size = new Size(32, 15);
            label5.TabIndex = 11;
            label5.Text = "ISBN";
            // 
            // BookRegisterForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnRegister);
            Controls.Add(txtISBN);
            Controls.Add(txtGenre);
            Controls.Add(txtPublisher);
            Controls.Add(txtAuthor);
            Controls.Add(txtBookName);
            Name = "BookRegisterForm";
            Text = "BookRegisterForm";
            Load += BookRegisterForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private void txtISBN_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void txtPublisher_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void txtAuthor_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private TextBox txtBookName;
        private TextBox txtAuthor;
        private TextBox txtPublisher;
        private TextBox txtGenre;
        private TextBox txtISBN;
        private Button btnRegister;
        private Button btnCancel;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}