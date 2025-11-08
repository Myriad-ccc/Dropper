using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static Dropper.InstanceData;

namespace Dropper
{
    public partial class Form1 : Form
    {
        private Blocks blocks;
        private Gravity gravity;

        private ControlBar controlBar;
        private CustomPanel PanelOptions;
        private CustomPanel ConfigOptions;
        private ToolBarPanel toolBar;
        private GameArea gameArea;
        private Floor floor;

        private ViewConfig viewConfig;
        public static readonly string instanceSavePath = Path.Combine(Application.StartupPath, "DropperStateSave.json");

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigureForm();
            HoodooVoodooBlockMagic();
            AddPanels();
            ConfigurePanels();
            if (Blocks.Stack.Count == 0) blocks.Add();
            LoadState();
            gravity.Start(Blocks.Stack);

            var label = new Label()
            {
                Text = "Saving",
                ForeColor = Color.Green,
                Font = new Font(QOL.VCROSDMONO, 20f),
                AutoSize = true
            };
            controlBar.Controls.Add(label);
            label.Location = new Point(Width - 128 - TextRenderer.MeasureText(label.Text, label.Font).Width, 0);

            if (!File.Exists(instanceSavePath))
                label.ForeColor = Color.Red;
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

            FormClosing += Form1_Closing;
        }
        private void Form1_Closing(object sender, EventArgs e)
        {
            SaveState();
            Application.RemoveMessageFilter(toolBar.weightPanel.WeightDisplayFilter);
        }
        private void SaveState()
        {
            try
            {
                var currentInstanceState = new AppState //form
                {
                    GravityX = gravity.X,
                    GravityY = gravity.Y,
                    Blocks = []
                };

                foreach (var block in Blocks.Stack) //blocks
                {
                    currentInstanceState.Blocks.Add(new BlockData
                    {
                        Bounds = block.Bounds,
                        Weight = block.Weight,
                        Gravity = block.Gravity,
                        CrackCount = block.Cracks.Count,
                        ActiveColor = block.ActiveColor,
                        Active = block == blocks.Target,
                        InactiveColor = block.InactiveColor,
                        OriginalWeight = block.OriginalWeight,
                        ActiveBorderColor = block.ActiveBorderColor,
                        InactiveBorderColor = block.InactiveBorderColor
                    });
                }

                string jsonString = JsonConvert.SerializeObject(currentInstanceState, Formatting.Indented);
                if (File.Exists(instanceSavePath))
                    File.WriteAllText(instanceSavePath, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save state: {ex.Message}");
            }
        }
        private void LoadState()
        {
            if (!File.Exists(instanceSavePath))
                return;

            try
            {
                string jsonString = File.ReadAllText(instanceSavePath);
                AppState lastInstanceState = JsonConvert.DeserializeObject<AppState>(jsonString);

                gravity.X = lastInstanceState.GravityX; //form
                gravity.Y = lastInstanceState.GravityY;

                Blocks.Stack.Clear();
                foreach (var blockData in lastInstanceState.Blocks) //blocks
                {
                    var newBlock = new Block
                    {
                        Active = blockData.Active,
                        Weight = blockData.Weight,
                        Bounds = blockData.Bounds,
                        Gravity = blockData.Gravity,
                        ActiveColor = blockData.ActiveColor,
                        UserBounds = gameArea.ClientRectangle,
                        InactiveColor = blockData.InactiveColor,
                        OriginalWeight = blockData.OriginalWeight,
                        ActiveBorderColor = blockData.ActiveBorderColor,
                        InactiveBorderColor = blockData.InactiveBorderColor,
                    };
                    BlockMagneticCore(newBlock);

                    for (int i = 0; i < blockData.CrackCount; i++)
                        newBlock.Crack();

                    Blocks.Stack.Add(newBlock);

                    if (blockData.Active)
                        ChangeFocusedBlock(newBlock);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load state: {ex.Message}. Loading defaults.");
            }
        }

        private void AddPanels()
        {
            blocks = new Blocks();
            blocks.Redraw += () => gameArea.Invalidate();
            blocks.ChangeFocus += block => ChangeFocusedBlock(block);
            blocks.ConfigureBlock += ConfigureBlock;

            gravity = new Gravity();
            gravity.Redraw += () => gameArea.Invalidate();
            gravity.SplitBlock += block => blocks.Split(block);

            toolBar = new ToolBarPanel(gravity);
            PanelOptions = new CustomPanel
            {
                Height = 160,
                BackColor = QOL.RandomColor(),
            };
            ConfigOptions = new CustomPanel
            {
                Height = 160,
                BackColor = QOL.RandomColor(),
            };
            controlBar = new ControlBar(toolBar, PanelOptions, ConfigOptions);
            gameArea = new GameArea();
            floor = new Floor();

            Controls.Add(PanelOptions);
            Controls.Add(ConfigOptions);
            Controls.Add(toolBar);
            Controls.Add(gameArea);
            Controls.Add(controlBar);
            Controls.Add(floor);
        }
        private void ConfigurePanels()
        {
            gameArea.Dock = DockStyle.Fill;

            controlBar.Height = 54;
            controlBar.Dock = DockStyle.Top;

            toolBar.Size = new Size(ClientSize.Width, 0);
            toolBar.Location = new Point(0, controlBar.Bottom);
            toolBar.Hide();

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

            floor.Height = 32;
            floor.Dock = DockStyle.Bottom;

            var viewConfigInitial = new Point(Width / 4, controlBar.Bottom);
            viewConfig = new ViewConfig
            {
                Size = new Size(Width / 2, gameArea.Height),
                Visible = false,
                Location = viewConfigInitial
            };
            Controls.Add(viewConfig);
            viewConfig.BringToFront();

            viewConfig.MouseDown += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Middle)
                    viewConfig.Location = viewConfigInitial;
            };

            controlBar.ShowViewConfig += () => viewConfig.Visible = !viewConfig.Visible;

            gameArea.FocusedBlockChanged += block => ChangeFocusedBlock(block);
            gameArea.SplitBlock += block => blocks.Split(block);
        }

        private void ConfigureBlock(Block block)
        {
            block.OriginalWeight = block.Weight;
            block.UserBounds = gameArea.ClientRectangle;
            BlockToStartingPoint(block);
            BlockMagneticCore(block);
        }
        private void BlockToStartingPoint(Block block) =>
            block.Bounds = new RectangleF(
                new PointF(
                    (int)(gameArea.Width / 2 - block.W / 2),
                    (int)(gameArea.ClientSize.Height - block.H)),
                block.Size);
        private void BlockMagneticCore(Block block) =>
            block.MagneticCore = new Point(
                (int)(gameArea.Width / 2 - block.W / 2),
                (int)(gameArea.Height / 2 - block.H / 2));
        private void ChangeFocusedBlock(Block block)
        {
            if (block == blocks.Target)
                return;

            blocks.Target?.Active = false;
            blocks.Target = block;
            blocks.Target.Active = true;

            toolBar.SetTarget(block);
            gameArea.Invalidate();
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
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                blocks.Add();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}