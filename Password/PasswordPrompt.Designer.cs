namespace BudgetLedger.Password
{
    partial class passwordPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(passwordPrompt));
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword = new Label();
            passwordPrompt_opacityPanel = new OpacityPanel();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_status = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit = new Button();
            passwordPrompt_opacityPanel_darkTextBox_txtPassword = new DarkTextBox();
            passwordPrompt_opacityPanel.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword
            // 
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword.AutoSize = true;
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword.Font = new Font("Segoe UI", 11F);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword.Location = new Point(8, 14);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword.Name = "passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword";
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword.Size = new Size(111, 20);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword.TabIndex = 0;
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword.Text = "Enter Password:";
            // 
            // passwordPrompt_opacityPanel
            // 
            passwordPrompt_opacityPanel.BackColor = Color.Transparent;
            passwordPrompt_opacityPanel.Controls.Add(statusStrip1);
            passwordPrompt_opacityPanel.Controls.Add(passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit);
            passwordPrompt_opacityPanel.Controls.Add(passwordPrompt_opacityPanel_darkTextBox_txtPassword);
            passwordPrompt_opacityPanel.Controls.Add(passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword);
            passwordPrompt_opacityPanel.Dock = DockStyle.Fill;
            passwordPrompt_opacityPanel.Location = new Point(0, 0);
            passwordPrompt_opacityPanel.Name = "passwordPrompt_opacityPanel";
            passwordPrompt_opacityPanel.Size = new Size(383, 145);
            passwordPrompt_opacityPanel.TabIndex = 1;
            // 
            // statusStrip1
            // 
            statusStrip1.Font = new Font("Segoe UI", 10F);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, passwordPrompt_opacityPanel_darkTextBox_txtPassword_status, toolStripStatusLabel3 });
            statusStrip1.Location = new Point(0, 121);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(383, 24);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(177, 19);
            toolStripStatusLabel1.Spring = true;
            // 
            // passwordPrompt_opacityPanel_darkTextBox_txtPassword_status
            // 
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_status.Name = "passwordPrompt_opacityPanel_darkTextBox_txtPassword_status";
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_status.Size = new Size(13, 19);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_status.Text = " ";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(177, 19);
            toolStripStatusLabel3.Spring = true;
            // 
            // passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit
            // 
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit.Font = new Font("Segoe UI", 10F);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit.Location = new Point(264, 76);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit.Name = "passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit";
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit.Size = new Size(106, 27);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit.TabIndex = 2;
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit.Text = "Submit";
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit.UseVisualStyleBackColor = true;
            passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit.Click += passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit_Click;
            // 
            // passwordPrompt_opacityPanel_darkTextBox_txtPassword
            // 
            passwordPrompt_opacityPanel_darkTextBox_txtPassword.BorderStyle = BorderStyle.FixedSingle;
            passwordPrompt_opacityPanel_darkTextBox_txtPassword.Font = new Font("Segoe UI", 11F);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword.Location = new Point(13, 39);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword.Name = "passwordPrompt_opacityPanel_darkTextBox_txtPassword";
            passwordPrompt_opacityPanel_darkTextBox_txtPassword.Size = new Size(357, 27);
            passwordPrompt_opacityPanel_darkTextBox_txtPassword.TabIndex = 1;
            passwordPrompt_opacityPanel_darkTextBox_txtPassword.UseSystemPasswordChar = true;
            // 
            // passwordPrompt
            // 
            AcceptButton = passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(383, 145);
            Controls.Add(passwordPrompt_opacityPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "passwordPrompt";
            Opacity = 0.95D;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Budget Ledger";
            TopMost = true;
            passwordPrompt_opacityPanel.ResumeLayout(false);
            passwordPrompt_opacityPanel.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword;
        private OpacityPanel passwordPrompt_opacityPanel;
        private DarkTextBox passwordPrompt_opacityPanel_darkTextBox_txtPassword;
        private Button passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel passwordPrompt_opacityPanel_darkTextBox_txtPassword_status;
        private ToolStripStatusLabel toolStripStatusLabel3;
    }
}