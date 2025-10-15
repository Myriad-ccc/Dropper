using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class PivotPanel : CustomPanel
    {
        private readonly Random random = new Random();
        private readonly Block Block;
        private readonly Gravity Gravity;

        public PivotPanel(Block block, Gravity gravity)
        {
            Block = block;
            Gravity = gravity;
            BuildPivotPanel();
        }

        private void BuildPivotPanel()
        {
            ForeColor = Color.Transparent;
            BackColor = QOL.Colors.SameRGB(35);
            Width = 1024;
            Height = 72;
            Paint += (s, ev) =>
               {
                   using (var pen = new Pen(BackColor, 1f))
                       ev.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
               };

            var pivotLabel = new Label()
            {
                Tag = "ToolBarInfoLabel",
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 22f),
                Text = $"Pivot",
                AutoSize = true,
                Height = Height,
            };
            Controls.Add(pivotLabel);


            //var bounce = new Button()
            //{
            //    UseCompatibleTextRendering = true,
            //    TextAlign = ContentAlignment.MiddleCenter,
            //    ForeColor = Color.CadetBlue,
            //    BackColor = Color.Transparent,
            //    Font = new Font(QOL.VCROSDMONO, 12f, FontStyle.Regular),
            //    Text = "B",
            //    TabStop = false,
            //    FlatStyle = FlatStyle.Flat,
            //    Size = new Size(24, 24),
            //};
            ////Controls.Add(bounce);
            //bool bounceBold = false;
            //bounce.MouseClick += (s, ev) =>
            //{
            //    bounceBold = !bounceBold;
            //    if (bounceBold)
            //        bounce.Font = new Font(bounce.Font.FontFamily, bounce.Font.Size, FontStyle.Bold | FontStyle.Underline | FontStyle.Italic);
            //    else
            //        bounce.Font = new Font(bounce.Font.FontFamily, bounce.Font.Size, FontStyle.Regular);
            //};
            //QOL.Align.Right(bounce, reorient, 1);


            var pivot = new CustomPanel
            {
                BackColor = QOL.Colors.SameRGB(60),
                Size = new Size(72, 72)
            };
            QOL.Align.Right(pivot, pivotLabel, 1);
            Controls.Add(pivot);

            string[,] directions =
            {
                { "↖", "↑", "↗" },
                { "←", "◎", "→" },
                { "↙", "↓", "↘" }
            };

            var cards = new Card[3, 3];
            int[] offsets = { -1, 0, 1 };

            for (int row = 0; row < cards.GetLength(0); row++)
            {
                for (int col = 0; col < cards.GetLength(1); col++)
                {
                    int r = row;
                    int c = col;

                    cards[r, c] = new Card(r, c)
                    {
                        Location = new Point(c * 24, r * 24),
                        Text = directions[r, c]
                    };
                    pivot.Controls.Add(cards[r, c]);

                    if (r == 2 && c == 1)
                        cards[r, c].Toggle();
                }
            }
            Card.deck = cards;
            Card.Activated = (row, col) =>
            {
                Gravity.X = offsets[col];
                Gravity.Y = offsets[row];
            };

            var randomPivot = new Button()
            {
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.FromArgb(255, 104, 163, 42),
                Font = new Font(QOL.VCROSDMONO, 16f, FontStyle.Regular),
                Text = "🎲",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(32, 32)
            };
            QOL.Align.Bottom.Left(randomPivot, pivotLabel, 1);
            Controls.Add(randomPivot);

            bool randomPivotOn = false;
            var randomPivotTimer = new Timer() { Interval = 1001 };
            randomPivotTimer.Tick += (s, ev) => cards[random.Next(cards.GetLength(0)), random.Next(cards.GetLength(1))].SetActive();

            var copyColor = randomPivot.ForeColor;
            randomPivot.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    randomPivotOn = !randomPivotOn;
                    if (randomPivotOn)
                    {
                        randomPivotTimer.Start();
                        randomPivot.ForeColor = Color.FromArgb(255, 42, 96, 163);
                    }
                    else
                    {
                        randomPivotTimer.Stop();
                        cards[2, 1].SetActive();
                        randomPivot.ForeColor = copyColor;
                    }
                }
                if (ev.Button == MouseButtons.Right && randomPivotTimer.Interval > 1)
                    randomPivotTimer.Interval -= 100;
            };

            foreach (var button in Controls.OfType<Button>())
                button.BackColor = QOL.Colors.SameRGB(20);
        }
    }
}
