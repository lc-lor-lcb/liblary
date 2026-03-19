namespace library.View
{
    partial class CheckoutForm
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
            btnCheckout = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // txtUserId
            // 
            txtUserId.Location = new Point(36, 42);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(100, 23);
            txtUserId.TabIndex = 0;
            // 
            // txtBookId
            // 
            txtBookId.Location = new Point(165, 42);
            txtBookId.Name = "txtBookId";
            txtBookId.Size = new Size(100, 23);
            txtBookId.TabIndex = 1;
            // 
            // btnCheckout
            // 
            btnCheckout.Location = new Point(110, 108);
            btnCheckout.Name = "btnCheckout";
            btnCheckout.Size = new Size(75, 23);
            btnCheckout.TabIndex = 2;
            btnCheckout.Text = "貸出";
            btnCheckout.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 24);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 3;
            label1.Text = "利用者ID";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(153, 24);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 4;
            label2.Text = "蔵書ID";
            // 
            // CheckoutForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnCheckout);
            Controls.Add(txtBookId);
            Controls.Add(txtUserId);
            HelpButton = true;
            Name = "CheckoutForm";
            Text = "貸出画面";
            Load += CheckoutForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUserId;
        private TextBox txtBookId;
        private Button btnCheckout;
        private Label label1;
        private Label label2;
    }
}