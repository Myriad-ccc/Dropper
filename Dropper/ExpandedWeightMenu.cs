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
                using (Pen borderPen = new Pen(QOL.Colors.SameRGB(160), 1f))
                    ev.Graphics.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 2);
            };
        }

        private void BuildExpandedWeightMenu()
        {
            Visible = false;
            BackColor = Color.Transparent;

            //var oneWeight = QOL.GenericControls.Button(null, "1", QOL.RandomColor());
            //QOL.Align.Right(oneWeight, zeroWeight, 1);
            //Controls.Add(oneWeight);
            //oneWeight.MouseClick += (s, ev) =>
            //{
            //    Block.Weight = 1;
            //    WeightChanged?.Invoke(Block.Weight);
            //};

            //var millionWeight = QOL.GenericControls.Button(null, "M", QOL.RandomColor());
            //QOL.Align.Right(millionWeight, oneWeight, 1);
            //Controls.Add(millionWeight);
            //millionWeight.MouseClick += (s, ev) =>
            //{
            //    Block.Weight = 10000000;
            //    WeightChanged?.Invoke(Block.Weight);
            //};

            //var absoluteWeight = QOL.GenericControls.Button(12f, "Abs", QOL.RandomColor());
            //QOL.Align.Right(absoluteWeight, millionWeight, 1);
            //Controls.Add(absoluteWeight);
            //absoluteWeight.MouseClick += (s, ev) =>
            //{
            //    if (QOL.ValidFloat32(Math.Abs(Block.Weight)))
            //        Block.Weight = Math.Abs(Block.Weight);    
            //    else
            //        ResetWeight?.Invoke();
            //    WeightChanged?.Invoke(Block.Weight);
            //};

            Button[,] buttons = new Button[11, 3];
            for (int r = 0; r < buttons.GetLength(0); r++)
            {
                for (int c = 0; c < buttons.GetLength(1); c++)
                {
                    int x = r;
                    int y = c;
                    var b = buttons[x, y];

                    b = QOL.GenericControls.Button(null, "-", Color.Gray);
                    b.Location = new Point(x * 24, y * 24);

                    switch (y)
                    {
                        case 0:
                            switch (x)
                            {
                                case 0:
                                    b.Name = "minus5Weight";
                                    b.Text = "-";
                                    b.ForeColor = Color.FromArgb(255, 163, 42, 42);
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = Block.Weight - 5 > int.MinValue ? Block.Weight - 5 : Block.Weight;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 1:
                                    b.Name = "plus5Weight";
                                    b.Text = "+";
                                    b.ForeColor = Color.FromArgb(255, 53, 206, 84);
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = Block.Weight + 5 < int.MaxValue ? Block.Weight + 5 : Block.Weight;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 2:
                                    b.Name = "zeroWeight";
                                    b.Font = new Font(b.Font.FontFamily, 13f);
                                    b.Text = "0";
                                    b.ForeColor = Color.Moccasin;
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = 0;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 3:
                                    
                                    break;
                                case 4:

                                    break;
                                case 5:
                                    
                                    break;
                                case 6:

                                    break;
                                case 7:

                                    break;
                            }
                            break;
                        case 1:
                            switch (x)
                            {
                                case 0:
                                    b.Name = "minus50Weight";
                                    b.Text = "-";
                                    b.ForeColor = Color.FromArgb(255, 127, 33, 33);
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = Block.Weight - 50 > int.MinValue ? Block.Weight - 50 : Block.Weight;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 1:
                                    b.Name = "plus50Weight";
                                    b.Text = "+";
                                    b.ForeColor = Color.FromArgb(255, 47, 181, 73);
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = Block.Weight + 50 < int.MaxValue ? Block.Weight + 50 : Block.Weight;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                            }
                            break;
                        case 2:
                            switch (x)
                            {
                                case 0:
                                    b.Name = "minus500Weight";
                                    b.Text = "-";
                                    b.ForeColor = Color.FromArgb(255, 94, 24, 24);
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = Block.Weight - 500 > int.MinValue ? Block.Weight - 500 : Block.Weight;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
                                    break;
                                case 1:
                                    b.Name = "plus500Weight";
                                    b.Text = "+";
                                    b.ForeColor = Color.FromArgb(255, 42, 163, 68);
                                    b.MouseClick += (s, ev) =>
                                    {
                                        Block.Weight = Block.Weight + 500 < int.MaxValue ? Block.Weight + 500 : Block.Weight;
                                        WeightChanged?.Invoke(Block.Weight);
                                    };
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