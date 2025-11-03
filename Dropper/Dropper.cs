using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form
    {
        private Blocks blocks;
        private readonly Gravity gravity = new Gravity();

        private ControlBar controlBar;
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

            gravity.SplitBlock += block => blocks.Split(block);
            area.gameArea.SplitBlock += block => blocks.Split(block);
            area.gameArea.FocusedBlockChanged += block => ChangeBlockFocused(block);
            blocks.Add();

            gravity.Start(Blocks.Stack);
            //KeyMovement();
            HoodooVoodooBlockMagic();
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

            controlBar = new ControlBar();
            controlBar.Size = new Size(ClientSize.Width, 198);
            Controls.Add(controlBar);

            FormClosing += (s, ev) =>
            {
                if (area?.toolBar?.weightPanel?.WeightDisplayFilter != null)
                    Application.RemoveMessageFilter(area?.toolBar?.weightPanel?.WeightDisplayFilter);
            };
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
                    if (!area.toolBar.weightPanel.weightDisplay.ContainsFocus)
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
                Size = new Size(ClientSize.Width, ClientSize.Height - controlBar.Height),
                Location = new Point(0, controlBar.Height)
            };
            area.Build(gravity);
            Controls.Add(area);
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
    }
}