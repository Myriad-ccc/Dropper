using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class ConfigTab : CustomPanel
    {
        public bool On;

        public Point InitialLocation;
        private bool InitialLocationSet = false;
        public Size InitialSize;
        private bool InitialSizeSet;

        private readonly CustomLabel configLabel;

        private readonly LerpButton closingButton;

        private readonly LerpButton cancelButton;
        private readonly LerpButton applyButton;

        private readonly CustomPanel workArea;


        public ConfigTab()
        {
            Draggable = true;
            BackColor = QOL.RGB(30);

            configLabel = new CustomLabel(25f)
            {
                Text = "Config",
                AutoSize = true,
            };
            Controls.Add(configLabel);

            closingButton = new LerpButton()
            {
                TextAlign = ContentAlignment.MiddleCenter,
                BaseColor = QOL.RGB(22),
                CurrentColor = QOL.RGB(22),
                ClickColor = Color.Crimson,
                ForeColor = QOL.RGB(163, 42, 42),
                Font = new Font(QOL.VCROSDMONO, 28f),
                Text = "✖",
                Size = new Size(48, 48)
            };
            closingButton.MouseUp += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    if (closingButton.Bounds.Contains(
                        PointToClient(
                            closingButton.PointToScreen(
                                ev.Location))))
                    {
                        Location = InitialLocation;
                        Visible = false;
                        On = false;
                    }
                }
            };
            Controls.Add(closingButton);

            cancelButton = new LerpButton()
            {
                ShowBorder = true,
                BaseColor = QOL.RGB(40),
                CurrentColor = QOL.RGB(40),
                Font = new Font(QOL.VCROSDMONO, 23f),
                Text = "Cancel",
                AutoSize = true,
            };
            Controls.Add(cancelButton);

            applyButton = new LerpButton()
            {
                ShowBorder = true,
                BaseColor = QOL.RGB(40),
                CurrentColor = QOL.RGB(40),
                Font = new Font(QOL.VCROSDMONO, 23f),
                Text = "Apply",
                AutoSize = true,
            };
            Controls.Add(applyButton);

            workArea = new CustomPanel()
            {
                BackColor = QOL.RGB(15)
            };
            Controls.Add(workArea);

            SizeChanged += (s, ev) =>
            {
                closingButton.Location = new Point(ClientSize.Width - closingButton.Width, 0);
                cancelButton.Location = new Point(ClientSize.Width - cancelButton.Width, ClientSize.Height - cancelButton.Height);
                QOL.Align.Left(applyButton, cancelButton);
                workArea.Bounds = new Rectangle(0, closingButton.Bottom, ClientSize.Width, ClientSize.Height - closingButton.Bottom - applyButton.Height);
                configLabel.Location = new Point((ClientSize.Width - closingButton.Width) / 2 - TextRenderer.MeasureText(configLabel.Text, configLabel.Font).Width / 2, workArea.Top / 2 - TextRenderer.MeasureText(configLabel.Text, configLabel.Font).Height / 2);

                if (workArea == null) return;

                var labelNames = new CustomPanel()
                {
                    BackColor = QOL.RGB(40),
                    Width = 250,
                    Height = workArea.Height
                };
                workArea.Controls.Add(labelNames);

                var colors = new LerpButton()
                {
                    Text = "⬈Colors",
                    AutoSize = true,
                    Location = new Point()
                };
                labelNames.Controls.Add(colors);

                var colorOptions = new FlowLayoutPanel
                {
                    BackColor = QOL.RGB(40),
                    Visible = false,
                    Location = new Point(0, colors.Bottom),
                    Width = labelNames.Width,
                    Height = labelNames.Height,
                    FlowDirection = FlowDirection.TopDown,
                    AutoSize = true,
                };
                labelNames.Controls.Add(colorOptions);

                string[] names = { "Lerp Buttons", "Custom Panels", "Labels", "Empty", "Empty", "Empty", "Empty" };
                foreach (var name in names)
                {
                    var button = new LerpButton
                    {
                        CurrentColor = QOL.RGB(40),
                        BaseColor = QOL.RGB(40),
                        Text = name,
                        AutoSize = true,
                        Margin = new Padding(0, 0, 0, 5) // spacing between buttons
                    };
                    colorOptions.Controls.Add(button);
                }


                bool colorsFolded = true;
                colors.MouseClick += (s, ev) =>
                {
                    if (ev.Button == MouseButtons.Left)
                    {
                        colorsFolded = !colorsFolded;
                        if (colorsFolded)
                        {
                            colorOptions.Visible = false;
                            colors.Text = "⬈Colors";
                        }
                        else
                        {
                            colorOptions.Visible = true;
                            colors.Text = "⬊Colors";
                        }
                    }
                };
            };
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (!InitialLocationSet)
            {
                InitialLocation = Location;
                InitialLocationSet = true;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!InitialSizeSet)
            {
                InitialSize = Size;
                InitialLocationSet = true;
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            HandleMouseDoubleClick(e);
        }

        private void HandleMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Location = InitialLocation;
                Visible = false;
                On = false;
            }
        }

        private void DescendentMouseDoubleClick(object sender, MouseEventArgs e) => HandleMouseDoubleClick(e);

        private void HandleMouseMiddleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                Location = InitialLocation;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            HandleMouseMiddleClick(e);
        }

        private void DescendentMiddleClick(object sender, MouseEventArgs e) => HandleMouseMiddleClick(e);

        private void WireControl(Control control)
        {
            control.MouseDoubleClick += DescendentMouseDoubleClick;
            control.MouseDown += DescendentMiddleClick;

            if (control.HasChildren)
                foreach (Control c in control.Controls)
                    WireControl(c);
        }

        private void UnwireControl(Control control)
        {
            control.MouseDoubleClick -= DescendentMouseDoubleClick;
            control.MouseDown -= DescendentMiddleClick;

            if (control.HasChildren)
                foreach (Control c in control.Controls)
                    UnwireControl(c);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            WireControl(e.Control);
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            UnwireControl(e.Control);
        }
    }
}