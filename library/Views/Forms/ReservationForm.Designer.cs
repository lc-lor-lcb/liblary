namespace library.View
{
    partial class ReservationForm
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
            lblReturnDue = new Label();
            btnReserve = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // txtUserId
            // 
            txtUserId.Location = new Point(32, 37);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(100, 23);
            txtUserId.TabIndex = 0;
            txtUserId.TextChanged += txtUserId_TextChanged;
            // 
            // txtBookId
            // 
            txtBookId.Location = new Point(177, 37);
            txtBookId.Name = "txtBookId";
            txtBookId.Size = new Size(100, 23);
            txtBookId.TabIndex = 1;
            // 
            // lblReturnDue
            // 
            lblReturnDue.AutoSize = true;
            lblReturnDue.Location = new Point(82, 83);
            lblReturnDue.Name = "lblReturnDue";
            lblReturnDue.Size = new Size(0, 15);
            lblReturnDue.TabIndex = 2;
            // 
            // btnReserve
            // 
            btnReserve.Location = new Point(116, 83);
            btnReserve.Name = "btnReserve";
            btnReserve.Size = new Size(75, 23);
            btnReserve.TabIndex = 3;
            btnReserve.Text = "予約";
            btnReserve.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 19);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 5;
            label1.Text = "利用者ID";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(162, 19);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 6;
            label2.Text = "蔵書ID";
            // 
            // ReservationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnReserve);
            Controls.Add(lblReturnDue);
            Controls.Add(txtBookId);
            Controls.Add(txtUserId);
            Name = "ReservationForm";
            Text = "予約画面";
            Load += ReservationForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUserId;
        private TextBox txtBookId;
        private Label lblReturnDue;
        private Button btnReserve;
        private Label label1;
        private Label label2;
    }
}