using System;
using System.Drawing;

namespace Dropper
{
    public class Block
    {
        public RectangleF Bounds { get; set; } = new RectangleF(PointF.Empty, new SizeF(64, 64));
        public PointF Location => Bounds.Location;
        public SizeF Size => Bounds.Size;

        public bool Active { get; set; } = false;
        public bool MouseDragging { get; set; }
        public Rectangle UserBounds { get; set; }

        public Color ActiveColor { get; set; } = QOL.RGB(50);
        public Color InactiveColor { get; set; } = QOL.RGB(50);
        public Color ActiveBorderColor { get; set; } = Color.IndianRed;
        public Color InactiveBorderColor { get; set; } = Color.Black;

        public float BorderWidth => (float)Math.Pow(W, 1f / 6f);

        public float Weight { get; set; } = 100.0f;
        public float OriginalWeight { get; set; }
        public static Point StartPoint { get; set; }
        public PointF MagneticCore { get; set; }

        public float VX { get; set; } = 0.0f;
        public float VY { get; set; } = 0.0f;

        public float Area => W * H;
        public float TerminalVelocity => (float)Math.Pow(Area * Math.Abs(Weight), 1f / 1.8f);

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

        public enum GravityMode { Linear, Dynamic, Magnetic, None }
        public GravityMode Gravity { get; set; } = GravityMode.Dynamic;

        public void Constrain(int GX = 0, int GY = 0)
        {
            if (UserBounds == Rectangle.Empty) return;
            float nx = X;
            float ny = Y;
            bool bouncingX = false;
            bool bouncingY = false;
            float deltaTime = QOL.GlobalTimerUpdateRate / 1000f;

            if (Left <= UserBounds.Left)
            {
                nx = UserBounds.Left;
                float VTX = (GX * Weight < 0) ? (Math.Abs(Weight) * deltaTime * Dropper.Gravity.GravitationalConstant) : 0;

                if (Special == SpecialMode.Bounce && VX < -VTX && !bouncingX)
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
                float VTX = (GX * Weight > 0) ? (Math.Abs(Weight) * deltaTime * Dropper.Gravity.GravitationalConstant) : 0;

                if (Special == SpecialMode.Bounce && !bouncingX && VX > VTX)
                    VX = -VX * Restituion;
                else
                    ResetVX();
            }

            if (Top <= UserBounds.Top)
            {
                ny = UserBounds.Top;
                float VTY = (GY * Weight < 0) ? (Math.Abs(Weight) * deltaTime * Dropper.Gravity.GravitationalConstant) : 0;

                if (Special == SpecialMode.Bounce && !bouncingY && VY < -VTY)
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
                float VTY = (GY * Weight > 0) ? (Math.Abs(Weight) * deltaTime * Dropper.Gravity.GravitationalConstant) : 0;

                if (Special == SpecialMode.Bounce && !bouncingY && VY > VTY)
                    VY = -VY * Restituion;
                else
                    ResetVY();
            }

            Bounds = new RectangleF(new PointF(nx, ny), Size);
        }

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