using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form
    {
        private Blocks blocks;
        private Gravity gravity;

        private ControlBar controlBar;
        private ToolBarPanel toolBar;
        private GameArea gameArea;
        private Floor floor;

        private CustomPanel PanelOptions;

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            HoodooVoodooBlockMagic();

            blocks = new Blocks();
            blocks.Redraw += () => gameArea.Invalidate();
            blocks.ChangeFocus += block => ChangeFocusedBlock(block);
            blocks.ConfigureBlock += ConfigureBlock;

            gravity = new Gravity();
            gravity.Redraw += () => gameArea.Invalidate();
            gravity.SplitBlock += block => blocks.Split(block);

            toolBar = new ToolBarPanel(gravity);
            PanelOptions = new CustomPanel()
            {
                Height = 160,
                BackColor = QOL.RandomColor(),
            };
            controlBar = new ControlBar(toolBar, PanelOptions);
            gameArea = new GameArea();
            floor = new Floor();

            Controls.Add(PanelOptions);
            Controls.Add(toolBar);
            Controls.Add(gameArea);
            Controls.Add(controlBar);
            Controls.Add(floor);

            gameArea.Dock = DockStyle.Fill;

            controlBar.Height = 54;
            controlBar.Dock = DockStyle.Top;

            toolBar.Size = new Size(ClientSize.Width, 0);
            toolBar.Location = new Point(0, controlBar.Bottom);
            toolBar.Hide();

            floor.Height = 32;
            floor.Dock = DockStyle.Bottom;

            controlBar.ShowToolBar += show =>
            {
                var timer = new Timer { Interval = 10 };
                int target = show ? 98 : 0;

                foreach (var block in Blocks.Stack)
                    block.UserBounds = new Rectangle(
                        gameArea.Location.X,
                            gameArea.Location.Y + (show ? 44 : -54), // i have no earthly idea why the magic numbers are 44 and -54. i never will
                        gameArea.Width, // UPDATE!!: 54 is controlbar's width. 44 remains a complete mystery
                    gameArea.Height - (show ? 98 : 0)); //UPDATE 2: replacing -54 with -controlBar.Height breaks everything. i am mystified


                timer.Tick += (s, ev) =>
                {
                    int step = show ? 5 : -5;
                    toolBar.Height += step;
                    if (toolBar.Height >= target && show || toolBar.Height <= target && !show)
                    {
                        toolBar.Height = target;
                        timer.Stop();
                    }
                    toolBar.Visible = toolBar.Height > 0;
                };

                if (show) toolBar.Visible = true;
                timer.Start();
            };
            gameArea.FocusedBlockChanged += block => ChangeFocusedBlock(block);
            gameArea.SplitBlock += block => blocks.Split(block);

            blocks.Add();
            gravity.Start(Blocks.Stack);
            //KeyMovement();
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

            FormClosing += (s, ev) => //might be leaving some timers running unintentionally
                Application.RemoveMessageFilter(toolBar.weightPanel.WeightDisplayFilter);
        }
        private void ChangeFocusedBlock(Block block)
        {
            if (block == blocks.Target)
                return;

            if (blocks.Target != null)
                blocks.Target.Active = false;
            blocks.Target = block;
            blocks.Target.Active = true;

            toolBar.SetTarget(block);
            gameArea.Invalidate();
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
                        if (blocks.Target.W <= gameArea.Width / 2 && blocks.Target.H <= gameArea.Height / 2)
                        blocks.Target.DoubleSize();
                    gameArea.Invalidate();
                }
                if (ev.KeyCode == Keys.Back)
                {
                    if (toolBar.weightPanel.weightDisplay.ContainsFocus)
                        return;
                    blocks.Remove();
                    gameArea.Invalidate();
                }
            };

            KeyUp += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Space)
                    eventResolved = false;
            };
        }

        private void ConfigureBlock(Block block)
        {
            block.OriginalWeight = block.Weight;
            block.UserBounds = gameArea.ClientRectangle;

            Block.StartPoint = new Point(
                (int)(gameArea.Width / 2 - block.W / 2),
                (int)(gameArea.ClientSize.Height - block.H));
            block.Bounds = new RectangleF(Block.StartPoint, block.Size);

            block.MagneticCore = new Point(
                (int)(gameArea.Width / 2 - block.W / 2),
                (int)(gameArea.Height / 2 - block.H / 2));
        }
    }
}