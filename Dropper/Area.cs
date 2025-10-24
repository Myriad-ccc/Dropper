using System;
using System.Drawing;

namespace Dropper
{
    public class Area : CustomPanel
    {
        private Block targetBlock;
        private readonly Gravity Gravity;

        public ToolbarPanel toolBar;
        public GameArea gameArea;
        public Floor floor;

        public void SetActiveBlock(Block block)
        {
            targetBlock = block;
            if (Gravity == null || toolBar == null || gameArea == null || floor == null) return;
            Gravity.VXChanged += newVX =>
            {
                if (targetBlock.Gravity == Block.GravityMode.Dynamic)
                    toolBar.gravityPanel.displayVX.Text = $"{newVX:F1}";
            };
            Gravity.VYChanged += newVY =>
            {
                if (targetBlock.Gravity == Block.GravityMode.Dynamic)
                    toolBar.gravityPanel.displayVY.Text = $"{newVY:F1}";
            };

            Gravity.Redraw += () =>
            {
                if (targetBlock.Gravity != Block.GravityMode.Dynamic)
                {
                    toolBar.gravityPanel.displayVX.Text = "";
                    toolBar.gravityPanel.displayVY.Text = "";
                }
                gameArea.Invalidate();
            };
        }

        public Area(Form1 mainForm, Gravity gravity)
        {
            if (targetBlock == null) targetBlock = mainForm.activeBlock;
            Gravity = gravity;

            toolBar = new ToolbarPanel(mainForm, gravity);  
            gameArea = new GameArea(mainForm.blocks);
            floor = new Floor();

            Controls.Add(toolBar);
            Controls.Add(gameArea);
            Controls.Add(floor);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (toolBar == null || gameArea == null || floor == null) return;

            toolBar.Size = new Size(ClientSize.Width, toolBar.ClientSize.Height);
            toolBar.Location = new Point();

            floor.Size = new Size(ClientSize.Width, 32);
            floor.Location = new Point(0, ClientSize.Height - floor.ClientSize.Height);

            gameArea.Size = new Size(ClientSize.Width, ClientSize.Height - toolBar.ClientSize.Height - floor.ClientSize.Height);
            gameArea.Location = new Point(0, toolBar.ClientSize.Height);
        }
    }
}
