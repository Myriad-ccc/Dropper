using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class WeightSlider : CustomPanel
    {
        public TrackBarOverlayed bar;

        public event Action<float> WeightChanged;

        private bool built;
        private Block targetBlock;

        public void SetActiveBlock(Block block)
        {
            targetBlock = block ?? throw new ArgumentNullException();
            if (!built)
            {
                Build();
                built = true;
            }
        }

        private void Build()
        {
            Width = 144;

            BackColor = Color.Transparent;
            DoubleBuffered = true;
            Visible = true;

            bar = new TrackBarOverlayed
            {
                TabStop = false,
                BackColor = QOL.RGB(50),
                Size = new Size(ClientSize.Width, 20),
                TickStyle = TickStyle.None
            };
            Controls.Add(bar);

            bool positive = true;
            int[] barValues = { 0, 1, 2, 5, 10, 20, 35, 50, 100, 250, 500, 750, 1000, 10000, 100000 };
            bar.Maximum = barValues.Length - 1;
            bar.Value = 8;

            void UpdateBar()
            {
                if (positive)
                    targetBlock.Weight = barValues[bar.Value];
                else
                    targetBlock.Weight = -barValues[bar.Value];
                WeightChanged?.Invoke(targetBlock.Weight);
            }

            var timer = new Timer() { Interval = 1000 };
            int seconds = 0;
            timer.Tick += (s, ev) =>
            {
                seconds++;
                if (seconds == 5)
                {
                    bar.RailCover = Color.RoyalBlue;
                    bar.Invalidate();
                    timer.Stop();
                }
            };

            bar.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Right)
                {
                    timer.Stop();
                    seconds = 0;

                    positive = !positive;
                    bar.RailCover = positive ? Color.LimeGreen : Color.IndianRed;

                    UpdateBar();
                    bar.Invalidate();

                    timer.Start();
                }
            };
            bar.ValueChanged += (s, ev) => UpdateBar();
        }
    }
}
