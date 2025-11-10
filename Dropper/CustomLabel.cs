using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Dropper
{
    public class CustomLabel : Label
    {
        public CustomLabel(float fontSize = 24f)
        {
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font(QOL.VCROSDMONO, fontSize);
        }
    }
}
