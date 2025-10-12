using System;
using System.Drawing;

namespace Dropper
{
    public class Block
    {
        public RectangleF Bounds { get; set; }
        public bool MouseDragging { get; set; }
        public float Mass { get; set; }
        public SizeF Size = new Size(64, 64);

        public Color? Color { get; set; }
        public Color? BorderColor { get; set; }

        public float BorderWidth { get; set; }
        public PointF MagneticCore { get; set; }

        public float X => Bounds.X;
        public float Y => Bounds.Y;
        public float W => Size.Width;
        public float H => Size.Height;
        public float L => Bounds.Left;
        public float R => Bounds.Right;
        public float T => Bounds.Top;
        public float B => Bounds.Bottom;

        public float VX { get; set; } = 0f;
        public float VY { get; set; } = 0f;

        public float Weight => Mass * Environment.g;
        public float Area => W * H;
        public float DragCoefficient = 1.05f;       //https://en.wikipedia.org/wiki/Drag_coefficient
        public float DragX { get; set; }            //https://en.wikipedia.org/wiki/Drag_equation
        public float DragY { get; set; }            //https://en.wikipedia.org/wiki/Drag_equation

        public Block(RectangleF bounds, float mass, Color? color = null, Color? borderColor = null)
        {
            Bounds = bounds;
            Mass = mass;
            Color = color ?? QOL.RandomColor();
            BorderColor = borderColor ?? QOL.RandomColor();
        }
        public enum GravityMode { Linear, Dynamic, Magnetic }
        public GravityMode Gravity { get; set; } = GravityMode.Linear;

        public void CalculateDrag()
        {
            DragX = -(float)(0.5 * Environment.AirDensity * VX * Math.Abs(VX) * Area * DragCoefficient);
            DragY = -(float)(0.5 * Environment.AirDensity * VY * Math.Abs(VY) * Area * DragCoefficient);
        }
    }
}
