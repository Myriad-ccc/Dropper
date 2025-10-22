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
        public event Action<float> VXChanged;
        public event Action<float> VYChanged;
        public int Update => Timer.Interval;

        public event Action Redraw;

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
                    block.X + block.Weight * X,
                    block.Y + block.Weight * Y),
                block.Size);
        }

        private void DynamicGravity(Block block, int updateRate)
        {
            float deltaTime = updateRate / 1000f;
            block.UpdateTerminalVelocity();

            block.VX += block.Weight * deltaTime * 16 * X;
            block.VY += block.Weight * deltaTime * 16 * Y;

            if (block.Weight > 0)
            {
                block.VX = Math.Min(block.VX, block.TerminalVelocity);
                block.VY = Math.Min(block.VY, block.TerminalVelocity);
            }
            if (block.Weight < 0)
            {
                block.VX = Math.Min(block.VX, -block.TerminalVelocity);
                block.VY = Math.Min(block.VY, -block.TerminalVelocity);
            }

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

        public void CheckGravity(Block block)
        {
            Timer.Tick += (s, ev) =>
            {
                if (!block.MouseDragging)
                {
                    Apply(block);
                    block.ConstrainToArea();

                    if (block.Gravity == Block.GravityMode.Dynamic)
                    {
                        VXChanged?.Invoke(block.VX);
                        VYChanged?.Invoke(block.VY);
                    }
                    Redraw?.Invoke();
                }
            };
            Timer.Start();
        }
    }
}
