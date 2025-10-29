using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class GameArea : CustomPanel
    {
        public event Action<Block> ActiveBlockChanged;
        private bool debug = false;

        private readonly Random random = new Random();

        public GameArea(List<Block> blocks)
        {
            BackColor = QOL.RGB(20);
            Paint += (s, ev) =>
            {
                var g = ev.Graphics;

                foreach (var block in blocks)
                {
                    using (var blockBrush = new SolidBrush(block.Active ? block.ActiveColor : block.InactiveColor))
                        g.FillRectangle(
                            blockBrush,
                            block.Bounds.X,
                            block.Bounds.Y,
                            block.Bounds.Width,
                            block.Bounds.Height);
                    using (var borderPen = new Pen(block.Active ? block.ActiveBorderColor : block.InactiveBorderColor, (float)block.BorderWidth))
                        g.DrawRectangle(
                            borderPen,
                            block.Bounds.X,
                            block.Bounds.Y,
                            block.Bounds.Width,
                            block.Bounds.Height);
                    for (int i = 0; i < block.Cracks; i++)
                    {
                        float angle = 0;

                        switch (i)
                        {
                            case 0: angle = 0; break;
                            case 1: angle = 45; break;
                            case 2: angle = 90; break;
                        }

                        g.TranslateTransform(block.X + block.W / 2, block.Y + block.H / 2); 
                        g.RotateTransform(angle);                                           
                        g.DrawImage(
                            Properties.Resources.Crack,
                            -Properties.Resources.Crack.Width / 2,
                            -Properties.Resources.Crack.Height / 2
                        );
                        g.ResetTransform();
                    }


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
            Clicks(blocks);
        }

        private void Clicks(List<Block> blocks)
        {
            Block draggable = null;
            PointF dragOffset = PointF.Empty;

            MouseDown += (s, ev) =>
                {
                    Block clicked = null;
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].Bounds.Contains(ev.Location))
                        {
                            clicked = blocks[i];
                            ActiveBlockChanged?.Invoke(blocks[i]);
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
                    var crackTimer = new Timer() { Interval = 2000 };
                    crackTimer.Tick += (se, e) =>
                    {
                        clicked.Cracks = 0;
                        clicked.HalveSize();
                        crackTimer.Stop();
                    };
                    if (ev.Button == MouseButtons.Right)
                    {
                        if (clicked != null)
                            clicked.Cracks++;
                        if (clicked.Cracks == 3)
                            crackTimer.Start();                            
                    }
                };
            //TODO fix this shit
            /*
             * Scale cracks depending on size
             * Add crack sfx
             * Give random crack angles & size (still relatively scaled)
             * Change crack texture (rock eyebrow raise gif)
             * Make crack split the block into 2 new blocks and randomly assign block focus
             * Crack after terminal velocity impact
             * Encapsulate crack timer or some shit
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
    }
}