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
                //case Block.GravityMode.LinearBounce:
                //    block.PeakAltitude = block.Y;
                //    block.MinAltitude = block.PeakAltitude;
                //    Bounce(block);
                //    break;
            }
        }

        private void LinearGravity(Block block)
        {
            block.Bounds = new RectangleF(
                new PointF(
                    block.X + block.Weight * X,
                    block.Y + block.Weight * Y),
                block.Size);
        }

        private void DynamicGravity(Block block, int updateRate)
        {
            float deltaTime = updateRate / 1000f;

            float terminalVelocity = (block.Weight * block.Area) / 10;

            block.VX = Math.Min(block.VX + block.Weight * deltaTime * 10 * X, terminalVelocity);
            block.VY = Math.Min(block.VY + block.Weight * deltaTime * 10 * Y, terminalVelocity);

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
                stepX = Math.Min(Math.Abs(dx), block.Weight) * Math.Sign(dx);
            }
            if (block.Y != block.MagneticCore.Y)
            {
                float dy = block.MagneticCore.Y - block.Y;
                stepY = Math.Min(Math.Abs(dy), block.Weight) * Math.Sign(dy);
            }

            block.Bounds = new RectangleF(
                new PointF(
                    block.X + stepX,
                    block.Y + stepY),
                block.Size);
        }

        //private void Bounce(Block block)
        //{
        //    bool canBounce = true;
        //    if (block.MouseDragging)
        //        canBounce = false;

        //    if (block.Y > block.PeakAltitude)
        //        block.PeakAltitude = block.Y;
        //    if (block.MinAltitude < block.Y)
        //        block.MinAltitude = block.Y;
        //    else
        //    {
        //        if (canBounce)
        //        {
        //            block.Bounds = new RectangleF(
        //                new PointF(
        //                    block.X + block.PeakAltitude * 0.5f * X,
        //                    block.Y + block.PeakAltitude * 0.5f * Y),
        //                block.Size);
        //        }
        //        else
        //            LinearGravity(block);
        //    }
        //}
    }
}
