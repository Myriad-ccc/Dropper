using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Dropper
{
    public class ControlBar : CustomPanel
    {
        private readonly CustomButton closingButton;
        private readonly CustomButton minimizeButton;

        private readonly CustomPanel PanelingPanel = new CustomPanel();
        private readonly LerpButton Paneling;

        private readonly Timer AnimationTimer = new Timer() { Interval = 10 };
        private readonly int AnimationSpeed = 1;
        private readonly int TargetHeight = 160;
        private bool Expanding = false;


        public ControlBar()
        {
            Drag();
            BackColor = QOL.RGB(35);

            closingButton = new CustomButton()
            {
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(64, 64),
                BackColor = QOL.RGB(20),
                ForeColor = Color.FromArgb(255, 163, 42, 42),
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "✖",
            };
            closingButton.MouseClick += (s, ev) => FindForm()?.Close();
            Controls.Add(closingButton);

            minimizeButton = new CustomButton()
            {
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(64, 64),
                BackColor = QOL.RGB(20),
                ForeColor = Color.FromArgb(255, 42, 163, 150),
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "―",
            };
            minimizeButton.MouseClick += (s, ev) => FindForm().WindowState = FormWindowState.Minimized;
            Controls.Add(minimizeButton);

            Paneling = new LerpButton(0.00005f, QOL.RandomColor())
            {
                BackColor = QOL.RandomColor(),
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Underline),
                Text = "Paneling",
                AutoSize = true,
            };
            Controls.Add(Paneling);

            PanelingPanel = new CustomPanel()
            {
                Width = Paneling.Width,
                Height = 0,
                Location = new Point(Paneling.Left, Paneling.Bottom),
                BackColor = QOL.RandomColor(),
                Visible = false
            };
            Controls.Add(PanelingPanel);

            AddButtons();

            AnimationTimer.Tick += (s, ev) =>
            {
                if (Expanding)
                {
                    int diff = TargetHeight - PanelingPanel.Height;
                    PanelingPanel.Height += AnimationSpeed;
                    if (PanelingPanel.Height >= TargetHeight)
                    {
                        PanelingPanel.Height = TargetHeight;
                        AnimationTimer.Stop();
                    }
                }
                else
                {
                    int diff = PanelingPanel.Height;
                    PanelingPanel.Height -= AnimationSpeed;
                    if (PanelingPanel.Height <= 0)
                    {
                        PanelingPanel.Height = 0;
                        PanelingPanel.Visible = false;
                        AnimationTimer.Stop();
                    }
                }
            };

            Paneling.Tag = PanelingPanel;
            Paneling.MouseEnter += ShowMe;
            Paneling.MouseLeave += CheckState;
            PanelingPanel.MouseLeave += CheckState;
            foreach (Control control in PanelingPanel.Controls.OfType<Button>())
                control.MouseLeave += CheckState;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            closingButton.Location = new Point(ClientSize.Width - closingButton.Width);
            QOL.Align.Left(minimizeButton, closingButton, 4);
        }

        public void Drag()
        {
            bool MouseDragging = false;
            Point cursorPos = Cursor.Position;

            MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    MouseDragging = true;
                    cursorPos = Cursor.Position;
                }
            };

            MouseUp += (s, ev) => MouseDragging = false;

            MouseMove += (s, ev) =>
            {
                if (MouseDragging)
                {
                    int deltaX = Cursor.Position.X - cursorPos.X;
                    int deltaY = Cursor.Position.Y - cursorPos.Y;

                    FindForm().Left += deltaX;
                    FindForm().Top += deltaY;

                    cursorPos = Cursor.Position;

                    Invalidate();
                }
            };
        }

        private void ShowMe(object sender, EventArgs e)
        {
            foreach (Button sibling in Paneling.Parent.Controls.OfType<Button>())
                if (sibling != Paneling && sibling.Tag is Panel siblingPanel)
                    HideMe(siblingPanel);
            PanelingPanel.Visible = true;
            Expanding = true;
            AnimationTimer.Start();
        }

        private void HideMe(Panel panel)
        {
            if (!panel.Visible)
                return;

            Expanding = false;
            AnimationTimer.Start();
        }

        private bool Hovering(Panel panel)
        {
            if (!panel.Visible)
                return false;

            if (panel.Bounds.Contains(PointToClient(Cursor.Position)))
                return true;

            foreach (Button button in PanelingPanel.Controls.OfType<Button>())
                if (button.Tag is Panel descendent)
                    if (Hovering(descendent))
                        return true;
            return false;
        }

        private void CheckState(object sender, EventArgs e)
        {
            if (Paneling.Bounds.Contains(PointToClient(Cursor.Position)))
                return;

            if (Hovering(PanelingPanel))
                return;

            HideMe(PanelingPanel);
        }

        private void AddButtons()
        {
            int buttonCount = 5;
            var buttons = new LerpButton[buttonCount];
            string[] names = { "Weight", "Expanded", "Slider", "Pivot", "Gravity", "Empty" };

            for (int i = 0; i < buttonCount; i++)
            {
                buttons[i] = new LerpButton(null, QOL.RGB(163, 42, 42))
                {
                    TabStop = false,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(PanelingPanel.Width, TargetHeight / 5),
                    Location = new Point(0, i * (TargetHeight / 5)),
                    Font = new Font(QOL.VCROSDMONO, 16f),
                    Text = names[i],
                };
                PanelingPanel.Controls.Add(buttons[i]);
            }
        }
    }
}