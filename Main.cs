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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace BudgetLedger
{
    public partial class budgetLedgerMain : Form
    {
        public budgetLedgerMain()
        {
            InitializeComponent();
            
            LedgerServices.Initialize(DateTime.Now.Year);
            
            InitializeMonthButtons();
            budgetLedgerMain_menuStrip.Visible = false; // Hide the MenuStrip on the main form
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

                Image normalImage = Properties.Resources.ResourceManager.GetObject($"bkdr_{month}") as Image;
                Image hoverImage = Properties.Resources.ResourceManager.GetObject($"bkdr_{month}_h") as Image;
                Image clickImage = Properties.Resources.ResourceManager.GetObject($"bkdr_{month}_c") as Image;

                SetupMonthButton(button, normalImage, hoverImage, clickImage);
                button.Cursor = Cursors.Hand;
                button.Click += (sender, e) => LoadMonthUserControl(month);
            }
        }

        private void SetupMonthButton(PictureBox button, Image normalImage, Image hoverImage, Image clickImage)
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
            // Map the abbreviated month name to the full month name
            string monthName = GetFullMonthName(month);

            // Set the form title to "<Month Name> Expense Report"
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
        }

        // Helper method to get the full month name
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

        }

        private void budgetLedgerMain_menuStrip_button_saveDatabase_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Saved automatically. Database location: %LocalAppData%\\BudgetLedger\\budgetledger_<year>.sqlite",
                "BudgetLedger",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
