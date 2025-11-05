using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;

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
        public Color ActiveBorderColor { get; set; } = QOL.RGB(163, 42, 42);
        public Color InactiveBorderColor { get; set; } = QOL.RGB(25);

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

        public bool CanBounce { get; set; } = true;
        public List<Crack> Cracks = new List<Crack>();

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

        public float VTX { get; set; }
        public float VTY { get; set; }
        public void Constrain(int GX = 0, int GY = 0)
        {
            if (UserBounds == Rectangle.Empty) return;
            float nx = X;
            float ny = Y;
            bool bouncingX = false;
            bool bouncingY = false;
            float deltaTime = QOL.GlobalTimerUpdateRate / 1000f;
            float gc = Dropper.Gravity.GravitationalConstant;

            if (Left <= UserBounds.Left)
            {
                if (Math.Abs(VX) >= Math.Abs(TerminalVelocity) && Weight != 0)
                    Cracks.Add(new Crack(this));

                nx = UserBounds.Left;
                VTX = (GX * Weight < 0) ? (Math.Abs(Weight) * deltaTime * gc) : 0;

                if (CanBounce && VX < -VTX && !bouncingX)
                {
                    VX = -VX * Restituion;
                    bouncingX = true;
                }
                else
                    ResetVX();
            }

            if (Right >= UserBounds.Right)
            {
                if (Math.Abs(VX) >= Math.Abs(TerminalVelocity) && Weight != 0)
                    Cracks.Add(new Crack(this));

                nx = UserBounds.Right - W;
                VTX = (GX * Weight > 0) ? (Math.Abs(Weight) * deltaTime * gc) : 0;

                if (CanBounce && !bouncingX && VX > VTX)
                    VX = -VX * Restituion;
                else
                    ResetVX();
            }

            if (Top <= UserBounds.Top)
            {
                if (Math.Abs(VY) >= Math.Abs(TerminalVelocity) && Weight != 0)
                    Cracks.Add(new Crack(this));

                ny = UserBounds.Top;
                VTY = (GY * Weight < 0) ? (Math.Abs(Weight) * deltaTime * gc) : 0;

                if (CanBounce && !bouncingY && VY < -VTY)
                {
                    VY = -VY * Restituion;
                    bouncingY = true;
                }
                else
                    ResetVY();
            }

            if (Bottom >= UserBounds.Bottom)
            {
                if (Math.Abs(VY) == Math.Abs(TerminalVelocity) && Weight != 0)
                    Cracks.Add(new Crack(this));

                ny = UserBounds.Bottom - H;
                VTY = (GY * Weight > 0) ? (Math.Abs(Weight) * deltaTime * gc) : 0;

                if (CanBounce && !bouncingY && VY > VTY)
                    VY = -VY * Restituion;
                else
                    ResetVY();
            }

            Bounds = new RectangleF(new PointF(nx, ny), Size);
        }
        public void ResetVX() => VX = 0;
        public void ResetVY() => VY = 0;
        public void ResetVelocity()
        {
            ResetVX();
            ResetVY();
        }

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

        public Block Copy()
        {
            return new Block()
            {
                Bounds = this.Bounds,
                Active = this.Active,
                MouseDragging = this.MouseDragging,
                UserBounds = this.UserBounds,
                ActiveColor = this.ActiveColor,
                InactiveColor = this.InactiveColor,
                ActiveBorderColor = this.ActiveBorderColor,
                InactiveBorderColor = this.InactiveBorderColor,
                Weight = this.Weight,
                OriginalWeight = this.OriginalWeight,
                MagneticCore = this.MagneticCore,
                VX = this.VX,
                VY = this.VY,
                Restituion = this.Restituion,
                CanBounce = this.CanBounce,
                Cracks = new List<Crack>(this.Cracks),
                Gravity = this.Gravity,
            };
        }

        public static Block Clone(Block source)
        {
            if (source == null) return null;
            return source.Copy();
        }
    }

    public class Crack
    {
        private static readonly Random random = new Random();
        private readonly Bitmap CrackBitMap = Properties.Resources.Crack;

        private float StartX { get; set; }
        private float StartY { get; set; }

        private float EndX { get; set; }
        private float EndY { get; set; }

        private float DX => EndX - StartX;
        private float DY => EndY - StartY;

        private float Angle => (float)Math.Atan2(DY, DX) * (180.0f / (float)Math.PI);
        private float Length => (float)Math.Sqrt(DX * DX + DY * DY);

        private float ScaleX => Length / CrackBitMap.Width;
        private float ScaleY { get; set; }

        private readonly SoundPlayer VineBoom = new SoundPlayer(Properties.Resources.VineBoom);

        public Crack(Block block)
        {
            if (block.Cracks.Count == 3) return;

            //if (block.Active)
            //    VineBoom.Play();

            StartX = (float)(block.W * random.NextDouble());
            StartY = (float)(block.H * random.NextDouble());
            EndX = (float)(block.W * random.NextDouble());
            EndY = (float)(block.H * random.NextDouble());

            while (Math.Abs(StartX - EndX) < block.W / 10 || Math.Abs(StartY - EndY) < block.H / 10)
            {
                EndX = (float)(block.W * random.NextDouble());
                EndY = (float)(block.H * random.NextDouble());
            }
            ScaleY = (float)(random.NextDouble() + 2);
        }

        public static void DrawAll(Graphics g, Block block)
        {
            var crackRegion = block.Bounds;
            crackRegion.Inflate(-2, -2);
            g.SetClip(crackRegion);

            var blockState = g.Save();

            g.TranslateTransform(block.Left, block.Top);

            for (int i = 0; i < block.Cracks.Count; i++)
                block.Cracks[i].Draw(g);

            g.Restore(blockState);

            g.ResetClip();
        }

        private void Draw(Graphics g)
        {
            if (Length == 0) return;

            var crackState = g.Save();

            g.TranslateTransform(StartX, StartY);

            g.RotateTransform(Angle);
            g.ScaleTransform(ScaleX, ScaleY);

            g.DrawImage(CrackBitMap, 0, -CrackBitMap.Height / 2f);

            g.Restore(crackState);
        }
    }

    public class Blocks
    {
        private readonly Random random = new Random();

        public static List<Block> Stack { get; set; } = new List<Block>();
        public Block Target { get; set; }

        public event Action<Block> ChangeFocus;
        public event Action<Block> ConfigureBlock;
        public event Action Redraw;

        public void Add(Block @new = null)
        {
            Block block = @new ?? new Block();
            Stack.Add(block);
            ConfigureBlock.Invoke(block);

            if (Stack.Count == 1)
            {
                block.Active = true;
                ChangeFocus.Invoke(block);
            }
        }

        public void Remove(bool RandomRefocus = false)
        {
            Stack.Remove(Target);
            Target = null;
            var refocused =
                RandomRefocus
                ? (Stack.Count > 0 ? Stack[random.Next(Stack.Count)] : null)
                : (Stack.Count >= 1 ? Stack[Stack.Count - 1] : null);
            ChangeFocus.Invoke(refocused);
        }

        public void Split(Block block)
        {
            var left = Block.Clone(block);
            var right = Block.Clone(block);

            left.Cracks.Clear();
            right.Cracks.Clear();

            if (block.Active)
            {
                int randomBool = random.Next(0, 2);
                left.Active = randomBool == 0;
                right.Active = !left.Active;
                ChangeFocus.Invoke(left.Active ? left : right);
            }
            else left.Active = right.Active = false;

            int index = Stack.IndexOf(block);
            Stack.Remove(block);

            left.Bounds = new RectangleF(
                new PointF(
                    left.Left,
                    left.Top),
                new SizeF(
                    left.W / 2,
                    left.H));
            left.Weight /= 2;
            left.VX /= 2;
            left.VY /= 2;

            right.Bounds = new RectangleF(
                new PointF(
                    right.Left + right.W / 2,
                    right.Top),
                new SizeF(
                    right.W / 2,
                    right.H));
            right.Weight /= 2;
            right.VX /= 2;
            right.VY /= 2;

            Stack.Insert(index, left);
            Stack.Insert(index, right);

            Redraw.Invoke();
        }
    }
}