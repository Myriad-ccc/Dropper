using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class Gravity
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 1;
        public Timer Timer { get; set; } = new Timer() { Interval = 10 };
        public int Update => Timer.Interval;

        public void Apply(Block block)
        {
            switch (block.Gravity)
            {
                case Block.GravityMode.Linear:
                    LinearGravity(block);
                    break;
                case Block.GravityMode.Dynamic:
                    DynamicGravity(block, Update);
                    break;
                case Block.GravityMode.Magnetic:
                    MagneticGravity(block);
                    break;
            }
        }

        private void LinearGravity(Block block)
        {
            block.Bounds = new RectangleF(
                new PointF(
                    block.X + block.Mass * X,
                    block.Y + block.Mass * Y),
                block.Size);
        }

        private const float ppm = 64f;
        private void DynamicGravity(Block block, int updateRate)
        {
            float deltaTime = updateRate / 1000f; // in seconds

            float gravityX = Environment.g * X * ppm; // in pixels
            float gravityY = Environment.g * Y * ppm;

            float VXm = block.VX / ppm;
            float VYm = block.VY / ppm;

            float AreaM = block.Area / (ppm * ppm);

            float dragX = (float)(0.5 * Environment.AirDensity * block.DragCoefficient * AreaM * VXm * Math.Abs(VXm));
            float dragY = (float)(0.5 * Environment.AirDensity * block.DragCoefficient * AreaM * VYm * Math.Abs(VYm));

            float dragAccelerationXm = -Math.Sign(VXm) * (Math.Abs(dragX) / block.Mass);
            float dragAccelerationYm = -Math.Sign(VYm) * (Math.Abs(dragY) / block.Mass);

            float dragAccelerationXp = dragAccelerationXm / ppm;
            float dragAccelerationYp = dragAccelerationYm / ppm;

            float netAccelerationX = gravityX + dragAccelerationXp;
            float netAccelerationY = gravityY + dragAccelerationYp;

            block.VX += netAccelerationX * deltaTime;
            block.VY += netAccelerationY * deltaTime;

            block.Bounds = new RectangleF(
                new PointF(
                    block.X + block.VX * deltaTime,
                    block.Y + block.VY * deltaTime), 
                block.Size);
        }

        private void MagneticGravity(Block block)
        {
            float stepX = 0f;
            float stepY = 0f;
            if (block.X != block.MagneticCore.X)
            {
                float dx = block.MagneticCore.X - block.X;
                stepX = Math.Min(Math.Abs(dx), block.Mass) * Math.Sign(dx);
            }
            if (block.Y != block.MagneticCore.Y)
            {
                float dy = block.MagneticCore.Y - block.Y;
                stepY = Math.Min(Math.Abs(dy), block.Mass) * Math.Sign(dy);
            }

            block.Bounds = new RectangleF(
                new PointF(
                    block.X + stepX, 
                    block.Y + stepY), 
                block.Size);
        }
    }
}
