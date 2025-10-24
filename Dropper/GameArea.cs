using System.Collections.Generic;
using System.Drawing;

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
                    block.BorderColor = block.On ? Color.RoyalBlue : Color.DarkGray;
                    using (var blockBrush = new SolidBrush((Color)block.Color))
                        ev.Graphics.FillRectangle(
                            blockBrush,
                            block.Bounds.X,
                            block.Bounds.Y,
                            block.Bounds.Width,
                            block.Bounds.Height);
                    using (var borderPen = new Pen((Color)block.BorderColor, (float)block.BorderWidth))
                        ev.Graphics.DrawRectangle(
                            borderPen,
                            block.Bounds.X,
                            block.Bounds.Y,
                            block.Bounds.Width,
                            block.Bounds.Height);
                }
            };
        }
    }
}
