using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form
    {
        private Blocks blocks;
        private readonly Gravity gravity = new Gravity();

        private TitleBar titleBar;
        private Area area;

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            LoadArea();

            blocks = new Blocks();
            blocks.Redraw += () => area.gameArea.Invalidate();
            blocks.ChangeFocus += block => ChangeBlockFocused(block);
            blocks.ConfigureBlock += ConfigureBlock;
            area.gameArea.SplitBlock += block => blocks.Split(block);
            area.gameArea.FocusedBlockChanged += block => ChangeBlockFocused(block);
            blocks.Add();

            gravity.Start(Blocks.Stack);
            //KeyMovement();
            HoodooVoodooBlockMagic();
        }

        private void ChangeBlockFocused(Block block)
        {
            if (block == blocks.Target)
                return;

            if (blocks.Target != null)
                blocks.Target.Active = false;
            blocks.Target = block;
            blocks.Target.Active = true;

            area.SetTarget(block);
            area.gameArea.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                blocks.Add();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void HoodooVoodooBlockMagic()
        {
            bool spaceDown = false;
            bool shiftDown = false;
            bool eventResolved = false;

            KeyDown += (s, ev) =>
            {
                if (eventResolved) return;

                if (ev.KeyCode == Keys.Space)
                {
                    eventResolved = true;
                    if (ev.Shift)
                    {
                        if (blocks.Target.W / 2 >= 4 && blocks.Target.H / 2 >= 4)
                            blocks.Target.HalveSize();
                    }
                    else
                        if (blocks.Target.W <= area.gameArea.Width / 2 && blocks.Target.H <= area.gameArea.Height / 2)
                        blocks.Target.DoubleSize();
                    area.gameArea.Invalidate();
                }
                if (ev.KeyCode == Keys.Back)
                {
                    blocks.Remove();
                    area.gameArea.Invalidate();
                }
            };

            KeyUp += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Space)
                    eventResolved = false;
            };
        }

        private void LoadArea()
        {
            area = new Area
            {
                Size = new Size(ClientSize.Width, ClientSize.Height - titleBar.Height),
                Location = new Point(0, titleBar.Height)
            };
            area.Build(gravity);
            Controls.Add(area);
        }

        private void ConfigureForm()
        {
            Text = "Dropper";
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(1024, 896);
            BackColor = QOL.RGB(20);
            KeyPreview = true;
            DoubleBuffered = true;
            CenterToScreen();

            titleBar = new TitleBar(new Size(Width, 64));
            Controls.Add(titleBar);

            FormClosing += (s, ev) =>
            {
                if (area?.toolBar?.weightPanel?.WeightDisplayFilter != null)
                    Application.RemoveMessageFilter(area?.toolBar?.weightPanel?.WeightDisplayFilter);
            };
        }
        private void ConfigureBlock(Block block)
        {
            block.OriginalWeight = block.Weight;
            block.UserBounds = area.gameArea.ClientRectangle;

            Block.StartPoint = new Point(
                (int)(area.gameArea.Width / 2 - block.W / 2),
                (int)(area.gameArea.ClientSize.Height - block.H));
            block.Bounds = new RectangleF(Block.StartPoint, block.Size);

            block.MagneticCore = new Point(
                (int)(area.gameArea.Width / 2 - block.W / 2),
                (int)(area.gameArea.Height / 2 - block.H / 2));
        }

        //private bool UpKey = false;
        //private bool LeftKey = false;
        //private bool DownKey = false;
        //private bool RightKey = false;
        //private float Speed => Blocks.Target.Weight / 10.0f;
        //private void KeyMovement()
        //{
        //    KeyDown += (s, ev) =>
        //    {
        //        if (ev.KeyCode == Keys.W && !UpKey) UpKey = true;
        //        if (ev.KeyCode == Keys.A && !LeftKey) LeftKey = true;
        //        if (ev.KeyCode == Keys.S && !DownKey) DownKey = true;
        //        if (ev.KeyCode == Keys.D && !RightKey) RightKey = true;
        //    };
        //    KeyUp += (s, ev) =>
        //    {
        //        if (ev.KeyCode == Keys.W) UpKey = false;
        //        if (ev.KeyCode == Keys.A) LeftKey = false;
        //        if (ev.KeyCode == Keys.S) DownKey = false;
        //        if (ev.KeyCode == Keys.D) RightKey = false;
        //    };

        //    var timer = new Timer() { Interval = 10 };
        //    timer.Tick += (s, ev) =>
        //    {
        //        if (Blocks.Target.MouseDragging) return;
        //        float NX = 0.0f;
        //        float NY = 0.0f;
        //        if (UpKey) NY -= 1;
        //        if (LeftKey) NX -= 1;
        //        if (DownKey) NY += 1;
        //        if (RightKey) NX += 1;

        //        if (NX != 0 || NY != 0)
        //        {
        //            float length = (float)Math.Sqrt(NX * NX + NY * NY);
        //            NX = NX / length * Speed;
        //            NY = NY / length * Speed;
        //        }
        //        Blocks.Target.Constrain();

        //        Blocks.Target.Bounds = new RectangleF(new PointF(
        //            Blocks.Target.X + NX,
        //            Blocks.Target.Y + NY),
        //            Blocks.Target.Size);
        //        area.gameArea.Invalidate();
        //    };
        //    timer.Start();
        //}
    }
}