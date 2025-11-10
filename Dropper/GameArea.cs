using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Dropper
{
    public class GameArea : CustomPanel
    {
        public event Action<Block> FocusedBlockChanged;
        public event Action<Block> SplitBlock;
        private readonly bool displayWeight = false;

        public GameArea()
        {
            BackColor = QOL.RGB(20);
            Paint += (s, ev) =>
            {
                var g = ev.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                DrawInactiveBlocks(g);
                DrawActiveBlocks(g);
            };
            ConfigureBlockInteractions();
        }

        private void DrawInactiveBlocks(Graphics g)
        {
            foreach (var block in Blocks.Stack)
            {
                if (block.Active) continue;

                using (var blockBrush = new SolidBrush(block.InactiveColor))
                    g.FillRectangle(
                        blockBrush,
                        block.Bounds.X,
                        block.Bounds.Y,
                        block.Bounds.Width,
                        block.Bounds.Height);

                Crack.DrawAll(g, block);

                using (var borderPen = new Pen(block.InactiveBorderColor, (float)block.BorderWidth))
                {
                    borderPen.Alignment = PenAlignment.Inset;
                    g.DrawRectangle(
                        borderPen,
                        block.Bounds.X,
                        block.Bounds.Y,
                        block.Bounds.Width,
                        block.Bounds.Height);
                }

                if (displayWeight)
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
        }
        private void DrawActiveBlocks(Graphics g)
        {
            foreach (var block in Blocks.Stack)
            {
                if (!block.Active) continue;

                using (var blockBrush = new SolidBrush(block.ActiveColor))
                    g.FillRectangle(
                        blockBrush,
                        block.Bounds.X,
                        block.Bounds.Y,
                        block.Bounds.Width,
                        block.Bounds.Height);

                Crack.DrawAll(g, block);

                using (var borderPen = new Pen(block.ActiveBorderColor, (float)block.BorderWidth))
                {
                    borderPen.Alignment = PenAlignment.Inset;
                    g.DrawRectangle(
                        borderPen,
                        block.Bounds.X,
                        block.Bounds.Y,
                        block.Bounds.Width,
                        block.Bounds.Height);

                    if (displayWeight)
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
            }
        }

        private Block draggable = null;
        private PointF dragOffset = PointF.Empty;

        private void ConfigureBlockInteractions()
        {
            MouseDown += (s, ev) =>
                {
                    Block clicked = null;
                    for (int i = 0; i < Blocks.Stack.Count; i++)
                    {
                        if (Blocks.Stack[i].Bounds.Contains(ev.Location))
                        {
                            clicked = Blocks.Stack[i];
                            break;
                        }
                    }

                    if (ev.Button == MouseButtons.Left)
                    {
                        if (clicked != null)
                        {
                            FocusedBlockChanged.Invoke(clicked);
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
                                SplitBlock.Invoke(clicked);
                                //var sp = new SoundPlayer(Properties.Resources.Hector);
                                //sp.Play();
                            }
                            else
                                clicked.Cracks.Add(new Crack(clicked));
                        }
                    }
                };

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

        public void RecalculateDragOffset()
        {
            if (draggable != null)
            {
                var mouse = PointToClient(Cursor.Position);
                dragOffset = new PointF(mouse.X - draggable.X, mouse.Y - draggable.Y);
            }
        }
    }
}