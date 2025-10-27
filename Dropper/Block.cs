using System;
using System.Drawing;

namespace Dropper
{
    public class Block
    {
        public RectangleF Bounds { get; set; } = new RectangleF(PointF.Empty, new SizeF(64, 64));
        public PointF Location => Bounds.Location;
        public SizeF Size => Bounds.Size;

        public bool Active { get; set; } = true;
        public bool MouseDragging { get; set; }
        public Rectangle UserBounds { get; set; }

        public Color Color { get; set; } = QOL.RandomColor();
        public Color BorderColor { get; set; } = Color.RoyalBlue;

        public float BorderWidth { get; set; } = 3f;

        public float Weight { get; set; } = 100.0f;
        public float OriginalWeight { get; set; }
        public static Point StartPoint { get; set; }
        public PointF MagneticCore { get; set; }

        public float VX { get; set; } = 0.0f;
        public float VY { get; set; } = 0.0f;

        public float Area => W * H;
        public float TerminalVelocity { get; set; }

        public float Restituion { get; set; } = 0.70f;

        public float X => Location.X;
        public float Y => Location.Y;
        public float W => Size.Width;
        public float H => Size.Height;
        public float Left => Bounds.Left;
        public float Right => Bounds.Right;
        public float Top => Bounds.Top;
        public float Bottom => Bounds.Bottom;

        public Block() { }
        public Block(RectangleF bounds, Color color, Color borderColor)
        {
            Bounds = bounds;
            Color = color;
            BorderColor = borderColor;
        }

        public enum GravityMode { Linear, Dynamic, Magnetic }
        public GravityMode Gravity { get; set; } = GravityMode.Dynamic;

        public void Constrain()
        {
            if (UserBounds == Rectangle.Empty) return;
            float nx = X;
            float ny = Y;
            bool bouncingX = false;
            bool bouncingY = false;

            if (Left <= UserBounds.Left)
            {
                nx = UserBounds.Left;
                if (Special == SpecialMode.Bounce && VX < 0)
                {
                    VX = -VX * Restituion;
                    bouncingX = true;
                }
                else
                    ResetVX();
            }

            if (Right >= UserBounds.Right)
            {
                nx = UserBounds.Right - W;
                if (Special == SpecialMode.Bounce && VX > 0 && !bouncingX)
                    VX = -VX * Restituion;
                else
                    ResetVX();
            }

            if (Top <= UserBounds.Top)
            {
                ny = UserBounds.Top;
                if (Special == SpecialMode.Bounce && VY < 0)
                {
                    VY = -VY * Restituion;
                    bouncingY = true;
                }
                else
                    ResetVY();
            }

            if (Bottom >= UserBounds.Bottom)
            {
                ny = UserBounds.Bottom - H;
                if (Special == SpecialMode.Bounce && VY > 0 && !bouncingY)
                    VY = -VY * Restituion;
                else
                    ResetVY();
            }

            Bounds = new RectangleF(new PointF(nx, ny), Size);
        }

        public float UpdateTerminalVelocity() => TerminalVelocity = (Area * Math.Abs(Weight)) / 16;

        public void ResetVelocity()
        {
            ResetVX();
            ResetVY();
        }
        public void ResetVX() => VX = 0;
        public void ResetVY() => VY = 0;

        public enum SpecialMode { Bounce, Split, Crack }
        public SpecialMode Special { get; set; } = SpecialMode.Bounce;

        public void DoubleSize()
        {
            float nw = W * 2f;
            float nh = H * 2f;
            float nx = X - W / 2f;
            float ny = Y - H / 2f;

            Bounds = new RectangleF(new PointF(nx, ny), new SizeF(nw, nh));
        }

        public void HalveSize()
        {
            float nw = W / 2f;
            float nh = H / 2f;
            float nx = X + nw / 2f;
            float ny = Y + nh / 2f;

            Bounds = new RectangleF(new PointF(nx, ny), new SizeF(nw, nh));
        }
    }
}