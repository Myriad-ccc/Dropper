using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class GameArea : CustomPanel
    {
        public GameArea(List<Block> blocks)
        {
            BackColor = QOL.RGB(20);
            Paint += (s, ev) =>
            {
                foreach (var block in blocks)
                {
                    block.BorderColor = block.Active ? Color.RoyalBlue : Color.DarkGray;
                    using (var blockBrush = new SolidBrush(block.Color))
                        ev.Graphics.FillRectangle(
                            blockBrush,
                            block.Bounds.X,
                            block.Bounds.Y,
                            block.Bounds.Width,
                            block.Bounds.Height);
                    using (var borderPen = new Pen(block.BorderColor, (float)block.BorderWidth))
                        ev.Graphics.DrawRectangle(
                            borderPen,
                            block.Bounds.X,
                            block.Bounds.Y,
                            block.Bounds.Width,
                            block.Bounds.Height);
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