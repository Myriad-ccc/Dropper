using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form
    {
        public readonly List<Block> blocks = new List<Block>();
        public Block _activeBlock;
        public Block activeBlock
        {
            get => _activeBlock;
            private set
            {
                if (_activeBlock != value)
                {
                    _activeBlock?.Toggle(); // disables previous
                    _activeBlock = value; // sets new
                    _activeBlock?.Toggle(); // enables new
                    ActiveBlockChanged?.Invoke(_activeBlock);
                    area?.gameArea?.Invalidate();
                }
            }
        }
        public event Action<Block> ActiveBlockChanged;

        private readonly Gravity gravity = new Gravity();

        private TitleBar titleBar;
        private Area area;

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            blocks.Add(new Block());
            activeBlock = blocks[0];
            area = new Area(this, gravity)
            {
                Location = new Point(0, titleBar.ClientSize.Height),
                Size = new Size(ClientSize.Width, ClientSize.Height - titleBar.Height)
            };
            Controls.Add(area);
            SetBlockProperties(activeBlock);
            //gravity.Start(blocks);
            Dragging();
            ActiveBlockChanged += block => SetBlockProperties(block);


            QOL.QuickWriteOut(() => area.toolBar.weightSlider.ClientRectangle, titleBar);
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
                if (area == null) return;
                if (area?.toolBar?.weightPanel?.WeightDisplayFilter != null)
                    Application.RemoveMessageFilter(area?.toolBar?.weightPanel?.WeightDisplayFilter);
            };
        }

        private Block draggable;
        private PointF dragOffset;
        private void Dragging()
        {
            if (area == null || area.gameArea == null) return;

            area.gameArea.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    Block clicked = null;
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].Bounds.Contains(ev.Location))
                        {
                            clicked = blocks[i];
                            break;
                        }
                    }

                    if (clicked != null)
                    {
                        draggable = clicked;
                        draggable.Dragging = true;
                        draggable.ResetVelocity();
                        dragOffset = new PointF(ev.Location.X - draggable.X, ev.Location.Y - draggable.Y);
                    }
                }
            };
            area.gameArea.MouseUp += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left && draggable != null)
                {
                    draggable.Dragging = false;
                    draggable = null;
                }
            };
            area.gameArea.MouseMove += (s, ev) =>
            {
                if (draggable == null) return;

                float dx = ev.X - dragOffset.X;
                float dy = ev.Y - dragOffset.Y;

                draggable.Bounds = new RectangleF(
                    new PointF(dx, dy),
                    draggable.Size);
                draggable.Constrain();
                area.gameArea.Invalidate();
            };
        }

        private void SetUserBounds(Block block) => block.UserBounds = area.gameArea.ClientRectangle;
        private void SetMagneticCore(Block block) => block.MagneticCore = new Point(
                (int)(area.gameArea.Width / 2 - block.W / 2),
                (int)(area.gameArea.Height / 2 - block.H / 2));

        private void SetBlockProperties(Block block)
        {
            if (area == null || area.gameArea == null || area.Location == null || area.Size == null || area.Location == Point.Empty || area.Size.IsEmpty) return;
            block.RedrawArea += () => area?.gameArea?.Invalidate();
            SetUserBounds(block);
            SetMagneticCore(block);

            if (block.Bounds == RectangleF.Empty)
            {
                PointF startPoint = new PointF(
                    area.gameArea.Width / 2f - block.W / 2f,
                    area.gameArea.Bottom - block.H);

                block.Bounds = new RectangleF(startPoint, block.Size);
            }
        }
    }
}