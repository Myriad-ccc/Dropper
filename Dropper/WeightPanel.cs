using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class WeightPanel : CustomPanel
    {
        private Block targetBlock;
        public ClickFilter WeightDisplayFilter;

        public TextBox weightDisplay;
        public float originalWeight;

        public Button collapsableMenu;
        public event EventHandler CollapseExpandedWeightPanel;

        public void SetActiveBlock(Block block)
        {
            targetBlock = block;
            if (block != null)
            {
                originalWeight = targetBlock.Weight;
                weightDisplay.Text = $"{targetBlock.Weight:F1}";
            }
        }

        public WeightPanel(Block block)
        {
            if (targetBlock == null) targetBlock = block;

            originalWeight = targetBlock.Weight;
            BuildWeightPanel();
        }

        public void UpdateWeightDisplay()
        {
            if (weightDisplay != null)
                weightDisplay.Text = $"{targetBlock:F1}";
        }

        private void BuildWeightPanel()
        {
            ForeColor = Color.Transparent;
            BackColor = QOL.RGB(35);
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
                Text = $"{targetBlock:F1}",
                Width = 92,
                Location = new Point()
            };
            weightDisplay.TextChanged += (s, ev) =>
            {
                if (float.TryParse(weightDisplay.Text, out float newWeight))
                    targetBlock.Weight = newWeight;
                else targetBlock.Weight = originalWeight;
            };
            weightDisplay.LostFocus += (s, ev) =>
            {
                if (string.IsNullOrEmpty(weightDisplay.Text)
                || !float.TryParse(weightDisplay.Text, out _))
                {
                    weightDisplay.Text = originalWeight.ToString("F1");
                    targetBlock.Weight = originalWeight;
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
                targetBlock.Weight = originalWeight;
                weightDisplay.Text = $"{targetBlock.Weight:F1}";
            };

            collapsableMenu = QOL.GenericControls.Button(12f, "+", Color.White);
            QOL.Align.Right(collapsableMenu, resetWeight);
            Controls.Add(collapsableMenu);
            collapsableMenu.MouseClick += (s, ev) => CollapseExpandedWeightPanel?.Invoke(this, EventArgs.Empty);
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
