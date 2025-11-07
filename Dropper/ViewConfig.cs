using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class ViewConfig : CustomPanel //temporary
    {
        public bool On;

        private Size FullSize;
        private bool FullSizeSet;

        private LerpButton instance;
        private LerpButton config;

        private Label warning;
        private Size warningSize;
        private Label prompt;
        private LerpButton yes;
        private LerpButton no;

        public ViewConfig()
        {
            Draggable = true;
            BackColor = Color.FromArgb(100, 40, 40, 40);

            instance = new LerpButton(hoverAnimation:false)
            {
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "Instance",
            };
            Controls.Add(instance);

            void UpdateBaseColor(LerpButton button)
            {
                button.BaseColor = button.On ? QOL.RGB(50) : QOL.RGB(35);
                button.Invalidate();
            }

            instance.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    instance.On = !instance.On;
                    UpdateBaseColor(instance);
                    if (config != null)
                    {
                        config.On = !instance.On;
                        UpdateBaseColor(config);
                    }
                }
            };

            config = new LerpButton(hoverAnimation:false)
            {
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "Config",
                AutoSize = true,
            };
            QOL.Align.Bottom.Center(config, instance);
            Controls.Add(config);

            config.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    config.On = !config.On;
                    UpdateBaseColor(config);
                    if (instance != null)
                    {
                        instance.On = !config.On;
                        UpdateBaseColor(instance);
                    }
                }
            };

            //if (!File.Exists("Droptions.txt"))
            //{
            //    warning = new Label
            //    {
            //        BackColor = Color.Transparent,
            //        ForeColor = Color.Crimson,
            //        Text = "Droptions.txt does not exist!",
            //        Font = new Font(QOL.VCROSDMONO, 21f),
            //        AutoSize = true,
            //    };
            //    warningSize = TextRenderer.MeasureText(warning.Text, warning.Font);
            //    Controls.Add(warning);

            //    prompt = new Label
            //    {
            //        BackColor = Color.Transparent,
            //        ForeColor = Color.White,
            //        Text = $"Allow a txt file to be created in the same directory as Dropper.exe to allow custom config?",
            //        Font = new Font(QOL.VCROSDMONO, 16f),
            //        AutoSize = true,
            //    };
            //    Controls.Add(prompt);

            //    yes = new LerpButton
            //    {
            //        BackColor = BackColor,
            //        ForeColor = Color.Green,
            //        Font = new Font(QOL.VCROSDMONO, 16f),
            //        Text = "yes",
            //        AutoSize = true,
            //    };
            //    Controls.Add(yes);
            //    yes.MouseDown += (s, ev) =>
            //    {
            //        if (ev.Button == MouseButtons.Left)
            //        {
            //            File.Create("Droptions.txt").Close();
            //            foreach (Control control in Controls.OfType<Control>())
            //                control.Dispose();
            //            Controls.Clear();
            //            Refresh();
            //        }
            //    };

            //    no = new LerpButton
            //    {
            //        BackColor = BackColor,
            //        ForeColor = Color.Red,
            //        Font = new Font(QOL.VCROSDMONO, 16f),
            //        Text = "no",
            //        AutoSize = true,
            //    };
            //    Controls.Add(no);
            //}
            //File.Delete("Droptions.txt");
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!FullSizeSet)
            {
                FullSize = Size;
                FullSizeSet = true;
            }

            if (instance != null)
            {
                QOL.AutoHeight(instance, 1.5f);
                instance.Location = Point.Empty;
                instance.Width = Width / 2;
            }
            if (config != null)
            {
                QOL.AutoHeight(config, 1.5f);
                config.Location = new Point(Width / 2, 0);
                config.Width = Width / 2;
            }

            if (warning != null)
                warning.Location = new Point(Width / 2 - warningSize.Width / 2, 0);
            if (prompt != null)
            {
                prompt.Location = new Point(0, warning.Location.Y + warningSize.Height + 10);
                prompt.MaximumSize = new Size(Width - 10, 0);
            }
            if (yes != null)
                yes.Location = new Point(0, prompt.Bottom);
            if (no != null)
                QOL.Align.Right(no, yes, 5);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Right)
            {
                Visible = false;
                On = false;
            }
        }
    }
}