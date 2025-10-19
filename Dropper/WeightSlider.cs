using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class WeightSlider : CustomPanel
    {
        private readonly Block Block;
        public TrackBarOverlayed bar;

        public event Action<float> WeightChanged;

        public WeightSlider(Block block)
        {
            Block = block;
            BuildWeightSlider();
        }

        private void BuildWeightSlider()
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
            bar.Value = 4;

            void UpdateBar()
            {
                if (positive)
                    Block.Weight = barValues[bar.Value];
                else
                    Block.Weight = -barValues[bar.Value];
                WeightChanged?.Invoke(Block.Weight);
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
                    if (positive)
                        bar.RailCover = Color.LimeGreen;
                    else
                        bar.RailCover = Color.IndianRed;
                    timer.Start();
                    UpdateBar();
                    bar.Invalidate();

                    timer.Start();
                }
            };
            bar.ValueChanged += (s, ev) => UpdateBar();
        }
    }
}
