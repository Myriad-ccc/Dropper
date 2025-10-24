using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class TitleBar : CustomPanel
    {
        public bool PaintTitle { get; set; } = true;
        public Color TitleColor { get; set; } = QOL.RandomColor();
        public bool PaintShadow { get; set; } = true;
        public Color ShadowTitleColor { get; set; } = QOL.RandomColor();

        public TitleBar(Size size)
        {
            Size = size;
            BackColor = QOL.RGB(35);

            BuildTitleBar();
            Drag();
        }

        private void BuildTitleBar()
        {
            var closingButton = new Button()
            {
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(Height, Height),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = QOL.RGB(20),
                ForeColor = Color.FromArgb(255, 163, 42, 42),
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "✖",
            };
            closingButton.Location = new Point(ClientSize.Width - closingButton.Width, 0);
            closingButton.MouseClick += (s, ev) => FindForm()?.Close();
            Controls.Add(closingButton);

            var minimizeButton = new Button()
            {
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = QOL.RGB(20),
                ForeColor = Color.FromArgb(255, 42, 163, 150),
                Font = new Font(QOL.VCROSDMONO, 20f),
                Size = closingButton.Size,
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
            bool Dragging = false;
            Point cursorPos = Cursor.Position;

            MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    Dragging = true;
                    cursorPos = Cursor.Position;
                }
                if (ev.Button == MouseButtons.Right && !Dragging)
                {
                    TitleColor = QOL.RandomColor();
                    ShadowTitleColor = QOL.RandomColor();
                    Invalidate();
                }
            };

            MouseUp += (s, ev) => Dragging = false;

            MouseMove += (s, ev) =>
            {
                if (Dragging)
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
