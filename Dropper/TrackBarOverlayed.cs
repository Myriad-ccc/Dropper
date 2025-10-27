using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class TrackBarOverlayed : TrackBar
    {
        //Constant for windows "paint" message
        private const int WM_PAINT = 0x0F;
        public Color RailCover { get; set; } = Color.RoyalBlue;

        public TrackBarOverlayed()
        {
            DoubleBuffered = true;
            TabStop = false;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT)
            {
                using (var g = CreateGraphics())
                {
                    var cover = new Rectangle(new Point(8, 8), new Size(ClientSize.Width - 16, 4));
                    using (var brush = new SolidBrush(RailCover))
                        g.FillRectangle(brush, cover);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            if (e.KeyCode == Keys.Left && Value != Minimum) Value--;
            if (e.KeyCode == Keys.Right && Value != Maximum) Value++;
        }

        protected override bool ShowFocusCues => false;
        protected override bool ShowKeyboardCues => false;

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Parent?.Focus();
        }
    }
}
