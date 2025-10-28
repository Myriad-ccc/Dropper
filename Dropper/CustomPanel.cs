using System.Windows.Forms;

namespace Dropper
{
    public class CustomPanel : Panel
    {
        public CustomPanel()
        {
            DoubleBuffered = true;
            TabStop = false;
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
        protected override bool ShowFocusCues => false;
    }
}
