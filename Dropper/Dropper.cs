using System;
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
        private CustomPanel toolBar;
        private CustomPanel floor;

        private Block block;
        private Point startPoint;
        private Timer gravityTimer;

        private ClickFilter weightDisplayFilter;

        private Gravity gravity = new Gravity();

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            AddBlock();
            SetScene();
            BlockPhysics();
            ToolBar();
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

        private void AddBlock()
        {
            block = new Block(RectangleF.Empty, 10);
            block.Gravity = Block.GravityMode.Linear;
        }

        private void Form1_Closing(object sender, EventArgs e)
        {
            if (weightDisplayFilter != null)
                Application.RemoveMessageFilter(weightDisplayFilter);
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


            int gap = 16;


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
            minimizeButton.Location = new Point(closingButton.Left - gap - minimizeButton.Width, 4);
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

            bool dragging = false;
            Point cursorPos = Cursor.Position;

            control.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    dragging = true;
                    cursorPos = Cursor.Position;
                }
                if (ev.Button == MouseButtons.Right && !dragging)
                {
                    TitleColor = QOL.RandomColor();
                    ShadowTitleColor = QOL.RandomColor();
                    control.Invalidate();
                }
            };

            control.MouseUp += (s, ev) => dragging = false;

            control.MouseMove += (s, ev) =>
            {
                if (dragging)
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

            toolBar = new CustomPanel
            {
                Location = new Point(0, 0),
                Size = new Size(area.Width, 96)
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
            block.Dragging = false;
            PointF cursorPos = Cursor.Position;

            parent.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left && block.Bounds.Contains(ev.Location))
                {
                    block.Dragging = true;
                    cursorPos = Cursor.Position;
                }
            };

            parent.MouseUp += (s, ev) => block.Dragging = false;

            parent.MouseMove += (s, ev) =>
            {
                if (block.Dragging)
                {
                    float deltaX = Cursor.Position.X - cursorPos.X;
                    float deltaY = Cursor.Position.Y - cursorPos.Y;

                    block.Bounds = new RectangleF(
                        new PointF(
                            block.X + deltaX,
                            block.Y + deltaY),
                        block.Bounds.Size);

                    cursorPos = Cursor.Position;
                    ConstrainToArea(block, parent);
                    parent.Invalidate();
                }
            };
        }

        private void ConstrainToArea(Block block, Control area)
        {
            float cx = block.X;
            float cy = block.Y;
            float bw = block.BorderWidth;
            float w = block.W;
            float h = block.H;

            if (cx < area.ClientRectangle.Left)
                cx = area.ClientRectangle.Left;

            if (cx + w + bw > area.ClientRectangle.Right)
                cx = area.ClientRectangle.Right - w - bw;

            if (cy < toolBar.Bottom)
                cy = toolBar.Bottom;

            if (cy + h > floor.Top)
                cy = floor.Top - h;

            block.Bounds = new RectangleF(new PointF(cx, cy), block.Size);
        }

        private void CheckGravity(Block block)
        {
            gravityTimer = new Timer() { Interval = 10 };
            gravityTimer.Tick += (s, ev) =>
            {
                if (!block.Dragging)
                {
                    gravity.Apply(block);
                    ConstrainToArea(block, area);
                    area.Invalidate();
                }
            };
            gravityTimer.Start();
        }

        private void ToolBar()
        {
            var weightOptions = new CustomPanel()
            {
                ForeColor = Color.Transparent,
                BackColor = QOL.Colors.SameRGB(35),
                Width = toolBar.Width,
                Height = 24,
                Location = new Point(toolBar.Left, toolBar.Top)
            };
            toolBar.Controls.Add(weightOptions);
            weightOptions.Paint += (s, ev) =>
            {
                using (var pen = new Pen(BackColor, 1f))
                    ev.Graphics.DrawRectangle(pen, 0, 0, weightOptions.Width - 1, weightOptions.Height - 1);
            };

            var weightLabel = new Label()
            {
                Tag = "ToolBarInfoLabel",
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 16f),
                Text = "Weight:",
                Height = weightOptions.Height,
            };
            weightOptions.Controls.Add(weightLabel);

            float originalWeight = block.Weight;
            var weightDisplay = new TextBox()
            {
                Tag = "ToolBarInfoLabel",
                Anchor = AnchorStyles.Left,
                BackColor = QOL.Colors.SameRGB(100),
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 16f),
                Text = $"{block.Weight:F1}",
                Width = weightLabel.Width,
                BorderStyle = BorderStyle.None,
                TabStop = false,
            };
            QOL.Align.Right(weightDisplay, weightLabel, 6);
            weightDisplay.TextChanged += (s, ev) =>
            {
                if (float.TryParse(weightDisplay.Text, out float newWeight))
                    block.Weight = newWeight;
                else block.Weight = originalWeight;
            };
            weightDisplay.LostFocus += (s, ev) =>
            {
                if (string.IsNullOrEmpty(weightDisplay.Text)
                || !float.TryParse(weightDisplay.Text, out _))
                {
                    weightDisplay.Text = originalWeight.ToString("F1");
                    block.Weight = originalWeight;
                }
            };
            weightOptions.Controls.Add(weightDisplay);
            weightDisplayFilter = new ClickFilter(weightDisplay);
            Application.AddMessageFilter(weightDisplayFilter);

            var minusWeight = new Button()
            {
                UseCompatibleTextRendering = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(255, 163, 42, 42),
                BackColor = BackColor,
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Regular),
                Text = "-",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(24, 24)
            };
            QOL.Align.Right(minusWeight, weightDisplay, 4);
            weightOptions.Controls.Add(minusWeight);
            minusWeight.MouseClick += (s, ev) =>
            {
                block.Weight = block.Weight - 5 > int.MinValue ? block.Weight - 5 : block.Weight;
                weightDisplay.Text = block.Weight.ToString("F1");
            };

            var plusWeight = new Button()
            {
                UseCompatibleTextRendering = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Green,
                BackColor = BackColor,
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Regular),
                Text = "+",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = minusWeight.Size
            };
            QOL.Align.Right(plusWeight, minusWeight, 1);
            weightOptions.Controls.Add(plusWeight);
            plusWeight.MouseClick += (s, ev) =>
            {
                block.Weight = block.Weight + 5 < int.MaxValue ? block.Weight + 5 : block.Weight;
                weightDisplay.Text = block.Weight.ToString("F1");
            };

            var zeroWeight = new Button()
            {
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.Gray,
                BackColor = BackColor,
                Font = new Font(QOL.VCROSDMONO, 13f, FontStyle.Regular),
                Text = "0",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = minusWeight.Size
            };
            QOL.Align.Right(zeroWeight, plusWeight, 1);
            weightOptions.Controls.Add(zeroWeight);
            zeroWeight.MouseClick += (s, ev) =>
            {
                block.Weight = 0;
                weightDisplay.Text = block.Weight.ToString("F1");
            };

            var oneWeight = new Button()
            {
                UseCompatibleTextRendering = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                BackColor = BackColor,
                Font = new Font(QOL.VCROSDMONO, 20f, FontStyle.Regular),
                Text = "1",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = minusWeight.Size
            };
            QOL.Align.Right(oneWeight, zeroWeight, 1);
            weightOptions.Controls.Add(oneWeight);
            oneWeight.MouseClick += (s, ev) =>
            {
                block.Weight = 1;
                weightDisplay.Text = block.Weight.ToString("F1");
            };

            var resetWeight = new Button()
            {
                UseCompatibleTextRendering = true,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                BackColor = BackColor,
                Font = new Font(QOL.VCROSDMONO, 18f, FontStyle.Regular),
                Text = "↻",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = minusWeight.Size
            };
            QOL.Align.Right(resetWeight, oneWeight, 1);
            weightOptions.Controls.Add(resetWeight);
            resetWeight.MouseClick += (s, ev) =>
            {
                block.Weight = originalWeight;
                weightDisplay.Text = block.Weight.ToString("F1");
            };

            var absoluteWeight = new Button()
            {
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.LightGoldenrodYellow,
                BackColor = BackColor,
                Font = new Font(QOL.VCROSDMONO, 12f, FontStyle.Regular),
                Text = "Abs",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(48, 24)
            };
            QOL.Align.Right(absoluteWeight, resetWeight, 1);
            weightOptions.Controls.Add(absoluteWeight);
            absoluteWeight.MouseClick += (s, ev) =>
            {
                if (Math.Abs(block.Weight) > int.MinValue && Math.Abs(block.Weight) < int.MaxValue)
                    block.Weight = Math.Abs(block.Weight);
                else
                    block.Weight = originalWeight;
                weightDisplay.Text = block.Weight.ToString("F1");

            };
            weightOptions.Bounds = new Rectangle(weightOptions.Location, new Size(weightOptions.Controls[weightOptions.Controls.Count - 1].Right, weightOptions.Height));


            var pivotOptions = new CustomPanel()
            {
                ForeColor = Color.Transparent,
                BackColor = QOL.Colors.SameRGB(35),
                Width = toolBar.Width / 2,
                //Height = 24,
                Height = toolBar.Height - weightOptions.Height,
                Location = new Point(toolBar.Left, toolBar.Top + weightOptions.Height + 1)
            };
            toolBar.Controls.Add(pivotOptions);
            pivotOptions.Paint += (s, ev) =>
            {
                using (var pen = new Pen(BackColor, 1f))
                    ev.Graphics.DrawRectangle(pen, 0, 0, pivotOptions.Width - 1, pivotOptions.Height - 1);
            };

            var pivotLabel = new Label()
            {
                Tag = "ToolBarInfoLabel",
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 22f),
                Text = $"Pivot:",
                AutoSize = true,
                Height = pivotOptions.Height,
            };
            pivotOptions.Controls.Add(pivotLabel);


            //var bounce = new Button()
            //{
            //    UseCompatibleTextRendering = true,
            //    TextAlign = ContentAlignment.MiddleCenter,
            //    ForeColor = Color.CadetBlue,
            //    BackColor = Color.Transparent,
            //    Font = new Font(QOL.VCROSDMONO, 12f, FontStyle.Regular),
            //    Text = "B",
            //    TabStop = false,
            //    FlatStyle = FlatStyle.Flat,
            //    Size = new Size(24, 24),
            //};
            ////pivotOptions.Controls.Add(bounce);
            //bool bounceBold = false;
            //bounce.MouseClick += (s, ev) =>
            //{
            //    bounceBold = !bounceBold;
            //    if (bounceBold)
            //        bounce.Font = new Font(bounce.Font.FontFamily, bounce.Font.Size, FontStyle.Bold | FontStyle.Underline | FontStyle.Italic);
            //    else
            //        bounce.Font = new Font(bounce.Font.FontFamily, bounce.Font.Size, FontStyle.Regular);
            //};
            //QOL.Align.Right(bounce, reorient, 1);


            var pivot = new CustomPanel
            {
                BackColor = QOL.Colors.SameRGB(60),
                Size = new Size(72, 72)
            };
            QOL.Align.Right(pivot, pivotLabel, 1);
            pivotOptions.Controls.Add(pivot);

            string[,] directions =
            {
                { "↖", "↑", "↗" },
                { "←", "◎", "→" },
                { "↙", "↓", "↘" }
            };

            var cards = new Card[3, 3];
            int[] offsets = { -1, 0, 1 };

            for (int row = 0; row < cards.GetLength(0); row++)
            {
                for (int col = 0; col < cards.GetLength(1); col++)
                {
                    int r = row;
                    int c = col;

                    cards[r, c] = new Card(r, c)
                    {
                        Location = new Point(c * 24, r * 24),
                        Text = directions[r, c]
                    };
                    pivot.Controls.Add(cards[r, c]);
    
                    if (r == 2 && c == 1)
                        cards[r, c].Toggle();
                }
            }
            Card.deck = cards;
            Card.Activated = (row, col) =>
            {
                gravity.X = offsets[col];
                gravity.Y = offsets[row];
            };

            var randomPivot = new Button()
            {
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.FromArgb(255, 104, 163, 42),
                BackColor = BackColor,
                Font = new Font(QOL.VCROSDMONO, 16f, FontStyle.Regular),
                Text = "🎲",
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(32, 32)
            };
            QOL.Align.Bottom.Left(randomPivot, pivotLabel, 1);
            pivotOptions.Controls.Add(randomPivot);

            bool randomPivotOn = false;
            var randomPivotTimer = new Timer() { Interval = 1000};
            Action PivotRandomly = () =>
            {
                int randomRow = random.Next(cards.GetLength(0));
                int randomCol = random.Next(cards.GetLength(1));
                gravity.X = offsets[randomCol];
                gravity.Y = offsets[randomRow];
                cards[randomRow, randomCol].SetActive();
            };
            randomPivotTimer.Tick += (s, ev) => Card.Activated(random.Next(cards.GetLength(0)), random.Next(cards.GetLength(1)));

            var copyColor = randomPivot.ForeColor;
            randomPivot.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    randomPivotOn = !randomPivotOn;
                    if (randomPivotOn)
                    {
                        randomPivotTimer.Start();
                        randomPivot.ForeColor = Color.FromArgb(255, 42, 96, 163);
                    }
                    else
                    {
                        randomPivotTimer.Stop();
                        cards[2, 1].SetActive();
                        randomPivot.ForeColor = copyColor;
                    }
                }
                if (ev.Button == MouseButtons.Right && randomPivotTimer.Interval > 100)
                    randomPivotTimer.Interval -= 100;
            };



            var gravityOptions = new CustomPanel()
            {
                ForeColor = Color.Transparent,
                BackColor = QOL.Colors.SameRGB(35),
                Width = toolBar.Width / 2,
                //Height = 24,
                Height = toolBar.Height - weightOptions.Height,
            };
            QOL.Align.Right(gravityOptions, pivotOptions);
            toolBar.Controls.Add(gravityOptions);
            gravityOptions.Paint += (s, ev) =>
            {
                using (var pen = new Pen(BackColor, 1f))
                    ev.Graphics.DrawRectangle(pen, 0, 0, gravityOptions.Width - 1, gravityOptions.Height - 1);
            };

            var gravityLabel = new Label()
            {
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "Gravity Type:",
                AutoSize = true,
            };
            gravityOptions.Controls.Add(gravityLabel);

            var gravityModes = Enum.GetValues(typeof(Block.GravityMode)).Cast<object>().ToArray();

            int gravityModeIndex = 0;
            block.Gravity = (Block.GravityMode)gravityModes[gravityModeIndex];
            var gravityChoice = new Button()
            {
                TabStop = false,
                Font = new Font(QOL.VCROSDMONO, 12f),
                Text = gravityModes[gravityModeIndex].ToString(),
            };
            QOL.Align.Bottom.Center(gravityChoice, gravityLabel);
            gravityChoice.MouseClick += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    gravityModeIndex++;
                    if (gravityModeIndex == 3)
                        gravityModeIndex = 0;

                    block.Gravity = (Block.GravityMode)gravityModes[gravityModeIndex];
                    gravityChoice.Text = gravityModes[gravityModeIndex].ToString();
                }
            };

            gravityOptions.Controls.Add(gravityChoice);

            borderBox.MouseClick += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Right)
                {
                    MessageBox.Show($"{block.Gravity}");
                }
            };
        }
    }
}
