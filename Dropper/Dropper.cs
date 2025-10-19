using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form
    {
        private TitleBar titleBar;

        private readonly Block block = new Block();
        private readonly Gravity gravity = new Gravity();

        private CustomPanel area;
        private ToolbarPanel toolBar;
        private CustomPanel floor;
        private Point startPoint;

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            SetScene();
            BlockPhysics();
        }

        private void ConfigureForm()
        {
            Text = "Dropper";
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(1024, 896);
            BackColor = QOL.RGB(20);
            KeyPreview = true;
            DoubleBuffered = true;
            FormClosing += Form1_Closing;
            titleBar = new TitleBar(new Size(Width, 64));
            Controls.Add(titleBar);
            CenterToScreen();
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            if (toolBar.weightPanel.WeightDisplayFilter != null)
                Application.RemoveMessageFilter(toolBar.weightPanel.WeightDisplayFilter);
        }

        private void SetScene()
        {
            area = new CustomPanel
            {
                Location = new Point(0, titleBar.Bottom),
                Size = new Size(ClientSize.Width, ClientSize.Height - titleBar.Height)
            };
            area.Paint += Area_Paint;
            Controls.Add(area);

            toolBar = new ToolbarPanel(block, gravity);
            area.Controls.Add(toolBar);

            floor = new CustomPanel
            {
                Location = new Point(0, area.Height - 32),
                Size = new Size(area.Width, 32),
                BackColor = QOL.RGB(40)
            };
            floor.Paint += (s, ev) =>
            {
                var g = ev.Graphics;
                using (var floorBrush = new SolidBrush(Color.FromArgb(255, 40, 40, 40)))
                    g.FillRectangle(floorBrush, floor.ClientRectangle);

            };
            area.Controls.Add(floor);

            startPoint = new Point((int)(floor.Width / 2 - block.W / 2), (int)(floor.Top - block.H));
            block.Bounds = new RectangleF(startPoint, block.Size);

            block.MagneticCore = new Point(
                (int)(area.Width / 2 - block.W / 2),
                (int)(area.Height / 2 - block.H / 2));

            block.UserBounds = new Rectangle(
                new Point(
                    area.Left,
                toolBar.Bottom),
                new Size(
                    area.Width,
                floor.Top - toolBar.Bottom));
        }

        private void Area_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (var blockBrush = new SolidBrush((Color)block.Color))
                g.FillRectangle(blockBrush, block.Bounds);
            using (var borderPen = new Pen((Color)block.BorderColor, (float)block.BorderWidth))
                g.DrawRectangle(borderPen, block.Bounds.X, block.Bounds.Y, block.Bounds.Width, block.Bounds.Height);
        }

        private void BlockPhysics()
        {
            block.Drag(area);
            CheckGravity(block);
        }

        private void CheckGravity(Block block)
        {
            gravity.Timer.Tick += (s, ev) =>
            {
                if (block.Gravity == Block.GravityMode.Dynamic)
                {
                    toolBar.gravityPanel.displayVX.Text = $"VX: {block.VX:F1}";
                    toolBar.gravityPanel.displayVY.Text = $"VY: {block.VY:F1}";
                }
                if (!block.MouseDragging)
                {
                    gravity.Apply(block);
                    block.ConstrainToArea();
                    area.Invalidate();
                }
            };
            gravity.Timer.Start();
        }
    }
}