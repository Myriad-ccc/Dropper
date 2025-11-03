using System.Windows.Forms;

namespace Dropper
{
    public class Area : CustomPanel
    {
        private Gravity Gravity;

        public ToolbarPanel toolBar;
        public GameArea gameArea;
        public Floor floor;

        public void SetTarget(Block block)
        {
            toolBar.SetTarget(block);
            gameArea.Invalidate();

            Gravity.VXChanged += newVX =>
            {
                if (block.Gravity == Block.GravityMode.Dynamic)
                    toolBar.gravityPanel.displayVX.Text = $"{newVX:F1}";
            };
            Gravity.VYChanged += newVY =>
            {
                if (block.Gravity == Block.GravityMode.Dynamic)
                    toolBar.gravityPanel.displayVY.Text = $"{newVY:F1}";
            };

            Gravity.Redraw += () =>
            {
                if (block.Gravity != Block.GravityMode.Dynamic)
                {
                    toolBar.gravityPanel.displayVX.Text = "";
                    toolBar.gravityPanel.displayVY.Text = "";
                }
                gameArea.Invalidate();
            };
        }

        public void Build(Gravity gravity)
        {
            Gravity = gravity;

            toolBar = new ToolbarPanel(gravity);
            gameArea = new GameArea();
            floor = new Floor();

            Controls.Add(toolBar);
            Controls.Add(gameArea);
            Controls.Add(floor);

            toolBar.Visible = false;
            toolBar.Height = 98;
            toolBar.Dock = DockStyle.Top;

            gameArea.BringToFront();
            gameArea.Dock = DockStyle.Fill;

            floor.Height = 32;
            floor.Dock = DockStyle.Bottom;

            gravity.Redraw += () => gameArea.Invalidate();
        }
    }
}
