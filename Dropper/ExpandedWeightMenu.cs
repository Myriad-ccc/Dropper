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

            //var minusWeight = QOL.GenericControls.Button(null, "-", Color.FromArgb(255, 163, 42, 42));
            //minusWeight.Location = new Point(2, 2);
            //minusWeight.MouseClick += (s, ev) =>
            //{
            //    Block.Weight = Block.Weight - 5 > int.MinValue ? Block.Weight - 5 : Block.Weight;
            //    WeightChanged?.Invoke(Block.Weight);
            //};
            //Controls.Add(minusWeight);

            //var plusWeight = QOL.GenericControls.Button(null, "+", Color.Green);
            //QOL.Align.Right(plusWeight, minusWeight, 1);
            //Controls.Add(plusWeight);
            //plusWeight.MouseClick += (s, ev) =>
            //{
            //    Block.Weight = Block.Weight + 5 < int.MaxValue ? Block.Weight + 5 : Block.Weight;
            //    WeightChanged?.Invoke(Block.Weight);
            //};

            //var zeroWeight = QOL.GenericControls.Button(13f, "0", QOL.RandomColor());
            //QOL.Align.Right(zeroWeight, plusWeight, 1);
            //Controls.Add(zeroWeight);
            //zeroWeight.MouseClick += (s, ev) =>
            //{
            //    Block.Weight = 0;
            //    WeightChanged?.Invoke(Block.Weight);
            //};

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
                    int row = r;
                    int col = c;

                    buttons[row, col] = QOL.GenericControls.Button(null, "-", Color.Gray);
                    buttons[row, col].Location = new Point(row * 24, col * 24);
                    Controls.Add(buttons[row, col]);
                }
            }
        }
    }
}