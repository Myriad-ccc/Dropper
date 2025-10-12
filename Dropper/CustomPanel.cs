using System.Windows.Forms;

namespace Dropper
{
    public class CustomPanel : Panel
    {
        public CustomPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
    }
}
