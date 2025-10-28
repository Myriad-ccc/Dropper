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
            Drag(blocks);
        }

        private Block draggable;
        private PointF dragOffset;
        private void Drag(List<Block> blocks)
        {
            MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
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

                    if (clicked != null)
                    {
                        draggable = clicked;
                        draggable.MouseDragging = true;
                        draggable.ResetVelocity();
                        dragOffset = new PointF(ev.Location.X - draggable.X, ev.Location.Y - draggable.Y);
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
    }
}