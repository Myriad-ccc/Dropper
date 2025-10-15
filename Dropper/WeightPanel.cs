using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class WeightPanel : CustomPanel
    {
        public ClickFilter WeightDisplayFilter;
        private readonly Block Block;
        private readonly Gravity Gravity;

        public WeightPanel(Block block, Gravity gravity)
        {
            Block = block;
            Gravity = gravity;
            BuildWeightPanel();
        }

        private void BuildWeightPanel()
        {
            ForeColor = Color.Transparent;
            BackColor = QOL.Colors.SameRGB(35);
            Width = 1024;
            Height = 24;
            Paint += (s, ev) =>
            {
                using (var pen = new Pen(BackColor, 1f))
                    ev.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            };

            var weightLabel = new Label()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 16f),
                Text = "Weight",
                Height = Height,
            };
            Controls.Add(weightLabel);

            float originalWeight = Block.Weight;
            var weightDisplay = new TextBox()
            {
                Anchor = AnchorStyles.Left,
                BackColor = QOL.Colors.SameRGB(100),
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 16f),
                Text = $"{Block.Weight:F1}",
                Width = weightLabel.Width,
                BorderStyle = BorderStyle.None,
                TabStop = false,
            };
            QOL.Align.Right(weightDisplay, weightLabel, 6);
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

            var minusWeight = new Button()
            {
                UseCompatibleTextRendering = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(255, 163, 42, 42),
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Regular),
                Text = "-",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(24, 24)
            };
            QOL.Align.Right(minusWeight, weightDisplay, 4);
            Controls.Add(minusWeight);
            minusWeight.MouseClick += (s, ev) =>
            {
                Block.Weight = Block.Weight - 5 > int.MinValue ? Block.Weight - 5 : Block.Weight;
                weightDisplay.Text = Block.Weight.ToString("F1");
            };

            var plusWeight = new Button()
            {
                UseCompatibleTextRendering = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Green,
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Regular),
                Text = "+",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = minusWeight.Size
            };
            QOL.Align.Right(plusWeight, minusWeight, 1);
            Controls.Add(plusWeight);
            plusWeight.MouseClick += (s, ev) =>
            {
                Block.Weight = Block.Weight + 5 < int.MaxValue ? Block.Weight + 5 : Block.Weight;
                weightDisplay.Text = Block.Weight.ToString("F1");
            };

            var zeroWeight = new Button()
            {
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.Gray,
                Font = new Font(QOL.VCROSDMONO, 13f, FontStyle.Regular),
                Text = "0",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = minusWeight.Size
            };
            QOL.Align.Right(zeroWeight, plusWeight, 1);
            Controls.Add(zeroWeight);
            zeroWeight.MouseClick += (s, ev) =>
            {
                Block.Weight = 0;
                weightDisplay.Text = Block.Weight.ToString("F1");
            };

            var oneWeight = new Button()
            {
                UseCompatibleTextRendering = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Regular),
                Text = "1",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = minusWeight.Size
            };
            QOL.Align.Right(oneWeight, zeroWeight, 1);
            Controls.Add(oneWeight);
            oneWeight.MouseClick += (s, ev) =>
            {
                Block.Weight = 1;
                weightDisplay.Text = Block.Weight.ToString("F1");
            };

            var millionWeight = new Button()
            {
                UseCompatibleTextRendering = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.RoyalBlue,
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Regular),
                Text = "M",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = minusWeight.Size
            };
            QOL.Align.Right(millionWeight, oneWeight, 1);
            Controls.Add(millionWeight);
            millionWeight.MouseClick += (s, ev) =>
            {
                Block.Weight = 10000000;
                weightDisplay.Text = Block.Weight.ToString("F1");
            };

            var resetWeight = new Button()
            {
                UseCompatibleTextRendering = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                Font = new Font(QOL.VCROSDMONO, 18f, FontStyle.Regular),
                Text = "↻",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = minusWeight.Size
            };
            QOL.Align.Right(resetWeight, millionWeight, 1);
            Controls.Add(resetWeight);
            resetWeight.MouseClick += (s, ev) =>
            {
                Block.Weight = originalWeight;
                weightDisplay.Text = Block.Weight.ToString("F1");
            };

            var absoluteWeight = new Button()
            {
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.LightGoldenrodYellow,
                Font = new Font(QOL.VCROSDMONO, 12f, FontStyle.Regular),
                Text = "Abs",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(48, 24)
            };
            QOL.Align.Right(absoluteWeight, resetWeight, 1);
            Controls.Add(absoluteWeight);
            absoluteWeight.MouseClick += (s, ev) =>
            {
                if (Math.Abs(Block.Weight) > int.MinValue && Math.Abs(Block.Weight) < int.MaxValue)
                    Block.Weight = Math.Abs(Block.Weight);
                else
                    Block.Weight = originalWeight;
                weightDisplay.Text = Block.Weight.ToString("F1");
            };

            foreach (var button in Controls.OfType<Button>())
                button.BackColor = QOL.Colors.SameRGB(20);
        }
    }
}
