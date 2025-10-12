using System.Drawing;

namespace Dropper
{
    public class Block
    {
        public RectangleF Bounds { get; set; }
        public bool Dragging { get; set; }
        public float Weight { get; set; }

        public Color? Color { get; set; }
        public Color? BorderColor { get; set; }

        public float BorderWidth { get; set; }
        public SizeF Size { get; set; } = new Size(64, 64);
        public PointF MagneticCore { get; set; }

        public float X => Bounds.X;
        public float Y => Bounds.Y;
        public float W => Size.Width;
        public float H => Size.Height;

        public float L => Bounds.Left;
        public float R => Bounds.Right;
        public float T => Bounds.Top;
        public float B => Bounds.Bottom;

        public Block(RectangleF bounds, float weight, Color? color = null, Color? borderColor = null)
        {
            Bounds = bounds;
            Weight = weight;
            Color = color ?? QOL.RandomColor();
            BorderColor = borderColor ?? QOL.RandomColor();
        }

        public enum GravityMode { Linear, Dynamic, Magnetic }
        public GravityMode Gravity { get; set; } = GravityMode.Linear;
    }
}
