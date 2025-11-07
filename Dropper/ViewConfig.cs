using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Dropper
{
    public class ViewConfig : CustomPanel //temporary
    {
        private LerpButton closing;
        private Label warning;
        private Size warningSize;

        public ViewConfig()
        {
            Draggable = true;
            BackColor = Color.FromArgb(100, 40, 40, 40);

            closing = new LerpButton()
            {
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Crimson,
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "✖",
                AutoSize = true,
            };
            closing.MouseDown += (s, ev) => { if (ev.Button == MouseButtons.Left) Visible = false; };
            Controls.Add(closing);

            if (!File.Exists("Droptions.txt"))
            {
                warning = new Label
                {
                    BackColor = Color.Transparent,
                    ForeColor = Color.Crimson,
                    Text = "Droptions.txt does not exist!",
                    Font = new Font(QOL.VCROSDMONO, 21f),
                    AutoSize = true,
                };
                warningSize = TextRenderer.MeasureText(warning.Text, warning.Font);
                Controls.Add(warning);

                var prompt = new Label
                {
                    BackColor = Color.Transparent,
                    ForeColor = Color.White,
                    Text = $"Allow a txt file to be created in the same directory as Dropper.exe to allow custom config?",
                    Font = new Font(QOL.VCROSDMONO, 16f),
                    Location = new Point(0, warning.Location.Y + warningSize.Height + 10),
                    AutoSize = true,
                    MaximumSize = new Size(Width - 10, 0)
                };
                Controls.Add(prompt);

                var yes = new LerpButton
                {
                    BackColor = BackColor,
                    ForeColor = Color.Green,
                    Font = new Font(QOL.VCROSDMONO, 16f),
                    Text = "yes",
                    Location = new Point(0, prompt.Bottom),
                    AutoSize = true,
                };
                Controls.Add(yes);
                yes.MouseDown += (s, ev) =>
                {
                    if (ev.Button == MouseButtons.Left)
                    {
                        File.Create("Droptions.txt").Close();
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
                QOL.Align.Right(no, yes, 5);
                Controls.Add(no);
            }
            File.Delete("Droptions.txt");
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            closing.Location = new Point(Width - closing.Width, 0);
            warning.Location = new Point(Width / 2 - warningSize.Width / 2, closing.Bottom);
        }
    }
}
