using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class ViewConfig : CustomPanel //temporary
    {
        public bool On;
        private Point InitialLocation;
        private bool InitialLocationSet = false;
        private Size FullSize;
        private bool FullSizeSet;
        private readonly LerpButton instanceTrigger;
        private readonly InstancePanel1 instancePanel;
        private readonly LerpButton configTrigger;
        private readonly ConfigPanel1 configPanel;

        public ViewConfig()
        {
            Draggable = true;
            BackColor = Color.FromArgb(100, 40, 40, 40);

            instanceTrigger = new LerpButton(animate: false)
            {
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "Instance",
            };
            Controls.Add(instanceTrigger);

            instancePanel = new InstancePanel1();
            Controls.Add(instancePanel);

            configTrigger = new LerpButton(animate: false)
            {
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "Config",
                AutoSize = true,
            };
            QOL.Align.Bottom.Center(configTrigger, instanceTrigger);
            Controls.Add(configTrigger);

            configPanel = new ConfigPanel1();
            Controls.Add(configPanel);

            instanceTrigger.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    TogglePanels(instanceTrigger, instancePanel, configTrigger, configPanel);
                }
            };

            configTrigger.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    TogglePanels(configTrigger, configPanel, instanceTrigger, instancePanel);
                }
            };
        }

        private void UpdateColor(LerpButton button)
        {
            button.CurrentColor = button.On ? QOL.RGB(50) : QOL.RGB(35);
            button.Invalidate();
        }

        private void TogglePanels(LerpButton activeButton, CustomPanel activePanel, LerpButton inactiveButton, CustomPanel inactivePanel)
        {
            inactiveButton.On = inactivePanel.Visible = activeButton.On && inactiveButton.On;
            UpdateColor(inactiveButton);
            activeButton.On = activePanel.Visible = !activeButton.On && !inactiveButton.On;
            UpdateColor(activeButton);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!FullSizeSet)
            {
                FullSize = Size;
                FullSizeSet = true;
            }

            if (instanceTrigger != null)
            {
                QOL.AutoHeight(instanceTrigger, 1.5f);
                instanceTrigger.Location = Point.Empty;
                instanceTrigger.Width = Width / 2;

                if (instancePanel != null)
                {
                    instancePanel.Location = new Point(0, instanceTrigger.Bottom);
                    instancePanel.Size = new Size(Width, Height - instanceTrigger.Height);
                }
            }

            if (configTrigger != null)
            {
                QOL.AutoHeight(configTrigger, 1.5f);
                configTrigger.Location = new Point(Width / 2, 0);
                configTrigger.Width = Width / 2;

                if (configPanel != null)
                {
                    configPanel.Location = new Point(0, configTrigger.Bottom);
                    configPanel.Size = new Size(Width, Height - configTrigger.Height);
                }
            }
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

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            HandleMouseDoubleClick(e);
        }

        private void HandleMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
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
