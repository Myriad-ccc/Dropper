using System.Windows.Forms;

namespace Dropper
{
    public class ClickFilter : IMessageFilter
    {
        public Control TargetControl;

        public ClickFilter(Control targetControl) => TargetControl = targetControl;

        public bool PreFilterMessage(ref Message m)
        {
            const int LBM_code = 0x0201;

            if (m.Msg == LBM_code)
            {
                Control clickedControl = Control.FromHandle(m.HWnd);

                if (clickedControl != TargetControl && !TargetControl.Contains(clickedControl))
                    TargetControl.FindForm().Focus();
            }
            return false;
        }
    }
}
