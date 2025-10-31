using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class Gravity
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 1;

        public static readonly float GravitationalConstant = 16f;

        public Timer Timer { get; set; } = new Timer() { Interval = QOL.GlobalTimerUpdateRate };
        public event Action<float> VXChanged;
        public event Action<float> VYChanged;

        public event Action Redraw;

        public void Apply(Block block)
        {
            switch (block.Gravity)
            {
                case Block.GravityMode.Linear:
                    LinearGravity(block);
                    break;
                case Block.GravityMode.Dynamic:
                    DynamicGravity(block);
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
            float deltaTime = QOL.GlobalTimerUpdateRate / 1000f;

            block.VX += block.Weight * deltaTime * GravitationalConstant * X;
            block.VY += block.Weight * deltaTime * GravitationalConstant * Y;

            if (block.Weight > 0)
            {
                block.VX = Math.Min(block.VX, block.TerminalVelocity);
                block.VY = Math.Min(block.VY, block.TerminalVelocity);
            }
            if (block.Weight < 0)
            {
                block.VX = Math.Max(block.VX, -block.TerminalVelocity);
                block.VY = Math.Max(block.VY, -block.TerminalVelocity);
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

        public void Start(List<Block> blocks)
        {
            Timer.Tick += (s, ev) =>
            {
                if (blocks == null || blocks.Count == 0) return;

                foreach (Block block in blocks)
                {
                    if (!block.MouseDragging) //&& block.Active
                    {
                        Apply(block);
                        block.Constrain(X, Y);
                    }
                }

                Block active = blocks.Find(x => x.Active);
                if (active != null && active.Gravity == Block.GravityMode.Dynamic)
                {
                    VXChanged?.Invoke(active.VX);
                    VYChanged?.Invoke(active.VY);
                }
                else
                {
                    VXChanged?.Invoke(0f);
                    VYChanged?.Invoke(0f);
                }
                Redraw.Invoke();
            };
            Timer.Start();
        }
    }
}