namespace BudgetLedger.Contacts
{
    partial class contacts
    {
        private System.ComponentModel.IContainer components = null;
        private Microsoft.Web.WebView2.WinForms.WebView2 contacts_webView21_pdfDisplay;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(contacts));
            contacts_webView21_pdfDisplay = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)contacts_webView21_pdfDisplay).BeginInit();
            SuspendLayout();
            // 
            // contacts_webView21_pdfDisplay
            // 
            contacts_webView21_pdfDisplay.AllowExternalDrop = false;
            contacts_webView21_pdfDisplay.CreationProperties = null;
            contacts_webView21_pdfDisplay.DefaultBackgroundColor = Color.White;
            contacts_webView21_pdfDisplay.Dock = DockStyle.Fill;
            contacts_webView21_pdfDisplay.Location = new Point(0, 0);
            contacts_webView21_pdfDisplay.Name = "contacts_webView21_pdfDisplay";
            contacts_webView21_pdfDisplay.Size = new Size(778, 1019);
            contacts_webView21_pdfDisplay.TabIndex = 0;
            contacts_webView21_pdfDisplay.ZoomFactor = 1D;
            // 
            // contacts
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(778, 1019);
            Controls.Add(contacts_webView21_pdfDisplay);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "contacts";
            Opacity = 0.95D;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Budget Ledger";
            ((System.ComponentModel.ISupportInitialize)contacts_webView21_pdfDisplay).EndInit();
            ResumeLayout(false);
        }
    }
}
