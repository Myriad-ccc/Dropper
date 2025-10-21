using System.Drawing;

namespace Dropper
{
    public class Area : CustomPanel
    {
        public ToolbarPanel toolBar;
        public GameArea gameArea;
        public Floor floor;

        public Area(Block block, Gravity gravity, Size formSize, Size titleSize)
        {
            Width = formSize.Width;
            Height = formSize.Height - titleSize.Height;

            toolBar = new ToolbarPanel(block, gravity);
            gameArea = new GameArea(block);
            floor = new Floor();

            Controls.Add(toolBar);
            Controls.Add(gameArea);
            Controls.Add(floor);

            toolBar.Size = new Size(formSize.Width, toolBar.Height);
            toolBar.Location = new Point();

            floor.Size = new Size(formSize.Width, 32);
            floor.Location = new Point(0, Bottom - floor.Height);

            gameArea.Size = new Size(formSize.Width, ClientSize.Height - toolBar.Height - floor.Height);
            gameArea.Location = new Point(0, toolBar.Bottom);

            Block.StartPoint = new Point(
                (int)(gameArea.Width / 2 - block.W / 2),
                (int)(gameArea.ClientSize.Height - block.H));
            block.Bounds = new RectangleF(Block.StartPoint, block.Size);

            block.UserBounds = gameArea.ClientRectangle;

            block.MagneticCore = new Point(
                (int)(gameArea.Width / 2 - block.W / 2),
                (int)(gameArea.Height / 2 - block.H / 2));

            gravity.VXChanged += newVX =>
            {
                if (block.Gravity == Block.GravityMode.Dynamic)
                    toolBar.gravityPanel.displayVX.Text = $"{newVX:F1}";
            };
            gravity.VYChanged += newVY =>
            {
                if (block.Gravity == Block.GravityMode.Dynamic)
                    toolBar.gravityPanel.displayVY.Text = $"{newVY:F1}";
            };

            gravity.Redraw += () =>
            {
                if (block.Gravity != Block.GravityMode.Dynamic)
                {
                    toolBar.gravityPanel.displayVX.Text = "";
                    toolBar.gravityPanel.displayVY.Text = "";
                }
                gameArea.Invalidate();
            };
        }
    }
}
