using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form
    {
        private Blocks Blocks;
        private readonly Gravity gravity = new Gravity();

        private TitleBar titleBar;
        private Area area;

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            LoadArea();
            Blocks = new Blocks();
            Blocks.ChangeFocus += block => ChangeBlockFocused(block);
            Blocks.ConfigureBlock += ConfigureBlock;
            Blocks.Add();
            gravity.Start(Blocks.Stack);
            //KeyMovement();
            area.gameArea.FocusedBlockChanged += block => ChangeBlockFocused(block);
            HoodooVoodooBlockMagic();
            QOL.QuickWriteOut(() => Blocks.Stack.Count, titleBar);
        }

        private void ChangeBlockFocused(Block block)
        {
            if (block == Blocks.Target)
                return;

            if (Blocks.Target != null)
                Blocks.Target.Active = false;
            Blocks.Target = block;
            Blocks.Target.Active = true;

            area.SetTarget(block);
            area.gameArea.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                Blocks.Add();
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
                        if (Blocks.Target.W / 2 >= 4 && Blocks.Target.H / 2 >= 4)
                            Blocks.Target.HalveSize();
                    }
                    else
                        if (Blocks.Target.W <= area.gameArea.Width / 2 && Blocks.Target.H <= area.gameArea.Height / 2)
                        Blocks.Target.DoubleSize();
                    area.gameArea.Invalidate();
                }
                if (ev.KeyCode == Keys.Back)
                {
                    Blocks.Remove();
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