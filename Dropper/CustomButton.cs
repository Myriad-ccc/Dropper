using System.Windows.Forms;

namespace Dropper
{
    public class CustomButton : Button
    {
        public CustomButton()
        {
            DoubleBuffered = true;
            TabStop = false;
        }
        protected override bool ShowFocusCues => false;
        protected override bool ShowKeyboardCues => false;
    }
}
