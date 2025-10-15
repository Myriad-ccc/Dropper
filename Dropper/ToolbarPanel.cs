using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class ToolbarPanel : CustomPanel
    {
        public WeightPanel weightPanel;
        public PivotPanel pivotPanel;
        public GravityPanel gravityPanel;

        public ToolbarPanel(Block block, Gravity gravity)
        {
            Width = 1024;
            Height = 96;

            weightPanel = new WeightPanel(block, gravity);
            pivotPanel = new PivotPanel(block, gravity);
            gravityPanel = new GravityPanel(block, gravity);

            Controls.Add(weightPanel);
            Controls.Add(pivotPanel);
            Controls.Add(gravityPanel);

            weightPanel.Location = new Point(Left, Top);
            QOL.ClampControlWidth(weightPanel);

            QOL.ClampControlWidth(pivotPanel);
            pivotPanel.Location = new Point(Right - pivotPanel.Width,Top);

            QOL.Align.Left(gravityPanel, pivotPanel, 32);
            QOL.ClampControlWidth(gravityPanel);
        }
    }
}
