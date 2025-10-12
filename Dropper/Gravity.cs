using System;
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
            if (block.Mass <= 0f) return;
            float deltaTime = updateRate / 1000f; //convert to seconds

            float gx = block.Weight * X * ppm; //total gravity acting on the object
            float gy = block.Weight * Y * ppm;

            block.CalculateDrag();
            float dx = block.DragX; //total drag acting on the object
            float dy = block.DragY;

            float ndx = gx + dx; //net displacement acting on the object
            float ndy = gy + dy;

            float accx = ndx / block.Mass; //a = F / m (https://en.wikipedia.org/wiki/Newton%27s_laws_of_motion)
            float accy = ndy / block.Mass;

            block.VX += accx * deltaTime; //v1 = v0 + a * dt
            block.VY += accy * deltaTime;

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
