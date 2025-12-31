using BudgetLedger.Contacts;
using BudgetLedger.Controls.April;
using BudgetLedger.Controls.August;
using BudgetLedger.Controls.December;
using BudgetLedger.Controls.February;
using BudgetLedger.Controls.January;
using BudgetLedger.Controls.July;
using BudgetLedger.Controls.June;
using BudgetLedger.Controls.March;
using BudgetLedger.Controls.May;
using BudgetLedger.Controls.November;
using BudgetLedger.Controls.October;
using BudgetLedger.Controls.September;
using BudgetLedger.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BudgetLedger
{
    public partial class budgetLedgerMain : Form
    {
        private Panel monthSummaryPanel;
        private DarkTextBox txtMonthlyStartTotal;
        private Label lblExpensesPaidOut;
        private Label lblMonthlyEndTotal;

        // Configuration options for label styling
        [Browsable(false)]
        [DefaultValue(false)]
        public bool UseBoldStyle { get; set; } = true;

        [Browsable(false)]
        [DefaultValue(false)]
        public bool UseItalicStyle { get; set; } = false;

        public budgetLedgerMain()
        {
            InitializeComponent();

            LedgerServices.Initialize(DateTime.Now.Year);

            InitializeMonthButtons();
            InitializeMonthSummaryPanel();
            budgetLedgerMain_menuStrip.Visible = false;
        }

        private void InitializeMonthButtons()
        {
            var monthButtons = new Dictionary<string, PictureBox>
            {
                {"jan", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_jan},
                {"feb", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_feb},
                {"mar", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_mar},
                {"apr", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_apr},
                {"may", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_may},
                {"jun", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_jun},
                {"jul", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_jul},
                {"aug", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_aug},
                {"sep", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_sep},
                {"oct", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_oct},
                {"nov", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_nov},
                {"dec", budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu_pictureBox_button_dec}
            };

            foreach (var kvp in monthButtons)
            {
                string month = kvp.Key;
                PictureBox button = kvp.Value;

                System.Drawing.Image normalImage = Properties.Resources.ResourceManager.GetObject($"butn_{month}") as System.Drawing.Image;
                System.Drawing.Image hoverImage = Properties.Resources.ResourceManager.GetObject($"butn_{month}_h") as System.Drawing.Image;
                System.Drawing.Image clickImage = Properties.Resources.ResourceManager.GetObject($"butn_{month}_cg") as System.Drawing.Image;

                SetupMonthButton(button, normalImage, hoverImage, clickImage);
                button.Cursor = Cursors.Hand;
                button.Click += (sender, e) => LoadMonthUserControl(month);
            }
        }

        private void InitializeMonthSummaryPanel()
        {
            monthSummaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = System.Drawing.Color.FromArgb(60, 60, 60),
                Visible = false
            };

            // Customizable offset from the right edge of the form
            int rightOffset = 20;
            // Fixed width for value fields
            int valueWidth = 120;
            // Vertical position for all controls
            int verticalPosition = 5;
            // Font size for all controls
            float fontSize = 11f;
            // Overall right margin for the entire row
            int rowRightMargin = 50;

            // Monthly Start Total Label
            var lblStartTotal = new Label
            {
                Text = "Monthly Start Total:",
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", fontSize)
            };

            // Monthly Start Total TextBox
            txtMonthlyStartTotal = new DarkTextBox
            {
                Width = valueWidth,
                TextAlign = System.Windows.Forms.HorizontalAlignment.Right,
                FontSize = fontSize,
                ForeColor = System.Drawing.Color.Green, // Always green
                Font = new System.Drawing.Font("Segoe UI", fontSize, GetFontStyle())
            };
            txtMonthlyStartTotal.TextChanged += (sender, e) =>
            {
                if (decimal.TryParse(txtMonthlyStartTotal.Text, out var startTotal))
                {
                    int month = DateTime.Now.Month;
                    UpdateMonthlyEndTotal(startTotal, month);
                }
            };

            // Expenses Paid Out Label
            var lblExpensesPaidOutTitle = new Label
            {
                Text = "Expenses Paid Out:",
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", fontSize)
            };

            // Expenses Paid Out Value Label
            lblExpensesPaidOut = new Label
            {
                ForeColor = System.Drawing.Color.Yellow, // Cautionary color
                Width = valueWidth,
                TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                AutoSize = false,
                Font = new System.Drawing.Font("Segoe UI", fontSize, GetFontStyle())
            };

            // Monthly End Total Label
            var lblEndTotalTitle = new Label
            {
                Text = "Monthly End Total:",
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", fontSize)
            };

            // Monthly End Total Value Label
            lblMonthlyEndTotal = new Label
            {
                ForeColor = System.Drawing.Color.White, // Default, will be set dynamically
                Width = valueWidth,
                TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                AutoSize = false,
                Font = new System.Drawing.Font("Segoe UI", fontSize, GetFontStyle())
            };

            // Add controls to the panel
            monthSummaryPanel.Controls.Add(lblStartTotal);
            monthSummaryPanel.Controls.Add(txtMonthlyStartTotal);
            monthSummaryPanel.Controls.Add(lblExpensesPaidOutTitle);
            monthSummaryPanel.Controls.Add(lblExpensesPaidOut);
            monthSummaryPanel.Controls.Add(lblEndTotalTitle);
            monthSummaryPanel.Controls.Add(lblMonthlyEndTotal);

            // Position the controls from the right edge with the specified offset
            monthSummaryPanel.SizeChanged += (sender, e) =>
            {
                int panelWidth = monthSummaryPanel.Width;
                int baseX = panelWidth - rowRightMargin;

                // Monthly Start Total
                txtMonthlyStartTotal.Location = new System.Drawing.Point(baseX - valueWidth, verticalPosition - 2);
                lblStartTotal.Location = new System.Drawing.Point(txtMonthlyStartTotal.Left - lblStartTotal.Width - rightOffset - 10, verticalPosition);

                // Expenses Paid Out
                lblExpensesPaidOut.Location = new System.Drawing.Point(txtMonthlyStartTotal.Left - 250 - valueWidth, verticalPosition);
                lblExpensesPaidOutTitle.Location = new System.Drawing.Point(lblExpensesPaidOut.Left - lblExpensesPaidOutTitle.Width - rightOffset - 10, verticalPosition);

                // Monthly End Total
                lblMonthlyEndTotal.Location = new System.Drawing.Point(lblExpensesPaidOut.Left - 250 - valueWidth, verticalPosition);
                lblEndTotalTitle.Location = new System.Drawing.Point(lblMonthlyEndTotal.Left - lblEndTotalTitle.Width - rightOffset - 10, verticalPosition);
            };

            budgetLedgerMain_splitDisplay.Panel2.Controls.Add(monthSummaryPanel);
        }

        // Helper method to get the font style based on configuration
        private System.Drawing.FontStyle GetFontStyle()
        {
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
            if (UseBoldStyle) style |= System.Drawing.FontStyle.Bold;
            if (UseItalicStyle) style |= System.Drawing.FontStyle.Italic;
            return style;
        }

        // Method to update label styles dynamically
        public void UpdateLabelStyles()
        {
            txtMonthlyStartTotal.Font = new System.Drawing.Font("Segoe UI", txtMonthlyStartTotal.Font.Size, GetFontStyle());
            lblExpensesPaidOut.Font = new System.Drawing.Font("Segoe UI", lblExpensesPaidOut.Font.Size, GetFontStyle());
            lblMonthlyEndTotal.Font = new System.Drawing.Font("Segoe UI", lblMonthlyEndTotal.Font.Size, GetFontStyle());
        }

        private void SetupMonthButton(PictureBox button, System.Drawing.Image normalImage, System.Drawing.Image hoverImage, System.Drawing.Image clickImage)
        {
            button.Tag = new { Normal = normalImage, Hover = hoverImage, Click = clickImage };
            button.Image = normalImage;

            button.MouseEnter += (sender, e) =>
            {
                var pb = (PictureBox)sender;
                var images = (dynamic)pb.Tag;
                pb.Image = images.Hover;
            };

            button.MouseLeave += (sender, e) =>
            {
                var pb = (PictureBox)sender;
                var images = (dynamic)pb.Tag;
                pb.Image = images.Normal;
            };

            button.MouseDown += (sender, e) =>
            {
                var pb = (PictureBox)sender;
                var images = (dynamic)pb.Tag;
                pb.Image = images.Click;
            };

            button.MouseUp += (sender, e) =>
            {
                var pb = (PictureBox)sender;
                var images = (dynamic)pb.Tag;
                if (pb.ClientRectangle.Contains(pb.PointToClient(Control.MousePosition)))
                    pb.Image = images.Hover;
                else
                    pb.Image = images.Normal;
            };
        }

        private void LoadMonthUserControl(string month)
        {
            string monthName = GetFullMonthName(month);
            this.Text = $"{monthName} Expense Report";

            budgetLedgerMain_splitDisplay_panel1_opacityPanel.Controls.Clear();

            UserControl monthControl = null;
            switch (month.ToLower())
            {
                case "jan":
                    monthControl = new January();
                    break;
                case "feb":
                    monthControl = new February();
                    break;
                case "mar":
                    monthControl = new March();
                    break;
                case "apr":
                    monthControl = new April();
                    break;
                case "may":
                    monthControl = new May();
                    break;
                case "jun":
                    monthControl = new June();
                    break;
                case "jul":
                    monthControl = new July();
                    break;
                case "aug":
                    monthControl = new August();
                    break;
                case "sep":
                    monthControl = new September();
                    break;
                case "oct":
                    monthControl = new October();
                    break;
                case "nov":
                    monthControl = new November();
                    break;
                case "dec":
                    monthControl = new December();
                    break;
                default:
                    MessageBox.Show($"No user control for {month} yet.");
                    return;
            }

            monthControl.Dock = DockStyle.Fill;
            budgetLedgerMain_splitDisplay_panel1_opacityPanel.Controls.Add(monthControl);
            budgetLedgerMain_menuStrip.Visible = true;

            // Show summary panel and hide month buttons
            monthSummaryPanel.Visible = true;
            budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu.Visible = false;

            // Set the background image for Panel2 when a month is selected
            budgetLedgerMain_splitDisplay.Panel2.BackgroundImage = Properties.Resources.img_003;
            budgetLedgerMain_splitDisplay.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            // Load saved start total for the month
            int monthNum = DateTime.ParseExact(month.Substring(0, 3), "MMM", System.Globalization.CultureInfo.CurrentCulture).Month;
            var summary = LedgerServices.MonthSummaries.GetByMonth(LedgerServices.LedgerYear, monthNum);
            if (summary.HasValue)
            {
                txtMonthlyStartTotal.Text = summary.Value.StartTotal.ToString("N2");
                UpdateMonthlyEndTotal(summary.Value.StartTotal, monthNum);
            }
            else
            {
                txtMonthlyStartTotal.Text = "0.00";
                UpdateMonthlyEndTotal(0, monthNum);
            }
        }

        private void UpdateMonthlyEndTotal(decimal startTotal, int month)
        {
            var expensesPaidOut = CalculateTotalExpensesForMonth(month);
            var endTotal = startTotal - expensesPaidOut;

            lblExpensesPaidOut.Text = $"{expensesPaidOut:C}";
            lblMonthlyEndTotal.Text = $"{endTotal:C}";

            // Set color for Monthly End Total
            lblMonthlyEndTotal.ForeColor = endTotal >= 0 ? System.Drawing.Color.Green : System.Drawing.Color.Red;

            // Save to database
            LedgerServices.MonthSummaries.Upsert(
                LedgerServices.LedgerYear,
                month,
                startTotal,
                endTotal
            );
        }

        private decimal CalculateTotalExpensesForMonth(int month)
        {
            decimal total = 0;
            string[] categories =
            {
                "Household", "Subscriptions", "State Taxes", "Vehicles", "Gas",
                "Trips", "Food", "Frivelous", "Emergency"
            };

            foreach (var category in categories)
            {
                var records = LedgerServices.Expenses.GetByScope(LedgerServices.LedgerYear, month, category);
                total += records.Sum(r => r.AmountPaid ?? 0);
            }
            return total;
        }

        public void NotifyExpenseChanged(int month)
        {
            if (decimal.TryParse(txtMonthlyStartTotal.Text, out var startTotal))
            {
                UpdateMonthlyEndTotal(startTotal, month);
            }
        }

        private string GetFullMonthName(string abbreviatedMonth)
        {
            switch (abbreviatedMonth.ToLower())
            {
                case "jan":
                    return "January";
                case "feb":
                    return "February";
                case "mar":
                    return "March";
                case "apr":
                    return "April";
                case "may":
                    return "May";
                case "jun":
                    return "June";
                case "jul":
                    return "July";
                case "aug":
                    return "August";
                case "sep":
                    return "September";
                case "oct":
                    return "October";
                case "nov":
                    return "November";
                case "dec":
                    return "December";
                default:
                    return string.Empty;
            }
        }

        private void budgetLedgerMain_menuStrip_button_calculator_Click(object sender, EventArgs e)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "calc.exe",
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open calculator: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void budgetLedgerMain_menuStrip_button_contacts_Click(object sender, EventArgs e)
        {
            // Create an instance of the contacts form
            contacts contactsForm = new contacts();

            // Show the contacts form
            contactsForm.Show();
        }

        private void budgetLedgerMain_menuStrip_button_saveDatabase_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Saved automatically. Database location: %LocalAppData%\\BudgetLedger\\budgetledger_<year>.sqlite",
                "BudgetLedger",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // Menu items for toggling label styles
        private void budgetLedgerMain_menuStrip_button_styleBold_Click(object sender, EventArgs e)
        {
            UseBoldStyle = !UseBoldStyle;
            UpdateLabelStyles();
        }

        private void budgetLedgerMain_menuStrip_button_styleItalic_Click(object sender, EventArgs e)
        {
            UseItalicStyle = !UseItalicStyle;
            UpdateLabelStyles();
        }

        private void budgetLedgerMain_menuStrip_button_back_Click(object sender, EventArgs e)
        {
            // Clear the month-specific UserControl from Panel1
            budgetLedgerMain_splitDisplay_panel1_opacityPanel.Controls.Clear();

            // Re-add the home screen panel to Panel1
            budgetLedgerMain_splitDisplay_panel1_opacityPanel.Controls.Add(budgetLedgerMain_splitDisplay_panel1_opacityPanel_homeScreen);

            // Show the month buttons panel
            budgetLedgerMain_splitDisplay_panel2_opacityPanel_menu.Visible = true;

            // Hide the summary panel
            monthSummaryPanel.Visible = false;

            // Reset the background image of Panel2
            budgetLedgerMain_splitDisplay.Panel2.BackgroundImage = null;

            // Reset the form title
            this.Text = "Expense Ledger";

            // Hide the menu strip
            budgetLedgerMain_menuStrip.Visible = false;
        }

        
        private void budgetLedgerMain_menuStrip_button_exportPDFReport_Click(object sender, EventArgs e)
        {
            // Acknowledge the QuestPDF license
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // 1. Determine the current month
            string currentMonthName = this.Text.Replace(" Expense Report", "");
            int currentMonth = DateTime.ParseExact(currentMonthName, "MMMM", System.Globalization.CultureInfo.CurrentCulture).Month;
            int currentYear = LedgerServices.LedgerYear;

            // 2. Gather data for the current month
            var categories = new string[]
            {
        "Household", "Subscriptions", "State Taxes", "Vehicles", "Gas",
        "Trips", "Food", "Frivelous", "Emergency"
            };

            // 3. Generate the PDF document
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(12));

                    // Header
                    page.Header()
                        .AlignLeft()
                        .PaddingBottom(20)
                        .ShowOnce()
                        .Text($"{currentMonthName} {currentYear}")
                        .Style(TextStyle.Default.FontSize(24).Bold());

                    // Summary Section
                    page.Content().Column(column =>
                    {
                        // Summary Border and Content
                        column.Item()
                            .Border(1)
                            .BorderColor(Colors.Black)
                            .Padding(10)
                            .Column(summaryColumn =>
                            {
                                var summary = LedgerServices.MonthSummaries.GetByMonth(currentYear, currentMonth);
                                if (summary.HasValue)
                                {
                                    summaryColumn.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("Monthly Start Total:").Style(TextStyle.Default.FontSize(12).Bold());
                                        row.RelativeItem().Text($"{summary.Value.StartTotal:C}").Style(TextStyle.Default.FontSize(12));
                                    });
                                    summaryColumn.Item().PaddingBottom(5);

                                    summaryColumn.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("Expenses Paid Out:").Style(TextStyle.Default.FontSize(12).Bold());
                                        row.RelativeItem().Text($"{CalculateTotalExpensesForMonth(currentMonth):C}").Style(TextStyle.Default.FontSize(12));
                                    });
                                    summaryColumn.Item().PaddingBottom(5);

                                    summaryColumn.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("Monthly End Total:").Style(TextStyle.Default.FontSize(12).Bold());
                                        row.RelativeItem().Text($"{summary.Value.EndTotal:C}")
                                            .Style(TextStyle.Default.FontSize(12)
                                            .FontColor(summary.Value.EndTotal >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2));
                                    });
                                }
                            });
                        column.Item().PaddingBottom(20);

                        // Categories Section
                        foreach (var category in categories)
                        {
                            var records = LedgerServices.Expenses.GetByScope(currentYear, currentMonth, category);
                            if (records.Count > 0)
                            {
                                // Category Header
                                column.Item()
                                    .PaddingBottom(10)
                                    .Text(category)
                                    .Style(TextStyle.Default.FontSize(16).Bold().FontColor(Colors.Blue.Darken1));

                                // Category Table
                                column.Item()
                                    .Border(0.5f)
                                    .BorderColor(Colors.Grey.Lighten1)
                                    .Padding(5)
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.ConstantColumn(200); // Expense
                                            columns.ConstantColumn(100); // Date
                                            columns.ConstantColumn(100); // Amount Paid
                                        });

                                        // Table headers
                                        table.Header(header =>
                                        {
                                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(5)
                                                .Text("Expense").Style(TextStyle.Default.FontSize(12).Bold());
                                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(5)
                                                .Text("Date").Style(TextStyle.Default.FontSize(12).Bold());
                                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(5)
                                                .Text("Amount Paid").Style(TextStyle.Default.FontSize(12).Bold());
                                        });

                                        // Table rows in the order they appear in the DataGridView
                                        foreach (var record in records)
                                        {
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                                .Text(record.Expense ?? "").Style(TextStyle.Default.FontSize(10));
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                                .Text(record.Date?.ToShortDateString() ?? "").Style(TextStyle.Default.FontSize(10));
                                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                                .Text(record.AmountPaid?.ToString("C") ?? "").Style(TextStyle.Default.FontSize(10));
                                        }
                                    });
                                column.Item().PaddingBottom(15);
                            }
                        }
                    });
                });
            });

            // 4. Save the PDF
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"{currentMonthName}_{currentYear}_Expense_Report.pdf"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                document.GeneratePdf(saveDialog.FileName);
                MessageBox.Show($"PDF exported successfully to: {saveDialog.FileName}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
