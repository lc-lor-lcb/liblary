namespace library.View
{
    partial class CompletionForm
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
            btnBack = new Button();
            lblMessage = new Label();
            lblCountdown = new Label();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Location = new Point(61, 63);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(75, 23);
            btnBack.TabIndex = 0;
            btnBack.Text = "戻る";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(61, 29);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(67, 15);
            lblMessage.TabIndex = 2;
            lblMessage.Text = "✓操作完了";
            lblMessage.Click += lblMessage_Click;
            // 
            // lblCountdown
            // 
            lblCountdown.AutoSize = true;
            lblCountdown.Location = new Point(33, 89);
            lblCountdown.Name = "lblCountdown";
            lblCountdown.Size = new Size(137, 15);
            lblCountdown.TabIndex = 3;
            lblCountdown.Text = "※30秒後に自動で戻ります";
            lblCountdown.Click += lblCountdown_Click;
            // 
            // CompletionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblCountdown);
            Controls.Add(lblMessage);
            Controls.Add(btnBack);
            Name = "CompletionForm";
            Text = "CompletionForm";
            Load += CompletionForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBack;
        private Label lblMessage;
        private Label lblCountdown;
    }
}