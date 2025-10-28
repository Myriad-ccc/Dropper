using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form //
    {
        public readonly List<Block> blocks = new List<Block>();
        private readonly Gravity gravity = new Gravity();

        private TitleBar titleBar;
        private Area area;

        private Block targetBlock = null;
        public Block TargetBlock
        {
            get => targetBlock;
            set
            {
                if (targetBlock != value)
                {
                    targetBlock.Active = false;
                    targetBlock = value;
                    targetBlock.Active = true;
                    area.SetActiveBlock(value);
                }
            }
        }

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            LoadArea();
            AddBlock(setActive: true);
            area.gameArea.ActiveBlockChanged += block => TargetBlock = block; //area.SetActiveBlock(block);
            HoodooVoodooBlockMagic();
            gravity.Start(blocks);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                AddBlock(setActive: false);
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
                        if (TargetBlock.W / 2 >= 2 && TargetBlock.H / 2 >= 2)
                            targetBlock.HalveSize();
                    }
                    else
                        if (TargetBlock.W <= area.gameArea.Width / 2 && TargetBlock.H <= area.gameArea.Height / 2)
                        TargetBlock.DoubleSize();
                    area.gameArea.Invalidate();
                }
                if (ev.KeyCode == Keys.Back)
                    RemoveBlock();
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
            area.Build(blocks, gravity);
            Controls.Add(area);
        }

        private void AddBlock(bool? setActive = null)
        {
            Block newBlock = new Block();
            blocks.Add(newBlock);
            ConfigureBlock(newBlock);

            if (setActive == true)
            {
                targetBlock = newBlock;
                TargetBlock = newBlock;
                area.SetActiveBlock(newBlock);
            }
        }

        private void RemoveBlock()
        {
            if (blocks.Count < 2) return;
            blocks.Remove(TargetBlock);
            TargetBlock = blocks[blocks.Count - 1];
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
            if (area == null || area.gameArea == null) QOL.WriteOut("idiot");

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