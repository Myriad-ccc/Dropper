using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class PivotPanel : CustomPanel
    {
        private readonly Random random = new Random();

        private readonly Gravity Gravity;

        private bool built;
        private Block targetBlock;

        public PivotPanel(Gravity gravity) => Gravity = gravity;

        public void SetActiveBlock(Block block)
        {
            targetBlock = block ?? throw new ArgumentNullException();
            if (!built)
            {
                Build();
                built = true;
            }
            // flip cards according to new current target block's pivots
        }

        private void Build()
        {
            ForeColor = Color.White;
            BackColor = Color.Transparent;
            Width = 1024;
            Height = 100;
            Paint += (s, ev) =>
            {
                using (var pen = new Pen(BackColor, 1f))
                    ev.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            };

            var pivots = new CustomPanel
            {
                BackColor = QOL.RGB(60),
                Width = Card.CardWidth * 3,
                Height = Card.CardHeight * 3,
            };
            Controls.Add(pivots);

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
                        Location = new Point(c * Card.CardWidth, r * Card.CardHeight),
                        Text = directions[r, c]
                    };
                    pivots.Controls.Add(cards[r, c]);

                    if (r == 2 && c == 1)
                        cards[r, c].Toggle();
                }
            }
            Card.deck = cards;

            var dissipateVX = new Timer() { Interval = 100 };
            dissipateVX.Tick += (s, ev) =>
            {
                if (Math.Abs(targetBlock.VX) <= 0.1)
                {
                    dissipateVX.Stop();
                    targetBlock.VX = 0;
                }
                targetBlock.VX -= targetBlock.VX / 10;
            };
            var dissipateVY = new Timer() { Interval = 100 };
            dissipateVY.Tick += (s, ev) =>
            {
                if (Math.Abs(targetBlock.VY) <= 0.1)
                {
                    dissipateVY.Stop();
                    targetBlock.VY = 0;
                }
                targetBlock.VY -= targetBlock.VY / 10;
            };
            Card.Activated = (row, col) =>
            {
                int nx = offsets[col];
                int ny = offsets[row];

                if (nx != Gravity.X || ny != Gravity.Y)
                {
                    Gravity.X = nx;
                    Gravity.Y = ny;

                    if (nx != 0)
                        dissipateVX.Stop();
                    else
                        dissipateVX.Start();

                    if (ny != 0)
                        dissipateVY.Stop();
                    else
                        dissipateVY.Start();
                }
            };

            Button[] buttons = new CustomButton[Height / Card.CardHeight];
            for (int i = 0; i < buttons.Length; i++)
            {
                int x = i;
                buttons[x] = QOL.GenericControls.Button(16f, null, Color.Gray, new Size(Card.CardWidth, Card.CardHeight));
                buttons[x].Location = new Point(pivots.Right + 1, i * Card.CardHeight);

                switch (x)
                {
                    case 0:
                        buttons[x].Name = "RandomPivot";
                        buttons[x].UseCompatibleTextRendering = false;
                        buttons[x].TextAlign = ContentAlignment.TopCenter;
                        buttons[x].Text = "🎲";
                        buttons[x].ForeColor = Color.Green;
                        var randomPivotTimer = new Timer() { Interval = 1001 };
                        randomPivotTimer.Tick += (s, ev) => cards[random.Next(cards.GetLength(0)), random.Next(cards.GetLength(1))].SetActive();

                        bool randomPivotOn = false;
                        Card copyActiveCard = null;
                        var copyColor = buttons[x].ForeColor;
                        buttons[x].MouseDown += (s, ev) =>
                        {
                            if (ev.Button == MouseButtons.Left)
                            {
                                randomPivotOn = !randomPivotOn;
                                if (randomPivotOn)
                                {
                                    copyActiveCard = Card.GetActive();
                                    randomPivotTimer.Start();
                                    buttons[x].ForeColor = Color.FromArgb(255, 42, 96, 163);
                                }
                                else
                                {
                                    randomPivotTimer.Stop();
                                    randomPivotTimer.Interval = 1001;

                                    copyActiveCard?.SetActive();
                                    buttons[x].ForeColor = copyColor;
                                }
                            }
                            if (ev.Button == MouseButtons.Right && randomPivotTimer.Interval > 1)
                                randomPivotTimer.Interval -= 100;
                        };
                        break;
                    case 1:

                        break;
                    case 2:

                        break;
                }
                Controls.Add(buttons[x]);
            }
        }
    }
}
