using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class Block
    {
        public RectangleF Bounds { get; set; }
        public SizeF Size = new Size(64, 64);

        public bool MouseDragging { get; set; }
        public Rectangle UserBounds { get; set; }

        public Color? Color { get; set; } = QOL.RandomColor();
        public Color? BorderColor { get; set; } = QOL.RandomColor();

        public float BorderWidth { get; set; } = 1f;

        public float Weight { get; set; } = 10.0f;
        public static Point StartPoint { get; set; }
        public PointF MagneticCore { get; set; }

        public float VX { get; set; } = 0.0f;
        public float VY { get; set; } = 0.0f;

        public float Restituion { get; set; } = 0.50f;
        public float PeakVY { get; set; } = 0.0f;

        public float Area => Size.Width * Size.Height;
        public float TerminalVelocity { get; set; }

        public float X => Bounds.X;
        public float Y => Bounds.Y;
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

        public void ConstrainToArea()
        {
            if (UserBounds == null) return;
            float nx = X;
            float ny = Y;

            if (Left < UserBounds.Left)
            {
                nx = UserBounds.Left;
                ResetVX();
            }

            if (Right > UserBounds.Right)
            {
                nx = UserBounds.Right - W;
                ResetVX();
            }

            if (Top < UserBounds.Top)
            {
                ny = UserBounds.Top;
                ResetVY();
            }

            if (Bottom > UserBounds.Bottom)
            {
                ny = UserBounds.Bottom - H;
                ResetVY();
            }

            Bounds = new RectangleF(new PointF(nx, ny), Size);
        }

        public void Drag(Control parent)
        {
            MouseDragging = false;
            PointF cursorPos = Cursor.Position;

            parent.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left && Bounds.Contains(ev.Location))
                {
                    MouseDragging = true;
                    cursorPos = Cursor.Position;
                    ResetVelocity();
                }
            };

            parent.MouseUp += (s, ev) => MouseDragging = false;

            parent.MouseMove += (s, ev) =>
            {
                if (MouseDragging)
                {
                    float deltaX = Cursor.Position.X - cursorPos.X;
                    float deltaY = Cursor.Position.Y - cursorPos.Y;

                    Bounds = new RectangleF(
                        new PointF(
                            X + deltaX,
                            Y + deltaY),
                        Bounds.Size);

                    cursorPos = Cursor.Position;
                    ConstrainToArea();
                    parent.Invalidate();
                }
            };
        }

        public float UpdateTerminalVelocity() => TerminalVelocity = (Area * Weight) / 16;

        public void ResetVelocity()
        {
            ResetVX();
            ResetVY();
        }
        public void ResetVX() => VX = 0;
        public void ResetVY() => VY = 0;

        public enum SpecialMode { Bounce, Split, Crack }
        public SpecialMode Special { get; set; } = SpecialMode.Bounce;
    }
}
