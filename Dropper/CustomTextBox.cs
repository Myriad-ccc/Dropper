using System.Windows.Forms;

namespace Dropper
{
    public class CustomTextBox : TextBox
    {
        private const int WM_CONTEXTMENU = 0x007B;

        public CustomTextBox() => ContextMenuStrip = new ContextMenuStrip();

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CONTEXTMENU)
                return;
            base.WndProc(ref m);
        }
    }
}
