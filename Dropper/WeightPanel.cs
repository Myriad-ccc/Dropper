using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class WeightPanel : CustomPanel
    {
        public ClickFilter WeightDisplayFilter;

        public TextBox weightDisplay;

        public Button collapsableMenu;
        public event EventHandler CollapseExpandedWeightPanel;

        private bool built;
        private Block targetBlock;

        public void SetTarget(Block block)
        {
            targetBlock = block ?? throw new ArgumentNullException();
            if (!built)
            {
                Build();
                built = true;
            }
            else
                UpdateWeightDisplay();
        }

        public void UpdateWeightDisplay()
        {
            if (targetBlock.Weight > -100 && targetBlock.Weight < 100)
                weightDisplay.Text = $"{targetBlock.Weight:F1}";
            else
                weightDisplay.Text = $"{targetBlock.Weight:F0}";
        }

        private void Build()
        {
            ForeColor = Color.Transparent;
            BackColor = QOL.RGB(35);
            Width = 264;
            Height = 24;
            Paint += (s, ev) =>
            {
                using (var pen = new Pen(BackColor, 1f))
                    ev.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            };

            weightDisplay = new TextBox()
            {
                Anchor = AnchorStyles.Left,
                TabStop = false,
                BorderStyle = BorderStyle.None,
                BackColor = QOL.RGB(100),
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 17f),
                Text = $"{targetBlock.Weight:F1}",
                Width = 92,
            };
            weightDisplay.TextChanged += (s, ev) =>
            {
                if (float.TryParse(weightDisplay.Text, out float newWeight))
                {
                    if (newWeight > 0)
                        targetBlock.Weight = Math.Min(newWeight, 100000);
                    if (newWeight < 0)
                        targetBlock.Weight = Math.Max(newWeight, -100000);
                }
                else targetBlock.Weight = targetBlock.OriginalWeight;
                UpdateWeightDisplay();
            };
            weightDisplay.LostFocus += (s, ev) =>
            {
                if (string.IsNullOrEmpty(weightDisplay.Text)
                || !float.TryParse(weightDisplay.Text, out _))
                {
                    targetBlock.Weight = targetBlock.OriginalWeight;
                    UpdateWeightDisplay();
                }
            };
            Controls.Add(weightDisplay);
            WeightDisplayFilter = new ClickFilter(weightDisplay);
            Application.AddMessageFilter(WeightDisplayFilter);

            var resetWeight = QOL.GenericControls.Button(18f, "↻", Color.White);
            QOL.Align.Right(resetWeight, weightDisplay, 4);
            Controls.Add(resetWeight);
            resetWeight.MouseClick += (s, ev) =>
            {
                targetBlock.Weight = targetBlock.OriginalWeight;
                UpdateWeightDisplay();
            };

            collapsableMenu = QOL.GenericControls.Button(12f, "+", Color.White);
            QOL.Align.Right(collapsableMenu, resetWeight);
            Controls.Add(collapsableMenu);
            collapsableMenu.MouseClick += (s, ev) => CollapseExpandedWeightPanel.Invoke(this, EventArgs.Empty);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            var form = FindForm();
            if (form != null)
            {
                form.FormClosing -= FormClosing;
                form.FormClosing += FormClosing;
            }
        }

        private void FormClosing(object sender, EventArgs e)
        {
            if (WeightDisplayFilter != null)
                Application.RemoveMessageFilter(WeightDisplayFilter);
        }
    }
}
