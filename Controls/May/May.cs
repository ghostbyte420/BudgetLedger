using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BudgetLedger.Controls.May
{
    public partial class May : BaseMonth
    {
        public May()
        {
            this.SuspendLayout();
            InitializeComponent();
            monthOpacityPanel.EnableDoubleBuffering();
            monthTabControl.EnableDoubleBuffering();
            SetupHeaderTab();
            this.ResumeLayout();
        }

        private void SetupHeaderTab()
        {
            TabPage headerTab = null;
            foreach (TabPage tab in monthTabControl.TabPages)
            {
                if (tab.Text == "Header")
                {
                    headerTab = tab;
                    break;
                }
            }

            if (headerTab != null)
            {
                Panel headerPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackgroundImage = Properties.Resources.bkgr_may,
                    BackgroundImageLayout = ImageLayout.Stretch
                };

                // Enable double buffering for the panel
                headerPanel.EnableDoubleBuffering();

                PictureBox foregroundPictureBox = new PictureBox
                {
                    Image = Properties.Resources.head_may,
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    BackColor = Color.Transparent
                };

                // Enable double buffering for the PictureBox
                foregroundPictureBox.EnableDoubleBuffering();

                headerPanel.SizeChanged += (sender, e) =>
                {
                    foregroundPictureBox.Location = new Point(
                        (headerPanel.Width - foregroundPictureBox.Width) / 2,
                        (headerPanel.Height - foregroundPictureBox.Height) / 2
                    );
                };

                headerPanel.Controls.Add(foregroundPictureBox);
                headerTab.Controls.Add(headerPanel);
            }
            else
            {
                MessageBox.Show("Header tab not found in May!");
            }
        }
    }
}
