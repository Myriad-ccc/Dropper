using System.Windows.Forms;

namespace Dropper
{
    public class Area : CustomPanel
    {
        public ToolbarPanel toolBar;
        public GameArea gameArea;
        public Floor floor;

        public void SetTarget(Block block)
        {
            toolBar.SetTarget(block);
            gameArea.Invalidate();
        }

        public void Build(Gravity gravity)
        {
            toolBar = new ToolbarPanel(gravity);
            gameArea = new GameArea();
            floor = new Floor();

            Controls.Add(toolBar);
            Controls.Add(gameArea);
            Controls.Add(floor);

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
