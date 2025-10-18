using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class WeightPanel : CustomPanel
    {
        private readonly Block Block;
        public ClickFilter WeightDisplayFilter;

        public TextBox weightDisplay;
        public float originalWeight;

        public Button collapsableMenu;
        public event EventHandler CollapseExpandedWeightPanel;

        public WeightPanel(Block block)
        {
            Block = block;
            originalWeight = block.Weight;
            BuildWeightPanel();
        }

        public void UpdateWeightDisplay()
        {
            if (weightDisplay != null)
                weightDisplay.Text = $"{Block.Weight:F1}";
        }

        private void BuildWeightPanel()
        {
            ForeColor = Color.Transparent;
            BackColor = QOL.Colors.SameRGB(35);
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
                BackColor = QOL.Colors.SameRGB(100),
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 17f),
                Text = $"{Block.Weight:F1}",
                Width = 92,
                Location = new Point()
            };
            weightDisplay.TextChanged += (s, ev) =>
            {
                if (float.TryParse(weightDisplay.Text, out float newWeight))
                    Block.Weight = newWeight;
                else Block.Weight = originalWeight;
            };
            weightDisplay.LostFocus += (s, ev) =>
            {
                if (string.IsNullOrEmpty(weightDisplay.Text)
                || !float.TryParse(weightDisplay.Text, out _))
                {
                    weightDisplay.Text = originalWeight.ToString("F1");
                    Block.Weight = originalWeight;
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
                Block.Weight = originalWeight;
                weightDisplay.Text = $"{Block.Weight:F1}";
            };

            collapsableMenu = QOL.GenericControls.Button(12f, "+", Color.White);
            QOL.Align.Right(collapsableMenu, resetWeight);
            Controls.Add(collapsableMenu);
            collapsableMenu.MouseClick += (s, ev) => CollapseExpandedWeightPanel?.Invoke(this, EventArgs.Empty);
        }
    }
}
