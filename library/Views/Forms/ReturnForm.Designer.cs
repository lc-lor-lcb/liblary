namespace library.View
{
    partial class ReturnForm
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
            txtUserId = new TextBox();
            txtBookId = new TextBox();
            btnReturn = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // txtUserId
            // 
            txtUserId.Location = new Point(279, 163);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(100, 23);
            txtUserId.TabIndex = 0;
            txtUserId.Text = "txtUserId";
            // 
            // txtBookId
            // 
            txtBookId.Location = new Point(421, 163);
            txtBookId.Name = "txtBookId";
            txtBookId.Size = new Size(100, 23);
            txtBookId.TabIndex = 1;
            txtBookId.Text = "txtBookId";
            // 
            // btnReturn
            // 
            btnReturn.Location = new Point(362, 236);
            btnReturn.Name = "btnReturn";
            btnReturn.Size = new Size(75, 23);
            btnReturn.TabIndex = 2;
            btnReturn.Text = "返却";
            btnReturn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(265, 145);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 3;
            label1.Text = "ユーザーID";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(407, 145);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 4;
            label2.Text = "蔵書ID";
            // 
            // ReturnForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnReturn);
            Controls.Add(txtBookId);
            Controls.Add(txtUserId);
            Name = "ReturnForm";
            Text = "ReturnForm";
            Load += ReturnForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUserId;
        private TextBox txtBookId;
        private Button btnReturn;
        private Label label1;
        private Label label2;
    }
}