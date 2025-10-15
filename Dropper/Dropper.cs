using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form
    {
        private readonly Random random = new Random();

        private Panel borderBox;
        private static Color TitleColor = QOL.RandomColor();
        private static Color ShadowTitleColor = QOL.RandomColor();

        private CustomPanel area;
        private ToolbarPanel toolBar;
        private CustomPanel floor;
        private Point startPoint;
        private Dictionary<string, int> userBounds;

        private Block block;

        private readonly Gravity gravity = new Gravity();

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            block = new Block();
            SetScene();
            BlockPhysics();
        }

        private void ConfigureForm()
        {
            Text = "Dropper";
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(1024, 896);
            BackColor = Color.FromArgb(255, 20, 20, 20);
            KeyPreview = true;
            DoubleBuffered = true;
            FormClosing += Form1_Closing;
            BorderMenu();
            CenterToScreen();
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            if (toolBar.weightPanel.WeightDisplayFilter != null)
                Application.RemoveMessageFilter(toolBar.weightPanel.WeightDisplayFilter);
        }

        private void BorderMenu()
        {
            borderBox = new Panel()
            {
                Location = new Point(0, 0),
                Size = new Size(Width, 64)
            };
            Controls.Add(borderBox);

            Button closingButton = new Button()
            {
                Size = new Size(56, 56),
                ForeColor = Color.FromArgb(255, 163, 42, 42),
                Font = new Font(QOL.VCROSDMONO, 20f),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "✖",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
            };
            closingButton.Location = new Point(Width - 4 - closingButton.Width, 4);
            closingButton.MouseClick += (s, ev) => Close();
            borderBox.Controls.Add(closingButton);

            Button minimizeButton = new Button()
            {
                Size = new Size(56, 56),
                ForeColor = Color.FromArgb(255, 42, 163, 150),
                Font = new Font(QOL.VCROSDMONO, 20f),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "―",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
            };
            minimizeButton.Location = new Point(closingButton.Left - 16 - minimizeButton.Width, 4);
            minimizeButton.MouseClick += (s, ev) => WindowState = FormWindowState.Minimized;
            borderBox.Controls.Add(minimizeButton);

            borderBox.Paint += (s, ev) =>
            {
                Graphics g = ev.Graphics;

                using (SolidBrush borderPen = new SolidBrush(Color.FromArgb(255, 35, 35, 35)))
                    g.FillRectangle(borderPen, borderBox.ClientRectangle);

                using (var shadowBrush = new SolidBrush(TitleColor))
                using (var mainBrush = new SolidBrush(ShadowTitleColor))
                using (var font = new Font(QOL.VCROSDMONO, 32f))
                {
                    g.DrawString(Text, font, shadowBrush, new Point(10, 10));
                    g.DrawString(Text, font, mainBrush, new Point(9, 9));
                }
            };
            DragControl(borderBox, true);
        }

        public static void DragControl(Control control, bool parent)
        {
            var target = parent ? control.Parent : control;

            bool MouseDragging = false;
            Point cursorPos = Cursor.Position;

            control.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    MouseDragging = true;
                    cursorPos = Cursor.Position;
                }
                if (ev.Button == MouseButtons.Right && !MouseDragging)
                {
                    TitleColor = QOL.RandomColor();
                    ShadowTitleColor = QOL.RandomColor();
                    control.Invalidate();
                }
            };

            control.MouseUp += (s, ev) => MouseDragging = false;

            control.MouseMove += (s, ev) =>
            {
                if (MouseDragging)
                {
                    int deltaX = Cursor.Position.X - cursorPos.X;
                    int deltaY = Cursor.Position.Y - cursorPos.Y;

                    target.Left += deltaX;
                    target.Top += deltaY;

                    cursorPos = Cursor.Position;

                    if (parent)
                        target.Invalidate();
                    else
                        target.Parent.Invalidate();
                }
            };
        }

        private void SetScene()
        {
            area = new CustomPanel
            {
                Location = new Point(0, borderBox.Bottom),
                Size = new Size(ClientSize.Width, ClientSize.Height - borderBox.Height)
            };
            area.Paint += Area_Paint;
            Controls.Add(area);
            block.MagneticCore = new Point(
                (int)(area.Width / 2 - block.W / 2),
                (int)(area.Height / 2 - block.H / 2));

            toolBar = new ToolbarPanel(block, gravity)
            {
                Location = new Point(),
                Size = new Size(area.Width, 96),
            };
            toolBar.Paint += (s, ev) =>
            {
                var g = ev.Graphics;
                using (var toolBarBrush = new SolidBrush(Color.FromArgb(255, 50, 50, 50)))
                    g.FillRectangle(toolBarBrush, toolBar.ClientRectangle);
            };
            area.Controls.Add(toolBar);

            floor = new CustomPanel
            {
                Location = new Point(0, area.Height - 32),
                Size = new Size(area.Width, 32)
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

            userBounds = new Dictionary<string, int>
            {
                ["Left"] = area.Left,
                ["Top"] = toolBar.Bottom,
                ["Right"] = area.Right,
                ["Bottom"] = floor.Top
            };
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
            DragBlock(block, area);
            CheckGravity(block);
        }

        public void DragBlock(Block block, Control parent)
        {
            block.MouseDragging = false;
            PointF cursorPos = Cursor.Position;

            parent.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left && block.Bounds.Contains(ev.Location))
                {
                    block.MouseDragging = true;
                    cursorPos = Cursor.Position;
                    block.ResetVelocity();
                }
            };

            parent.MouseUp += (s, ev) => block.MouseDragging = false;

            parent.MouseMove += (s, ev) =>
            {
                if (block.MouseDragging)
                {
                    float deltaX = Cursor.Position.X - cursorPos.X;
                    float deltaY = Cursor.Position.Y - cursorPos.Y;

                    block.Bounds = new RectangleF(
                        new PointF(
                            block.X + deltaX,
                            block.Y + deltaY),
                        block.Bounds.Size);

                    cursorPos = Cursor.Position;
                    ConstrainToArea(block);
                    parent.Invalidate();
                }
            };
        }

        private void ConstrainToArea(Block block)
        {
            float nx = block.X;
            float ny = block.Y;

            if (block.Left < userBounds["Left"])
            {
                nx = userBounds["Left"];
                block.ResetVX();
            }

            if (block.Right > userBounds["Right"])
            {
                nx = userBounds["Right"] - block.W;
                block.ResetVX();
            }

            if (block.Top < userBounds["Top"])
            {
                ny = userBounds["Top"];
                block.ResetVY();
            }

            if (block.Bottom > userBounds["Bottom"])
            {
                ny = userBounds["Bottom"] - block.H;
                block.ResetVY();
            }

            block.Bounds = new RectangleF(new PointF(nx, ny), block.Size);
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
                    ConstrainToArea(block);
                    area.Invalidate();
                }
            };
            gravity.Timer.Start();
        }
    }
}
