namespace library.View
{
    partial class UserManageForm
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
            txtSearchName = new TextBox();
            txtName = new TextBox();
            txtBirth = new TextBox();
            txtMail = new TextBox();
            txtPhone = new TextBox();
            txtAddress = new TextBox();
            btnSearch = new Button();
            btnUpdate = new Button();
            dgvUsers = new DataGridView();
            rbMale = new RadioButton();
            rbFemale = new RadioButton();
            btnRegister = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
            SuspendLayout();
            // 
            // txtSearchName
            // 
            txtSearchName.Location = new Point(56, 46);
            txtSearchName.Name = "txtSearchName";
            txtSearchName.Size = new Size(100, 23);
            txtSearchName.TabIndex = 0;
            // 
            // txtName
            // 
            txtName.Location = new Point(56, 108);
            txtName.Name = "txtName";
            txtName.Size = new Size(100, 23);
            txtName.TabIndex = 1;
            // 
            // txtBirth
            // 
            txtBirth.Location = new Point(56, 172);
            txtBirth.Name = "txtBirth";
            txtBirth.Size = new Size(100, 23);
            txtBirth.TabIndex = 2;
            // 
            // txtMail
            // 
            txtMail.Location = new Point(225, 46);
            txtMail.Name = "txtMail";
            txtMail.Size = new Size(100, 23);
            txtMail.TabIndex = 3;
            // 
            // txtPhone
            // 
            txtPhone.Location = new Point(225, 108);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(100, 23);
            txtPhone.TabIndex = 4;
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(225, 172);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(100, 23);
            txtAddress.TabIndex = 5;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(440, 90);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(75, 23);
            btnSearch.TabIndex = 6;
            btnSearch.Text = "検索";
            btnSearch.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(440, 170);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 23);
            btnUpdate.TabIndex = 7;
            btnUpdate.Text = "更新";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // dgvUsers
            // 
            dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsers.Location = new Point(12, 230);
            dgvUsers.Name = "dgvUsers";
            dgvUsers.Size = new Size(776, 208);
            dgvUsers.TabIndex = 8;
            // 
            // rbMale
            // 
            rbMale.AutoSize = true;
            rbMale.Location = new Point(380, 140);
            rbMale.Name = "rbMale";
            rbMale.Size = new Size(49, 19);
            rbMale.TabIndex = 9;
            rbMale.TabStop = true;
            rbMale.Text = "男性";
            rbMale.UseVisualStyleBackColor = true;
            // 
            // rbFemale
            // 
            rbFemale.AutoSize = true;
            rbFemale.Location = new Point(380, 176);
            rbFemale.Name = "rbFemale";
            rbFemale.Size = new Size(49, 19);
            rbFemale.TabIndex = 10;
            rbFemale.TabStop = true;
            rbFemale.Text = "女性";
            rbFemale.UseVisualStyleBackColor = true;
            // 
            // btnRegister
            // 
            btnRegister.Location = new Point(440, 131);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(75, 23);
            btnRegister.TabIndex = 11;
            btnRegister.Text = "登録";
            btnRegister.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(36, 28);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 12;
            label1.Text = "検索名";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(36, 90);
            label2.Name = "label2";
            label2.Size = new Size(31, 15);
            label2.TabIndex = 13;
            label2.Text = "名前";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(36, 157);
            label3.Name = "label3";
            label3.Size = new Size(55, 15);
            label3.TabIndex = 14;
            label3.Text = "生年月日";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(214, 28);
            label4.Name = "label4";
            label4.Size = new Size(33, 15);
            label4.TabIndex = 15;
            label4.Text = "メール";
            label4.Click += label4_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(214, 90);
            label5.Name = "label5";
            label5.Size = new Size(55, 15);
            label5.TabIndex = 16;
            label5.Text = "電話番号";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(214, 157);
            label6.Name = "label6";
            label6.Size = new Size(42, 15);
            label6.TabIndex = 17;
            label6.Text = "アドレス";
            // 
            // UserManageForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnRegister);
            Controls.Add(rbFemale);
            Controls.Add(rbMale);
            Controls.Add(dgvUsers);
            Controls.Add(btnUpdate);
            Controls.Add(btnSearch);
            Controls.Add(txtAddress);
            Controls.Add(txtPhone);
            Controls.Add(txtMail);
            Controls.Add(txtBirth);
            Controls.Add(txtName);
            Controls.Add(txtSearchName);
            Name = "UserManageForm";
            Text = "利用者管理画面";
            Load += UserManageForm_Load_1;
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSearchName;
        private TextBox txtName;
        private TextBox txtBirth;
        private TextBox txtMail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private Button btnSearch;
        private Button btnUpdate;
        private DataGridView dgvUsers;
        private RadioButton rbMale;
        private RadioButton rbFemale;
        private Button btnRegister;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}