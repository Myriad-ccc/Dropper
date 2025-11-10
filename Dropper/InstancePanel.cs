using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class InstancePanel : CustomPanel
    {
        private readonly string instanceSavePath = Form1.instanceSavePath;

        public InstancePanel()
        {
            BackColor = Color.Transparent;
            Visible = false;

            if (!File.Exists(instanceSavePath))
            {
                var warning = new Label
                {
                    BackColor = Color.Transparent,
                    ForeColor = Color.Crimson,
                    Text = $"{instanceSavePath} does not exist!",
                    Font = new Font(QOL.VCROSDMONO, 21f),
                    AutoSize = true,
                };
                var warningSize = TextRenderer.MeasureText(warning.Text, warning.Font);
                Controls.Add(warning);

                var prompt = new Label
                {
                    BackColor = Color.Transparent,
                    ForeColor = Color.White,
                    Text = $"Allow a JSON file to be created in the same directory as Dropper.exe to store current layout on close?",
                    Font = new Font(QOL.VCROSDMONO, 16f),
                    AutoSize = true,
                };
                Controls.Add(prompt);

                var yes = new LerpButton
                {
                    BackColor = BackColor,
                    ForeColor = Color.Green,
                    Font = new Font(QOL.VCROSDMONO, 16f),
                    Text = "yes",
                    AutoSize = true,
                };
                Controls.Add(yes);
                yes.MouseDown += (s, ev) =>
                {
                    if (ev.Button == MouseButtons.Left)
                    {
                        File.Create(instanceSavePath).Close();
                        foreach (Control control in Controls.OfType<Control>())
                            control.Dispose();
                        Controls.Clear();
                        Refresh();
                    }
                };

                var no = new LerpButton
                {
                    BackColor = BackColor,
                    ForeColor = Color.Red,
                    Font = new Font(QOL.VCROSDMONO, 16f),
                    Text = "no",
                    AutoSize = true,
                };
                Controls.Add(no);

                SizeChanged += (s, ev) =>
                {
                    warning?.Location = new Point(Width / 2 - warningSize.Width / 2, 0);
                    if (prompt != null)
                    {
                        prompt.Location = new Point(0, warning.Location.Y + warningSize.Height + 10);
                        prompt.MaximumSize = new Size(Width - 10, 0);
                    }
                    yes?.Location = new Point(0, prompt.Bottom);
                    no?.Location = new Point(yes.Location.X + yes.Width + 5, yes.Location.Y);
                };
            }

            var cube = new Rectangle();
            cube.Size = new Size(64, 64);
            Paint += (s, ev) =>
            {
                var g = ev.Graphics;
                g.DrawString(FindForm()?.Text, new Font(QOL.VCROSDMONO, 30f), new SolidBrush(Color.MediumPurple), new Point());
            };
        }
    }
}
