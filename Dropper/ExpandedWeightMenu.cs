using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class ExpandedWeightMenu : CustomPanel
    {
        public event Action<float> WeightChanged;

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
        }
        public ExpandedWeightMenu()
        {
            if (built)
                Paint += (s, ev) =>
                {
                    using (Pen borderPen = new Pen(QOL.RandomColor(), 1f))
                        ev.Graphics.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 2);
                };
        }

        private void Build()
        {
            Visible = false;
            BackColor = Color.Transparent;

            Card[,] buttons = new Card[6, 3];
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
                                        if (Math.Abs(targetBlock.Weight) <= Math.Sqrt(float.MaxValue))
                                        {
                                            targetBlock.Weight = (float)Math.Pow(targetBlock.Weight, 2);
                                            WeightChanged.Invoke(targetBlock.Weight);
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
                                        if (Math.Abs(targetBlock.Weight) <= Math.Pow(float.MaxValue, 1.0f / 3.0f))
                                        {
                                            targetBlock.Weight = (float)Math.Pow(targetBlock.Weight, 3);
                                            WeightChanged.Invoke(targetBlock.Weight);
                                        }
                                    };
                                    break;
                                case 2:
                                    b.Name = "flipWeight";
                                    b.ForeColor = Color.Tomato;
                                    b.Text = "||";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        targetBlock.Weight = targetBlock.Weight * Math.Sign(-1);
                                        WeightChanged.Invoke(targetBlock.Weight);
                                    };
                                    break;
                                case 3:
                                    b.Name = "factorializeWeight";
                                    b.ForeColor = Color.PaleVioletRed;
                                    b.Text = "!";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        if (targetBlock.Weight == (int)targetBlock.Weight && targetBlock.Weight >= 0 && targetBlock.Weight <= 16)
                                        {
                                            targetBlock.Weight = QOL.Factorial((int)targetBlock.Weight);
                                            WeightChanged.Invoke(targetBlock.Weight);
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
                                        targetBlock.Weight = DateTime.Now.Second;
                                        WeightChanged.Invoke(targetBlock.Weight);
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
                                        if (targetBlock.Weight > 0)
                                        {
                                            targetBlock.Weight = (float)Math.Sqrt(targetBlock.Weight);
                                            WeightChanged.Invoke(targetBlock.Weight);
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
                                        if (targetBlock.Weight > 0)
                                        {
                                            targetBlock.Weight = (float)Math.Pow(targetBlock.Weight, 1.0f / 3.0f);
                                            WeightChanged.Invoke(targetBlock.Weight);
                                        }
                                    };
                                    break;
                                case 2:
                                    b.Name = "piWeight";
                                    b.ForeColor = Color.CadetBlue;
                                    b.Text = "π";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        targetBlock.Weight = (float)Math.PI;
                                        WeightChanged.Invoke(targetBlock.Weight);
                                    };
                                    break;
                                case 3:
                                    b.Name = "reciprocateWeight";
                                    b.Font = new Font(b.Font.FontFamily, 16f);
                                    b.ForeColor = Color.BlueViolet;
                                    b.Text = "/";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        if (targetBlock.Weight != 0)
                                            targetBlock.Weight = 1 / targetBlock.Weight;
                                        WeightChanged.Invoke(targetBlock.Weight);
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
                                        if (targetBlock.MouseDragging)
                                        {
                                            secondsDragged += updateRate / 1000.0f;
                                            float multiplier = Math.Max(1.0f, secondsDragged / 2.0f);
                                            targetBlock.Weight += (float)(5.0f / (1000.0f / updateRate) * Math.Pow(multiplier, 3));
                                            WeightChanged.Invoke(targetBlock.Weight);
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
                                        {
                                            timer.Stop();
                                            b.ForeColor = Color.Gold;
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
                                        if (targetBlock.Weight > 0)
                                        {
                                            targetBlock.Weight = (float)Math.Log(targetBlock.Weight);
                                            WeightChanged.Invoke(targetBlock.Weight);
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
                                        if (targetBlock.Weight > 0)
                                        {
                                            targetBlock.Weight = (float)Math.Log10(targetBlock.Weight);
                                            WeightChanged.Invoke(targetBlock.Weight);
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
                                        targetBlock.Weight = (float)Math.E;
                                        WeightChanged.Invoke(targetBlock.Weight);
                                    };
                                    break;
                                case 3:
                                    b.Name = "sumDigitsOfWeight";
                                    b.Font = new Font(b.Font.FontFamily, 16f);
                                    b.ForeColor = Color.ForestGreen;
                                    b.Text = "Σ";
                                    b.MouseClick += (s, ev) =>
                                    {
                                        string blockWeight = Math.Abs(targetBlock.Weight).ToString();
                                        float sum = 0f;
                                        foreach (var ch in blockWeight)
                                            sum += (float)char.GetNumericValue(ch);
                                        targetBlock.Weight = QOL.ValidFloat32(sum) ? sum : targetBlock.Weight;
                                        WeightChanged.Invoke(targetBlock.Weight);
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