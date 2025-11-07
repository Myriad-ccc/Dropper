using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class ControlledPanel
    {
        public LerpButton Trigger { get; }
        public CustomPanel Options { get; }

        public event EventHandler Showing;

        private readonly Timer AnimationTimer = new Timer() { Interval = 10 };
        private int AnimationSpeed;
        private readonly int TargetHeight;
        private bool Expanding = false;

        public ControlledPanel(
            LerpButton trigger, CustomPanel options,
            int buttons, string[] names,
            int animationSpeed = 5
            )
        {
            Trigger = trigger;
            Options = options;
            AnimationSpeed = animationSpeed;
            TargetHeight = buttons * 32;

            options.Visible = false;
            options.Width = Trigger.Width;
            options.Height = 0;

            for (int i = 0; i < buttons; i++)
            {
                var button = new LerpButton()
                {
                    TabStop = false,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(Options.Width, 32),
                    Location = new Point(0, i * 32),
                    Font = new Font(QOL.VCROSDMONO, 16f),
                    Text = names[i],
                };
                Options.Controls.Add(button);
            }

            AnimationTimer.Tick += (s, ev) =>
            {
                if (Expanding)
                {
                    Options.Height += AnimationSpeed;
                    if (Options.Height >= TargetHeight)
                    {
                        Options.Height = TargetHeight;
                        AnimationTimer.Stop();
                    }
                }
                else
                {
                    Options.Height -= AnimationSpeed;
                    if (Options.Height <= 0)
                    {
                        Options.Height = 0;
                        Options.Visible = false;
                        AnimationTimer.Stop();
                        AnimationSpeed = Math.Min(++AnimationSpeed, animationSpeed + 5);
                    }
                }
            };

            Trigger.Tag = Options;
            Trigger.MouseEnter += Show;
            Trigger.MouseLeave += CheckState;
            Options.MouseLeave += CheckState;
            foreach (Control control in Options.Controls.OfType<Button>())
                control.MouseLeave += CheckState;
        }

        public void Show(object sender, EventArgs e)
        {
            Showing?.Invoke(sender, EventArgs.Empty);

            Point triggerBottomLeft = Trigger.Parent.PointToScreen(new Point(Trigger.Left, Trigger.Bottom));
            Options.Location = Options.Parent.PointToClient(triggerBottomLeft);

            Options.Visible = true;
            Expanding = true;
            AnimationTimer.Start();
        }

        public void Hide()
        {
            if (!Options.Visible || Options.Height == 0)
                return;

            Expanding = false;
            AnimationTimer.Start();
        }

        private bool Hovering(Panel panel)
        {
            if (!panel.Visible)
                return false;

            if (panel.Bounds.Contains(panel.Parent.PointToClient(Cursor.Position)))
                return true;

            foreach (Button button in panel.Controls.OfType<Button>())
                if (button.Tag is Panel descendent)
                    if (Hovering(descendent))
                        return true;
            return false;
        }

        private void CheckState(object sender, EventArgs e)
        {
            if (Trigger.Parent == null)
                return;

            if (Trigger.Bounds.Contains(Trigger.Parent.PointToClient(Cursor.Position)))
                return;

            if (Hovering(Options))
                return;

            Hide();
        }
    }
}