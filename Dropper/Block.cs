using System;
using System.Drawing;

namespace Dropper
{
    public class Block
    {
        public RectangleF Bounds { get; set; }
        public bool MouseDragging { get; set; }
        public float Mass { get; set; } = 10;
        public SizeF Size = new Size(64, 64);

        public Color? Color { get; set; }
        public Color? BorderColor { get; set; }

        public float BorderWidth { get; set; }
        public PointF MagneticCore { get; set; }

        public float VX { get; set; } = 0f;
        public float VY { get; set; } = 0f;

        public float Area => Size.Width * Size.Height;

        public float X => Bounds.X;
        public float Y => Bounds.Y;
        public float W => Size.Width;
        public float H => Size.Height;
        public float Left => Bounds.Left;
        public float Right => Bounds.Right;
        public float Top => Bounds.Top;
        public float Bottom => Bounds.Bottom;

        public Block(RectangleF bounds, float mass, Color? color = null, Color? borderColor = null)
        {
            Bounds = bounds;
            Mass = mass;
            Color = color ?? QOL.RandomColor();
            BorderColor = borderColor ?? QOL.RandomColor();
        }
        public enum GravityMode { Linear, Dynamic, Magnetic }
        public GravityMode Gravity { get; set; } = GravityMode.Linear;

        public void ResetVelocity(Block block)
        {
            ResetVX(block);
            ResetVY(block);
        }
        public void ResetVX(Block block) => block.VX = 0;
        public void ResetVY(Block block) => block.VY = 0;
    }
}
