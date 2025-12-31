using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace BudgetLedger.Contacts
{
    public partial class contacts : Form
    {
        private string pdfFilePath = string.Empty;

        public contacts()
        {
            InitializeComponent();

            // A4 size in hundredths of an inch (1 inch = 100 units)
            Size a4Size = new Size(827, 1169); // 8.27 inches x 11.69 inches

            // Enable drag-and-drop for the form
            this.AllowDrop = true;
            this.DragEnter += contacts_DragEnter;
            this.DragDrop += contacts_DragDrop;

            // Disable external drop for WebView2 to prevent default behavior
            contacts_webView21_pdfDisplay.AllowExternalDrop = false;

            // Initialize WebView2
            InitializeWebView2();
        }

        private async void InitializeWebView2()
        {
            try
            {
                await contacts_webView21_pdfDisplay.EnsureCoreWebView2Async();
                LoadLastPdf();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize PDF viewer: {ex.Message}", "BudgetLedger", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void contacts_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0 && Path.GetExtension(files[0]).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private async void contacts_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                string file = files[0];
                if (Path.GetExtension(file).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    pdfFilePath = file;
                    await DisplayPdfAsync(pdfFilePath);
                    SavePdfPath(pdfFilePath);
                }
            }
        }

        private async System.Threading.Tasks.Task DisplayPdfAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            try
            {
                if (contacts_webView21_pdfDisplay.CoreWebView2 == null)
                    await contacts_webView21_pdfDisplay.EnsureCoreWebView2Async();

                // Convert the PDF file to a base64 string and embed it in an HTML object tag
                byte[] pdfBytes = File.ReadAllBytes(filePath);
                string base64 = Convert.ToBase64String(pdfBytes);

                var html = $@"<!doctype html>
<html>
<head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1' />
    <style>
        html, body {{ margin: 0; padding: 0; height: 100%; width: 100%; overflow: hidden; }}
        object {{ width: 100%; height: 100%; }}
    </style>
</head>
<body>
    <object data='data:application/pdf;base64,{base64}' type='application/pdf' width='100%' height='100%'>
        <p>Your browser does not support PDFs. <a href='data:application/pdf;base64,{base64}'>Download the PDF</a>.</p>
    </object>
</body>
</html>";

                contacts_webView21_pdfDisplay.NavigateToString(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to display PDF: {ex.Message}", "BudgetLedger", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SavePdfPath(string filePath)
        {
            string persistenceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ContactPDFPath.txt");
            File.WriteAllText(persistenceFile, filePath);
        }

        private async void LoadLastPdf()
        {
            string persistenceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ContactPDFPath.txt");
            if (File.Exists(persistenceFile))
            {
                pdfFilePath = File.ReadAllText(persistenceFile).Trim();
                if (File.Exists(pdfFilePath))
                {
                    await DisplayPdfAsync(pdfFilePath);
                }
            }
        }
    }
}
