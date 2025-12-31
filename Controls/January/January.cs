using System;
using System.Drawing;
using System.Windows.Forms;

namespace BudgetLedger.Controls.January
{
    public partial class January : BaseMonth
    {
        public January()
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
                    BackgroundImage = Properties.Resources.mnth_jan,
                    BackgroundImageLayout = ImageLayout.Stretch
                };

                // Enable double buffering for the panel
                headerPanel.EnableDoubleBuffering();

                PictureBox foregroundPictureBox = new PictureBox
                {
                    Image = Properties.Resources.hedr_jan,
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
                MessageBox.Show("Header tab not found in January!");
            }
        }
    }
}