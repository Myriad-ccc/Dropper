using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class ExpandedWeightMenu : CustomPanel
    {
        private readonly Block Block;

        public event Action<float> WeightChanged;
        public event Action ResetWeight;

        public ExpandedWeightMenu(Block block)
        {
            Block = block;
            BuildExpandedWeightMenu();

            Paint += (s, ev) =>
            {
                using (Pen borderPen = new Pen(QOL.RandomColor(), 1f))
                    ev.Graphics.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 2);
            };
        }

        private void BuildExpandedWeightMenu()
        {
            Visible = false;
            BackColor = Color.Transparent;

            Button[,] buttons = new Button[6, 3];
            Size = new Size(buttons.GetLength(0) * 24, buttons.GetLength(1) * 24);
            for (int r = 0; r < buttons.GetLength(0); r++)
            {
                for (int c = 0; c < buttons.GetLength(1); c++)
                {
                    int x = r;
                    int y = c;
                    var b = QOL.GenericControls.Button(null, "-", Color.Gray);
                    b.Location = new Point(x * 24, y * 24);

                    switch (y)
                    {
                        case 0:
                            switch (x)
                            {
                                case 0:
                                    b.Name = "squareWeight";
                                    b.Font = new Font(b.Font.FontFamily, 16f);
                                    b.ForeColor = Color.Crimson;
                                    b.Text = "2";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        if (Math.Abs(Block.Weight) <= Math.Sqrt(float.MaxValue))
                                        {
                                            Block.Weight = (float)Math.Pow(Block.Weight, 2);
                                            WeightChanged?.Invoke(Block.Weight);
                                        }
                                    };
                                    break;
                                case 1:
                                    b.Name = "cubeWeight";
                                    b.Font = new Font(b.Font.FontFamily, 16f);
                                    b.ForeColor = Color.Salmon;
                                    b.Text = "3";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        if (Math.Abs(Block.Weight) <= Math.Pow(float.MaxValue, 1.0f / 3.0f))
                                        {
                                            Block.Weight = (float)Math.Pow(Block.Weight, 3);
                                            WeightChanged?.Invoke(Block.Weight);
                                        }
                                    };
                                    break;
                                case 2:
                                    b.Name = "flipWeight";
                                    b.ForeColor = Color.Tomato;
                                    b.Text = "||";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = Block.Weight * Math.Sign(-1);
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 3:
                                    b.Name = "factorializeWeight";
                                    b.ForeColor = Color.PaleVioletRed;
                                    b.Text = "!";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        if (Block.Weight == (int)Block.Weight && Block.Weight >= 0 && Block.Weight <= 16)
                                        {
                                            Block.Weight = QOL.Factorial((int)Block.Weight);
                                            WeightChanged?.Invoke(Block.Weight);
                                        }
                                    };
                                    break;
                                case 4:
                                    b.Name = "clockIncrementingWeight";
                                    b.UseCompatibleTextRendering = true;
                                    b.Font = new Font(b.Font.FontFamily, 14f);
                                    b.ForeColor = Color.Beige;
                                    b.Text = "⏱️";
                                    bool on = false;
                                    var timer = new Timer() { Interval = 1000 };
                                    timer.Tick += (s, ev) =>
                                    {
                                        Block.Weight = DateTime.Now.Second;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    b.MouseClick += (s, ev) =>
                                    {
                                        on = !on;
                                        if (on)
                                        {
                                            b.ForeColor = Color.OrangeRed;
                                            timer.Start();
                                        }
                                        else
                                        {
                                            b.ForeColor = Color.Beige;
                                            timer.Stop();
                                        }
                                    };
                                    break;
                                case 5:

                                    break;
                                case 6:

                                    break;
                                case 7:

                                    break;
                                case 8:

                                    break;
                                case 9:

                                case 10:

                                    break;
                            }
                            break;
                        case 1:
                            switch (x)
                            {
                                case 0:
                                    b.Name = "squareRootWeight";
                                    b.ForeColor = Color.DarkSlateBlue;
                                    b.Text = "√";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        if (Block.Weight > 0)
                                        {
                                            Block.Weight = (float)Math.Sqrt(Block.Weight);
                                            WeightChanged?.Invoke(Block.Weight);
                                        }
                                    };
                                    break;
                                case 1:
                                    b.Name = "cubeRootWeight";
                                    b.Font = new Font(b.Font.FontFamily, 16f);
                                    b.ForeColor = Color.Silver;
                                    b.Text = "∛";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        if (Block.Weight > 0)
                                        {
                                            Block.Weight = (float)Math.Pow(Block.Weight, 1.0f / 3.0f);
                                            WeightChanged?.Invoke(Block.Weight);
                                        }
                                    };
                                    break;
                                case 2:
                                    b.Name = "piWeight";
                                    b.ForeColor = Color.CadetBlue;
                                    b.Text = "π";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = (float)Math.PI;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 3:
                                    b.Name = "reciprocateWeight";
                                    b.Font = new Font(b.Font.FontFamily, 16f);
                                    b.ForeColor = Color.BlueViolet;
                                    b.Text = "/";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = 1 / Block.Weight;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 4:
                                    b.Name = "accumulateWeight";
                                    b.ForeColor = Color.Gold;
                                    b.Text = "⌖";

                                    float secondsDragged = 0;
                                    float weightPerSecond = 0;
                                    int updateRate = 100;
                                    var timer = new Timer() { Interval = updateRate };
                                    timer.Tick += (s, ev) =>
                                    {
                                        if (Block.MouseDragging)
                                        {
                                            secondsDragged += updateRate / 1000.0f;
                                            float multiplier = Math.Max(1.0f, secondsDragged / 2.0f);
                                            Block.Weight += (float)(5.0f / (1000.0f / updateRate) * Math.Pow(multiplier, 3));
                                            WeightChanged?.Invoke(Block.Weight);
                                        }
                                        else
                                            secondsDragged = 0.0f;
                                    };
                                    bool accumulatorOn = false;
                                    b.MouseClick += (s, ev) =>
                                    {
                                        accumulatorOn = !accumulatorOn;
                                        if (accumulatorOn)
                                        {
                                            timer.Start();
                                            b.ForeColor = Color.LightSeaGreen;
                                        }
                                        else
                                            b.ForeColor = Color.Gold;
                                    };

                                    break;
                                case 5:

                                    break;
                                case 6:

                                    break;
                                case 7:

                                    break;
                                case 8:

                                    break;
                                case 9:

                                    break;
                                case 10:

                                    break;
                            }
                            break;
                        case 2:
                            switch (x)
                            {
                                case 0:
                                    b.Name = "logWeight";
                                    b.Font = new Font(b.Font.FontFamily, 8f);
                                    b.ForeColor = Color.DeepPink;
                                    b.Text = "ln";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        if (Block.Weight > 0)
                                        {
                                            Block.Weight = (float)Math.Log(Block.Weight);
                                            WeightChanged?.Invoke(Block.Weight);
                                        }
                                    };
                                    break;
                                case 1:
                                    b.Name = "naturalLogWeight";
                                    b.Font = new Font(b.Font.FontFamily, 8f);
                                    b.ForeColor = Color.DarkTurquoise;
                                    b.Text = "log";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        if (Block.Weight > 0)
                                        {
                                            Block.Weight = (float)Math.Log10(Block.Weight);
                                            WeightChanged?.Invoke(Block.Weight);
                                        }
                                    };
                                    break;
                                case 2:
                                    b.Name = "eulerWeight";
                                    b.Font = new Font(b.Font.FontFamily, 16f);
                                    b.ForeColor = Color.LightYellow;
                                    b.Text = "e";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = (float)Math.E;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 3:
                                    b.Name = "sumDigitsOfWeight";
                                    b.Font = new Font(b.Font.FontFamily, 16f);
                                    b.ForeColor = Color.ForestGreen;
                                    b.Text = "Σ";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        string blockWeight = Math.Abs(Block.Weight).ToString();
                                        float sum = 0f;
                                        foreach (var ch in blockWeight)
                                            sum += (float)char.GetNumericValue(ch);
                                        Block.Weight = QOL.ValidFloat32(sum) ? sum : Block.Weight;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 4:

                                    break;
                                case 5:

                                    break;
                                case 6:

                                    break;
                                case 7:

                                    break;
                                case 8:

                                    break;
                                case 9:

                                    break;
                                case 10:

                                    break;
                            }
                            break;
                    }

                    Controls.Add(b);
                }
            }
        }
    }
}