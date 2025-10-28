using System.Collections.Generic;
using System.Windows.Forms;

namespace Dropper
{
    public class Area : CustomPanel
    {
        private Gravity Gravity;

        public ToolbarPanel toolBar;
        public GameArea gameArea;
        public Floor floor;

        public void SetActiveBlock(Block block)
        {
            toolBar.SetActiveBlock(block);
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

        public void Build(List<Block> blocks, Gravity gravity)
        {
            Gravity = gravity;

            toolBar = new ToolbarPanel(gravity);
            gameArea = new GameArea(blocks);
            floor = new Floor();

            Controls.Add(toolBar);
            Controls.Add(gameArea);
            Controls.Add(floor);

            toolBar.Height = 98;
            toolBar.Dock = DockStyle.Top;

            gameArea.Dock = DockStyle.Fill;
            gameArea.BringToFront();

            floor.Height = 32;
            floor.Dock = DockStyle.Bottom;

        }
    }
}
