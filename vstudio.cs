using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BudgetLedger
{
    [ToolboxItem(true)]
    public class SplitDisplay : SplitContainer
    {
        public SplitDisplay()
        {
            Orientation = Orientation.Horizontal;
            SplitterDistance = 150;
            SplitterWidth = 6;
            BorderStyle = BorderStyle.FixedSingle;
            Dock = DockStyle.Fill;
        }
    }

    #region Dark Visual Studio Controls for Windows Forms

    [ToolboxItem(true)]
    public class OpacityPanel : Panel
    {
        private float _opacity = 0.50f;

        public OpacityPanel()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint,
                true);
            BackColor = Color.Transparent;
        }

        [Category("Appearance")]
        [Description("Sets the opacity of the panel (0.0 to 1.0)")]
        [DefaultValue(0.50f)]
        public float Opacity
        {
            get { return _opacity; }
            set
            {
                if (value < 0.0f) _opacity = 0.0f;
                else if (value > 1.0f) _opacity = 1.0f;
                else _opacity = value;
                Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (_opacity > 0.0f)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), Color.Black)))
                {
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                }
            }
        }
    }

    [ToolboxItem(true)]
    public class DarkTabControl : TabControl
    {
        private float _opacity = 0.5f;
        private int _hoveredTabIndex = -1;

        public DarkTabControl()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
            this.BackColor = Color.Transparent;
            UpdateStyles();
        }

        [Category("Appearance")]
        [Description("Sets the opacity of the tab control (0.0 to 1.0)")]
        [DefaultValue(0.50f)]
        public float Opacity
        {
            get { return _opacity; }
            set
            {
                if (value < 0.0f) _opacity = 0.0f;
                else if (value > 1.0f) _opacity = 1.0f;
                else _opacity = value;
                Invalidate();
                foreach (TabPage page in TabPages)
                {
                    page.Invalidate();
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_opacity > 0.0f)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), Color.Black)))
                {
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                }
            }
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle tabRect = GetTabRect(i);

                // For all tabs except the first one, add a 5-pixel left margin
                if (i != 0)
                {
                    tabRect.X += 5;
                    tabRect.Width -= 5;
                }

                bool isSelected = (i == SelectedIndex);
                bool isHovered = (i == _hoveredTabIndex);

                using (Brush brush = new SolidBrush(
                    Color.FromArgb(
                        (int)(_opacity * (isSelected ? 128 : 96)),
                        isSelected ? Color.DarkGray : Color.Gray)))
                {
                    e.Graphics.FillRectangle(brush, tabRect);
                }
                using (Pen pen = new Pen(Color.FromArgb((int)(_opacity * 192), Color.DimGray)))
                {
                    e.Graphics.DrawRectangle(pen, tabRect);
                }
                TextRenderer.DrawText(
                    e.Graphics,
                    TabPages[i].Text,
                    Font,
                    tabRect,
                    Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int newHoveredIndex = -1;
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle tabRect = GetTabRect(i);
                if (i != 0)
                {
                    tabRect.X += 5;
                    tabRect.Width -= 5;
                }
                if (tabRect.Contains(e.Location))
                {
                    newHoveredIndex = i;
                    break;
                }
            }
            if (_hoveredTabIndex != newHoveredIndex)
            {
                _hoveredTabIndex = newHoveredIndex;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoveredTabIndex = -1;
            Invalidate();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control is TabPage page)
            {
                page.BackColor = Color.Transparent;
                page.Paint += TabPage_Paint;
                typeof(TabPage).InvokeMember(
                    "DoubleBuffered",
                    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    page,
                    new object[] { true });
            }
        }

        private void TabPage_Paint(object sender, PaintEventArgs e)
        {
            if (sender is TabPage page && page.Parent is DarkTabControl parentControl)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb((int)(parentControl.Opacity * 255), Color.Black)))
                {
                    e.Graphics.FillRectangle(brush, page.ClientRectangle);
                }
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Invalidate();
            if (SelectedIndex >= 0 && SelectedIndex < TabPages.Count)
            {
                TabPages[SelectedIndex].Invalidate();
            }
        }
    }

    [ToolboxBitmap(typeof(ComboBox))]
    public class DarkComboBox : ComboBox
    {
        private float _opacity = 1.0f;
        private Color _borderColor = Color.DimGray;

        public DarkComboBox()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true
            );
            DrawMode = DrawMode.OwnerDrawFixed;
            BackColor = Color.Black;
            ForeColor = Color.White;
            DropDownStyle = ComboBoxStyle.DropDownList;
            FlatStyle = FlatStyle.Flat;
            ItemHeight = 20;
        }

        [DefaultValue(1.0f)]
        public float Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = Math.Max(0f, Math.Min(1f, value));
                Invalidate();
            }
        }

        [DefaultValue(typeof(Color), "DimGray")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                base.OnPaint(e);
                return;
            }

            using (var brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), BackColor)))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            using (var pen = new Pen(Color.FromArgb((int)(_opacity * 255), BorderColor)))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }

            using (var brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), ForeColor)))
            {
                e.Graphics.DrawString(Text, Font, brush, new PointF(2, 2));
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int arrowX = ClientRectangle.Right - 15;
            int arrowY = ClientRectangle.Top + (ClientRectangle.Height / 2);
            Point[] arrowPoints = new Point[]
            {
                new Point(arrowX, arrowY - 4),
                new Point(arrowX + 7, arrowY - 4),
                new Point(arrowX + 3, arrowY + 1)
            };
            using (var arrowBrush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), ForeColor)))
            {
                e.Graphics.FillPolygon(arrowBrush, arrowPoints);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            Color backColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? Color.FromArgb(60, 60, 60)
                : Color.Black;
            Color textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? Color.White
                : Color.LightGray;

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            using (var textBrush = new SolidBrush(textColor))
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString(
                    Items[e.Index].ToString(),
                    Font,
                    textBrush,
                    new PointF(e.Bounds.X + 2, e.Bounds.Y + 2)
                );
            }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                using (var pen = new Pen(Color.DarkGray))
                {
                    e.Graphics.DrawRectangle(pen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }
    }

    [ToolboxBitmap(typeof(NumericUpDown))]
    public class DarkNumericUpDown : NumericUpDown
    {
        private float _opacity = 1.0f;

        public DarkNumericUpDown()
        {
            BackColor = Color.Black;
            ForeColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            this.Controls[0].Visible = false;
        }

        [DefaultValue(1.0f)]
        public float Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = Math.Max(0f, Math.Min(1f, value));
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), this.BackColor)))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }

            using (var pen = new Pen(Color.FromArgb((int)(_opacity * 255), Color.DarkGray)))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }

            using (var brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), this.ForeColor)))
            {
                e.Graphics.DrawString(this.Text, this.Font, brush, new PointF(2, 2));
            }

            this.DrawUpDownButtons(e.Graphics);
        }

        private void DrawUpDownButtons(Graphics g)
        {
            int buttonWidth = 16;
            int buttonHeight = (this.Height - 2) / 2;

            Rectangle upButtonRect = new Rectangle(this.Width - buttonWidth - 1, 1, buttonWidth, buttonHeight);
            using (var brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), Color.DimGray)))
            {
                g.FillRectangle(brush, upButtonRect);
            }
            using (var pen = new Pen(Color.FromArgb((int)(_opacity * 255), Color.Gray)))
            {
                g.DrawRectangle(pen, upButtonRect);
            }

            Point[] upArrowPoints = new Point[]
            {
                new Point(this.Width - 12, 10),
                new Point(this.Width - 8, 6),
                new Point(this.Width - 4, 10)
            };
            using (var brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), Color.Black)))
            {
                g.FillPolygon(brush, upArrowPoints);
            }

            Rectangle downButtonRect = new Rectangle(this.Width - buttonWidth - 1, buttonHeight + 1, buttonWidth, buttonHeight);
            using (var brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), Color.DimGray)))
            {
                g.FillRectangle(brush, downButtonRect);
            }
            using (var pen = new Pen(Color.FromArgb((int)(_opacity * 255), Color.Gray)))
            {
                g.DrawRectangle(pen, downButtonRect);
            }

            Point[] downArrowPoints = new Point[]
            {
                new Point(this.Width - 12, buttonHeight + 6),
                new Point(this.Width - 8, buttonHeight + 10),
                new Point(this.Width - 4, buttonHeight + 6)
            };
            using (var brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), Color.Black)))
            {
                g.FillPolygon(brush, downArrowPoints);
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0014)
            {
                m.Result = nint.Zero;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int buttonWidth = 16;
            int buttonHeight = (this.Height - 2) / 2;
            Rectangle upButtonRect = new Rectangle(this.Width - buttonWidth - 1, 1, buttonWidth, buttonHeight);
            Rectangle downButtonRect = new Rectangle(this.Width - buttonWidth - 1, buttonHeight + 1, buttonWidth, buttonHeight);

            if (upButtonRect.Contains(e.Location))
            {
                this.UpButton();
            }
            else if (downButtonRect.Contains(e.Location))
            {
                this.DownButton();
            }
        }
    }

    [ToolboxBitmap(typeof(CheckBox))]
    public class DarkCheckBox : CheckBox
    {
        private float _opacity = 1.0f;
        private int _boxSize = 17;
        private float _textSize = 11f;

        [DefaultValue(1.0f)]
        public float Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = Math.Max(0f, Math.Min(1f, value));
                Invalidate();
            }
        }

        [DefaultValue(17)]
        public int BoxSize
        {
            get { return _boxSize; }
            set
            {
                _boxSize = Math.Max(8, value);
                Invalidate();
            }
        }

        [DefaultValue(11f)]
        public float TextSize
        {
            get { return _textSize; }
            set
            {
                _textSize = Math.Max(6f, value);
                Font = new Font(Font.FontFamily, _textSize);
                Invalidate();
            }
        }

        public DarkCheckBox()
        {
            BackColor = Color.Black;
            ForeColor = Color.White;
            FlatStyle = FlatStyle.Flat;
            AutoSize = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Padding = new Padding(3, 0, 0, 0);
            Font = new Font(Font.FontFamily, _textSize);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var brush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), BackColor)))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            TextRenderer.DrawText(e.Graphics, Text, Font,
                new Rectangle(_boxSize + 5, 0, Width - (_boxSize + 5), Height),
                Color.FromArgb((int)(_opacity * 255), ForeColor),
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            Rectangle boxRect = new Rectangle(1, (Height - _boxSize) / 2, _boxSize, _boxSize);
            using (var boxPen = new Pen(Color.FromArgb((int)(_opacity * 255), Color.DarkGray), 1.5f))
            {
                e.Graphics.DrawRectangle(boxPen, boxRect);
            }

            if (Checked)
            {
                using (var xBrush = new SolidBrush(Color.FromArgb((int)(_opacity * 255), Color.White)))
                {
                    int offset = _boxSize / 4;
                    e.Graphics.DrawLine(new Pen(xBrush, 1.5f),
                        boxRect.X + offset, boxRect.Y + offset,
                        boxRect.X + _boxSize - offset, boxRect.Y + _boxSize - offset);
                    e.Graphics.DrawLine(new Pen(xBrush, 1.5f),
                        boxRect.X + _boxSize - offset, boxRect.Y + offset,
                        boxRect.X + offset, boxRect.Y + _boxSize - offset);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!DesignMode)
                return;
            base.OnPaintBackground(e);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }
    }

    [ToolboxBitmap(typeof(TextBox))]
    public class DarkTextBox : TextBox
    {
        private float _fontSize = 11f;
        private Color _backColor = Color.FromArgb(60, 60, 60);
        private Color _foreColor = Color.White;
        private Color _borderColor = Color.FromArgb(80, 80, 80);
        private Color _disabledBackColor = Color.FromArgb(45, 45, 48);
        private Color _disabledForeColor = Color.FromArgb(150, 150, 150);

        public DarkTextBox()
        {
            this.BackColor = _backColor;
            this.ForeColor = _foreColor;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Font = new Font(this.Font.FontFamily, _fontSize);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        [DefaultValue(11f)]
        [Category("Appearance")]
        [Description("Sets the font size for the text box")]
        public float FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = Math.Max(6f, value);
                    this.Font = new Font(this.Font.FontFamily, _fontSize);
                }
            }
        }

        [Category("Appearance")]
        [Description("The background color of the text box")]
        [DefaultValue(typeof(Color), "60, 60, 60")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [Category("Appearance")]
        [Description("The foreground color of the text box")]
        [DefaultValue(typeof(Color), "White")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        [Category("Appearance")]
        [Description("The border color of the text box")]
        [DefaultValue(typeof(Color), "80, 80, 80")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(_borderColor))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.BackColor = this.Enabled ? _backColor : _disabledBackColor;
            this.ForeColor = this.Enabled ? _foreColor : _disabledForeColor;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _borderColor = Color.FromArgb(0, 120, 215);
            this.Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _borderColor = Color.FromArgb(80, 80, 80);
            this.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0014)
            {
                m.Result = IntPtr.Zero;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }
    }

    #endregion

    public static class ControlHelper
    {
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_COMPOSITED = 0x02000000;

        public static void EnableDoubleBuffering(Control control)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                control,
                new object[] { true });

            if (control.IsHandleCreated)
            {
                SetWindowLong(control.Handle, GWL_EXSTYLE, GetWindowLong(control.Handle, GWL_EXSTYLE) | WS_EX_COMPOSITED);
            }
            else
            {
                control.HandleCreated += (sender, e) =>
                {
                    SetWindowLong(((Control)sender).Handle, GWL_EXSTYLE, GetWindowLong(((Control)sender).Handle, GWL_EXSTYLE) | WS_EX_COMPOSITED);
                };
            }

            foreach (Control child in control.Controls)
            {
                EnableDoubleBuffering(child);
            }
        }
    }
}
