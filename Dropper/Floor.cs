using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class Floor : CustomPanel
    {
        private const string Title = "Dropper";
        private readonly float TitleFontSize = 26f;
        private Font TitleFont => new Font(QOL.VCROSDMONO, TitleFontSize);
        private Rectangle TitleRect => new Rectangle(new Point(5, 0), TextRenderer.MeasureText(Title, new Font(QOL.VCROSDMONO, TitleFontSize + 2f)));
        public Color TitleColor { get; set; } = QOL.RandomColor();
        public Color ShadowTitleColor { get; set; } = QOL.RandomColor();
        public Color BorderColor => TitleColor;

        public Floor()
        {
            BackColor = QOL.RGB(40);

            Paint += (s, ev) =>
            {
                Graphics g = ev.Graphics;

                using (var mainBrush = new SolidBrush(ShadowTitleColor))
                    g.DrawString(Title, TitleFont, mainBrush, TitleRect);
                using (var shadowBrush = new SolidBrush(TitleColor))
                    g.DrawString(Title, TitleFont, shadowBrush, new Rectangle(new Point(TitleRect.X - 1, TitleRect.Y - 1), TitleRect.Size));
            };

            MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Right)
                {
                    TitleColor = QOL.RandomColor();
                    ShadowTitleColor = QOL.RandomColor();
                    Invalidate();
                }
            };
        }
    }
}
