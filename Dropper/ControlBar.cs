using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class ControlBar : CustomPanel
    {
        private readonly CustomButton closingButton;
        private readonly CustomButton minimizeButton;

        private readonly ToolbarPanel toolBar = new ToolbarPanel();

        private readonly List<ControlledPanel> Controllers = new List<ControlledPanel>();
        private readonly LerpButton PanelTrigger;
        private readonly CustomPanel PanelOptions;
        private readonly ControlledPanel PanelController;

        public ControlBar()
        {
            Drag();
            BackColor = QOL.RGB(35);

            toolBar.Visible = false;
            Controls.Add(toolBar);

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

            PanelTrigger = new LerpButton(0.00005f, QOL.RandomColor())
            {
                BackColor = QOL.RandomColor(),
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Underline),
                Text = "Paneling",
                Size = new Size(140, 40)
            };
            Controls.Add(PanelTrigger);

            PanelOptions = new CustomPanel()
            {
                Height = 160,
                BackColor = QOL.RandomColor(),
            };
            Controls.Add(PanelOptions);

            string[] names = toolBar.Values
                        .Select(x =>
                             x.GetType().Name
                                .Substring(0, x.GetType().Name.Skip(1).TakeWhile(c => !char.IsUpper(c)).Count() + 1))
                        .ToArray();
            names[1] = "Slider";
            PanelController = new ControlledPanel(PanelTrigger, PanelOptions, new LerpButton[names.Length], names);
            Controllers.Add(PanelController);
            PanelController.Showing += (s, ev) =>
            {
                var showingController = s as ControlledPanel;

                foreach (var controller in Controllers)
                    if (controller != showingController)
                        PanelController.Hide();
            };
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            closingButton.Location = new Point(ClientSize.Width - closingButton.Width);
            QOL.Align.Left(minimizeButton, closingButton, 4);

            toolBar.Location = new Point(PanelTrigger.Left, PanelTrigger.Bottom);
            toolBar.Size = new Size(ClientSize.Width, ClientSize.Height - PanelTrigger.Height);
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
    }
}