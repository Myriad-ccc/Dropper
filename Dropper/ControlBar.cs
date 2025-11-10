using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class ControlBar : CustomPanel
    {
        private readonly CustomButton closingButton;
        private readonly CustomButton minimizeButton;

        private readonly List<ControlledPanel> Controllers = [];

        private readonly LerpButton PanelTrigger;
        private readonly ControlledPanel PanelController;

        private readonly LerpButton OptionTrigger;
        private readonly ControlledPanel OptionController;
        public event Action ShowOptions;

        public bool instanceSaving { get; set; }
        private readonly string instanceSaveLocation = Form1.instanceSavePath;
        private readonly LerpButton instancingButton;

        public event Action<bool> ShowToolBar;

        public ControlBar(ToolBarPanel toolBar, CustomPanel panelOptions, CustomPanel optionOptions)
        {
            Drag();
            BackColor = QOL.RGB(35);

            closingButton = new CustomButton()
            {
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
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
                Size = new Size(140, 40),
                Location = new Point(0, 4)
            };
            Controls.Add(PanelTrigger);

            string[] names = toolBar.Values
                        .Select(x =>
                             x.GetType().Name
                                .Substring(0, x.GetType().Name.Skip(1).TakeWhile(c => !char.IsUpper(c)).Count() + 1))
                        .ToArray();
            names[1] = "Slider";
            PanelController = new ControlledPanel(PanelTrigger, panelOptions, names.Length, names);
            Controllers.Add(PanelController);
            PanelController.Showing += (s, ev) =>
            {
                var showingController = s as ControlledPanel;

                foreach (var controller in Controllers)
                    if (controller != showingController)
                        controller.Hide();
            };

            var PanelButtons = panelOptions.Controls.OfType<LerpButton>().ToList();
            for (int i = 0; i < PanelButtons.Count; i++)
            {
                int b = i;
                var button = PanelButtons[b];
                button.MouseDown += (s, ev) =>
                {
                    if (ev.Button == MouseButtons.Left)
                    {
                        button.On = !button.On;
                        button.ForeColor = button.On ? Color.CornflowerBlue : Color.White;
                        toolBar.Values[b].Visible = button.On;
                        PanelTrigger.ForeColor = PanelButtons.All(x => x.On) ? Color.MediumPurple : Color.White;
                        ShowToolBar?.Invoke(PanelButtons.Any(x => x.On));
                    }
                };
            }

            OptionTrigger = new LerpButton(0.00005f, QOL.RandomColor())
            {
                BackColor = QOL.RandomColor(),
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Underline),
                Text = "Options",
                Height = 40,
            };
            OptionTrigger.Width = TextRenderer.MeasureText(OptionTrigger.Text, OptionTrigger.Font).Width;
            Controls.Add(OptionTrigger);
            QOL.Align.Right(OptionTrigger, PanelTrigger);

            string[] optionNames = ["Config", "empty", "mpty", "mty", "mt"];
            OptionController = new ControlledPanel(OptionTrigger, optionOptions, optionNames.Length, optionNames);
            Controllers.Add(OptionController);

            OptionController.Showing += (s, ev) =>
            {
                var showingController = s as ControlledPanel;

                foreach (var controller in Controllers)
                    if (controller != showingController)
                        controller.Hide();
            };

            var OptionButtons = optionOptions.Controls.OfType<LerpButton>().ToList();
            for (int i = 0; i < OptionButtons.Count; i++)
            {
                int b = i;
                var button = OptionButtons[b];
                button.MouseDown += (se, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        button.On = !button.On;
                        button.ForeColor = button.On ? Color.CornflowerBlue : Color.White;
                        OptionTrigger.ForeColor = OptionButtons.All(x => x.On) ? Color.MediumPurple : Color.White;

                        if (b == 0)
                            ShowOptions?.Invoke();
                    }
                };
            }
            
            instancingButton = new LerpButton()
            {
                Animate = true,
                ShowBorder = false,
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "Saving",
            };
            QOL.AutoWidth(instancingButton);
            Controls.Add(instancingButton);
            UpdateInstancingStatus();

            instancingButton.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    if (!File.Exists(instanceSaveLocation))
                    {
                        File.Create(instanceSaveLocation).Close();
                        instancingButton.Text = "Saving";
                        QOL.AutoWidth(instancingButton);
                        QOL.Align.Left(instancingButton, minimizeButton, gap: 8, position: 1);
                    }
                    instanceSaving = !instanceSaving;
                    UpdateInstancingStatus();
                }
                else if (ev.Button == MouseButtons.Middle)
                    instancingButton.Visible = false;
            };
        }

        public void UpdateInstancingStatus()
        {
            if (!File.Exists(instanceSaveLocation))
            {
                instancingButton.Text = "Not Saving";
                QOL.AutoWidth(instancingButton);
                instancingButton.ForeColor = QOL.RGB(163, 42, 42);
                return;
            }
            instancingButton.ForeColor = instanceSaving ? Color.Green : Color.Gray;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            closingButton.Size = new Size(ClientSize.Height, ClientSize.Height);
            closingButton.Location = new Point(ClientSize.Width - closingButton.Width);
            minimizeButton.Size = closingButton.Size;
            QOL.Align.Left(minimizeButton, closingButton, 4);
            instancingButton.Height = 32;
            QOL.Align.Left(instancingButton, minimizeButton, gap: 8,position:1);
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