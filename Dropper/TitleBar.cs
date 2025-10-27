using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class TitleBar : CustomPanel
    {
        public Color TitleColor { get; set; } = QOL.RandomColor();
        public Color ShadowTitleColor { get; set; } = QOL.RandomColor();

        private bool built = false;

        public TitleBar()
        {
            BackColor = QOL.RGB(35);
            Drag();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (Width == 0 || Height == 0) return;

            if (built) return;
            else built = true;

            var closingButton = new CustomButton()
            {
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(64, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = QOL.RGB(20),
                ForeColor = Color.FromArgb(255, 163, 42, 42),
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "✖",
            };
            closingButton.Location = new Point(ClientSize.Width - closingButton.Width);
            closingButton.MouseClick += (s, ev) => FindForm()?.Close();
            Controls.Add(closingButton);

            var minimizeButton = new CustomButton()
            {
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = closingButton.Size,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = QOL.RGB(20),
                ForeColor = Color.FromArgb(255, 42, 163, 150),
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "―",
            };
            QOL.Align.Left(minimizeButton, closingButton, 4);
            minimizeButton.MouseClick += (s, ev) => FindForm().WindowState = FormWindowState.Minimized;
            Controls.Add(minimizeButton);

            Paint += (s, ev) =>
            {
                Graphics g = ev.Graphics;

                using (var shadowBrush = new SolidBrush(TitleColor))
                using (var mainBrush = new SolidBrush(ShadowTitleColor))
                using (var font = new Font(QOL.VCROSDMONO, 32f))
                {
                    g.DrawString(FindForm()?.Text, font, shadowBrush, new Point(10, 10));
                    g.DrawString(FindForm()?.Text, font, mainBrush, new Point(9, 9));
                }
            };

        }

        public void Drag()
        {
            bool MouseDragging = false;
            Point cursorPos = Cursor.Position;

            MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    MouseDragging = true;
                    cursorPos = Cursor.Position;
                }
                if (ev.Button == MouseButtons.Right && !MouseDragging)
                {
                    TitleColor = QOL.RandomColor();
                    ShadowTitleColor = QOL.RandomColor();
                    Invalidate();
                }
            };

            MouseUp += (s, ev) => MouseDragging = false;

            MouseMove += (s, ev) =>
            {
                if (MouseDragging)
                {
                    int deltaX = Cursor.Position.X - cursorPos.X;
                    int deltaY = Cursor.Position.Y - cursorPos.Y;

                    FindForm().Left += deltaX;
                    FindForm().Top += deltaY;

                    cursorPos = Cursor.Position;

                    Invalidate();
                }
            };
        }
    }
}
