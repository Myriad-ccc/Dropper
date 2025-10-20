using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public partial class Form1 : Form
    {
        private readonly Block block = new Block();
        private readonly Gravity gravity = new Gravity();

        private TitleBar titleBar;
        private Area area;

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
            titleBar = new TitleBar(new Size(Width, 64));
            Controls.Add(titleBar);
            CenterToScreen();
        }

        private void SetScene()
        {
            area = new Area(block, gravity, Size, titleBar.Size)
            {
                Location = new Point(0, titleBar.Bottom),
                Size = new Size(ClientSize.Width, ClientSize.Height - titleBar.Height)
            };
            Controls.Add(area);
        }

        private void BlockPhysics()
        {
            block.Drag(area.gameArea);
            gravity.CheckGravity(block);
        }
    }
}