namespace library.View
{
    partial class MainForm
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
            lblLibrarianName = new Label();
            btnBookList = new Button();
            btnBookRegister = new Button();
            btnUserManage = new Button();
            btnLogout = new Button();
            SuspendLayout();
            // 
            // lblLibrarianName
            // 
            lblLibrarianName.AutoSize = true;
            lblLibrarianName.Location = new Point(12, 9);
            lblLibrarianName.Name = "lblLibrarianName";
            lblLibrarianName.Size = new Size(0, 15);
            lblLibrarianName.TabIndex = 0;
            // 
            // btnBookList
            // 
            btnBookList.Location = new Point(15, 28);
            btnBookList.Name = "btnBookList";
            btnBookList.Size = new Size(131, 42);
            btnBookList.TabIndex = 1;
            btnBookList.Text = "蔵書一覧";
            btnBookList.UseVisualStyleBackColor = true;
            // 
            // btnBookRegister
            // 
            btnBookRegister.Location = new Point(177, 28);
            btnBookRegister.Name = "btnBookRegister";
            btnBookRegister.Size = new Size(131, 42);
            btnBookRegister.TabIndex = 2;
            btnBookRegister.Text = "蔵書登録";
            btnBookRegister.UseVisualStyleBackColor = true;
            // 
            // btnUserManage
            // 
            btnUserManage.Location = new Point(337, 28);
            btnUserManage.Name = "btnUserManage";
            btnUserManage.Size = new Size(131, 42);
            btnUserManage.TabIndex = 3;
            btnUserManage.Text = "利用者管理";
            btnUserManage.UseVisualStyleBackColor = true;
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(500, 28);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(131, 42);
            btnLogout.TabIndex = 4;
            btnLogout.Text = "ログアウト";
            btnLogout.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnLogout);
            Controls.Add(btnUserManage);
            Controls.Add(btnBookRegister);
            Controls.Add(btnBookList);
            Controls.Add(lblLibrarianName);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLibrarianName;
        private Button btnBookList;
        private Button btnBookRegister;
        private Button btnUserManage;
        private Button btnLogout;
    }
}