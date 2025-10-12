using System;
using System.Drawing;

namespace Dropper
{
    public class Gravity
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 1;

        public void Apply(Block block)
        {
            switch (block.Gravity)
            {
                case Block.GravityMode.Linear:
                    LinearGravity(block);
                    break;
                case Block.GravityMode.Dynamic:
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
                    block.X + block.Weight * X,
                    block.Y + block.Weight * Y),
                block.Size);
        }

        private void DynamicGravity(Block block)
        {

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
    }
}
