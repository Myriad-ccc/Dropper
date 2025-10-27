using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form
    {
        public readonly List<Block> blocks = new List<Block>();
        private readonly Gravity gravity = new Gravity();

        private TitleBar titleBar;
        private Area area;

        private Block activeBlock = null;
        public Block ActiveBlock
        {
            get => activeBlock;
            set
            {
                if (activeBlock != value)
                {
                    activeBlock = value;
                    OnBlockChanged(value);
                }
            }
        }

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            AddBlock();

            KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                    AddBlock();
            };

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
                        if (ActiveBlock.W / 2 >= 2 && ActiveBlock.H / 2 >= 2)
                            ActiveBlock.HalveSize();
                    }
                    else
                        if (ActiveBlock.W <= area.gameArea.Width / 2 && ActiveBlock.H <= area.gameArea.Height / 2)
                        ActiveBlock.DoubleSize();
                    area.gameArea.Invalidate();
                }
            };

            KeyUp += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Space)
                    eventResolved = false;
            };

            gravity.Start(blocks);
        }

        private void AddBlock()
        {
            Block newBlock = new Block();
            blocks.Add(newBlock);
            newBlock.OriginalWeight = newBlock.Weight;
            ActiveBlock = newBlock;
        }
        private void OnBlockChanged(Block block)
        {
            if (area == null)
            {
                area = new Area
                {
                    Size = new Size(ClientSize.Width, ClientSize.Height - titleBar.Height),
                    Location = new Point(0, titleBar.Height)
                };
                area.Build(blocks, gravity);
                Controls.Add(area);
            }

            ConfigureBlock(block);
            area.SetActiveBlock(block);

            foreach (var b in blocks)
                if (b.Active)
                    b.Active = false;
            block.Active = true;

            area.gameArea.Invalidate();
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

            titleBar = new TitleBar
            {
                Width = this.Width,
                Height = 64,
            };
            Controls.Add(titleBar);

            FormClosing += (s, ev) =>
            {
                if (area?.toolBar?.weightPanel?.WeightDisplayFilter != null)
                    Application.RemoveMessageFilter(area?.toolBar?.weightPanel?.WeightDisplayFilter);
            };
        }
        private void ConfigureBlock(Block block)
        {
            if (area == null || area.gameArea == null) return;
            Block.StartPoint = new Point(
                (int)(area.gameArea.Width / 2 - block.W / 2),
                (int)(area.gameArea.ClientSize.Height - block.H));
            block.Bounds = new RectangleF(Block.StartPoint, block.Size);

            block.UserBounds = area.gameArea.ClientRectangle;

            block.MagneticCore = new Point(
                (int)(area.gameArea.Width / 2 - block.W / 2),
                (int)(area.gameArea.Height / 2 - block.H / 2));
        }
    }
}