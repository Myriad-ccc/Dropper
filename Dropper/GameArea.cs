using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;

namespace Dropper
{
    public class GameArea : CustomPanel
    {
        public event Action<Block> ActiveBlockChanged;
        private bool debug = false;

        private readonly Random random = new Random();

        public GameArea()
        {
            BackColor = QOL.RGB(20);
            Paint += (s, ev) =>
            {
                var g = ev.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                foreach (var block in Form1.blocks)
                {
                    using (var blockBrush = new SolidBrush(block.Active ? block.ActiveColor : block.InactiveColor))
                        g.FillRectangle(
                            blockBrush,
                            block.Bounds.X,
                            block.Bounds.Y,
                            block.Bounds.Width,
                            block.Bounds.Height);

                    DrawCracks(g, block);

                    using (var borderPen = new Pen(block.Active ? block.ActiveBorderColor : block.InactiveBorderColor, (float)block.BorderWidth))
                        g.DrawRectangle(
                            borderPen,
                            block.Bounds.X,
                            block.Bounds.Y,
                            block.Bounds.Width,
                            block.Bounds.Height);

                    if (debug)
                        using (var brush = new SolidBrush(Color.IndianRed))
                        {
                            var font = new Font(QOL.VCROSDMONO, 20f);
                            var size = TextRenderer.MeasureText(block.Weight.ToString(), font);
                            g.DrawString(
                                $"{block.Weight:F0}",
                                new Font(QOL.VCROSDMONO, 16f),
                                brush,
                                new PointF(
                                    block.Left + size.Width / 8,
                                    block.Top + size.Height / 2));
                        }
                }
            };
            Clicks(Form1.blocks);
        }

        public void Split(Block block)
        {
            //Form1.blocks
        }

        private void Clicks(List<Block> blocks)
        {
            Block draggable = null;
            PointF dragOffset = PointF.Empty;

            MouseDown += (s, ev) =>
                {
                    Block clicked = null;
                    for (int i = 0; i < Form1.blocks.Count; i++)
                    {
                        if (Form1.blocks[i].Bounds.Contains(ev.Location))
                        {
                            clicked = Form1.blocks[i];
                            ActiveBlockChanged?.Invoke(Form1.blocks[i]);
                            break;
                        }
                    }

                    if (ev.Button == MouseButtons.Left)
                    {
                        if (clicked != null)
                        {
                            draggable = clicked;
                            draggable.MouseDragging = true;
                            draggable.ResetVelocity();
                            dragOffset = new PointF(ev.Location.X - draggable.X, ev.Location.Y - draggable.Y);
                        }
                    }

                    if (ev.Button == MouseButtons.Right)
                    {
                        if (clicked != null)
                        {
                            if (clicked.Cracks.Count >= 2)
                            {
                                clicked.Cracks.Clear();
                                Split(clicked);
                                var sp = new SoundPlayer(Properties.Resources.Hector);
                                sp.Play();
                            }
                            else
                                CrackBlock(clicked);
                        }
                    }
                };
            //TODO fix this shit
            /*
             * Make crack split the block into 2 new Form1.blocks and randomly assign block focus
             * Crack after terminal velocity impact
             * Something with an egg
             * MOVE THIS SHIT TO MAIN FORM?? Maybe
             */

            MouseUp += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left && draggable != null)
                {
                    draggable.MouseDragging = false;
                    draggable = null;
                }
            };

            MouseMove += (s, ev) =>
            {
                if (draggable == null) return;

                float dx = ev.X - dragOffset.X;
                float dy = ev.Y - dragOffset.Y;

                draggable.Bounds = new RectangleF(
                    new PointF(dx, dy),
                    draggable.Size);
                draggable.Constrain();

                Invalidate();
            };
        }

        private void CrackBlock(Block block, int cracks = 1)
        {
            for (int i = 0; i < cracks; i++)
            {
                float startX = (float)(block.W * random.NextDouble());
                float startY = (float)(block.H * random.NextDouble());
                float endX = (float)(block.W * random.NextDouble());
                float endY = (float)(block.H * random.NextDouble());

                while (Math.Abs(startX - endX) < 5 || Math.Abs(startY - endY) < 5)
                {
                    endX = (float)(block.W * random.NextDouble());
                    endY = (float)(block.H * random.NextDouble());
                }
                var start = new PointF(startX, startY);
                var end = new PointF(endX, endY);

                float sy = (float)(random.NextDouble() + 1);

                block.Cracks.Add((start, end, sy));
            }
        }

        private void DrawCracks(Graphics g, Block block)
        {
            Bitmap crackBitMap = Properties.Resources.Crack;

            var crackRegion = block.Bounds;
            crackRegion.Inflate(-2, -2);
            g.SetClip(crackRegion);

            var blockState = g.Save();

            g.TranslateTransform(block.Left, block.Top);

            foreach (var (start, end, sy) in block.Cracks)
            {
                var crackState = g.Save();

                g.TranslateTransform(start.X, start.Y);

                float dx = end.X - start.X;
                float dy = end.Y - start.Y;

                float angle = (float)Math.Atan2(dy, dx) * (180.0f / (float)Math.PI);
                float length = (float)Math.Sqrt(dx * dx + dy * dy);
                if (length == 0) continue;

                float scaleX = length / crackBitMap.Width;
                float scaleY = sy;

                g.RotateTransform(angle);
                g.ScaleTransform(scaleX, scaleY);

                g.DrawImage(crackBitMap, 0, -crackBitMap.Height / 2f);

                g.Restore(crackState);
            }
            g.Restore(blockState);

            g.ResetClip();
        }
    }
}