using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class ToolbarPanel : CustomPanel
    {
        public WeightPanel weightPanel;
        public ExpandedWeightMenu expandedWeightMenu;
        public PivotPanel pivotPanel;
        public GravityPanel gravityPanel;

        public ToolbarPanel(Block block, Gravity gravity)
        {
            Width = 1024;
            Height = 100; //96

            weightPanel = new WeightPanel(block);
            expandedWeightMenu = new ExpandedWeightMenu(block);
            pivotPanel = new PivotPanel(block, gravity);
            gravityPanel = new GravityPanel(block, gravity);

            Controls.Add(weightPanel);
            Controls.Add(expandedWeightMenu);
            Controls.Add(pivotPanel);
            Controls.Add(gravityPanel);

            QOL.ClampControlWidth(weightPanel);
            weightPanel.Location = new Point(Left, Top);
            weightPanel.CollapseExpandedWeightPanel += (s, ev) =>
            {
                expandedWeightMenu.Visible = !expandedWeightMenu.Visible;
                weightPanel.collapsableMenu.Text = expandedWeightMenu.Visible ? "-" : "+";
            };

            expandedWeightMenu.Size = new Size(weightPanel.Width, 72);
            QOL.Align.Bottom.Center(expandedWeightMenu, weightPanel, 3);
            expandedWeightMenu.WeightChanged += newWeight => weightPanel.weightDisplay.Text = $"{newWeight:F1}";
            expandedWeightMenu.ResetWeight += () =>
            {
                block.Weight = weightPanel.originalWeight;
                weightPanel.weightDisplay.Text = $"{block.Weight:F1}";
            };

            QOL.ClampControlWidth(pivotPanel);
            QOL.Align.Right(pivotPanel, weightPanel, 16);
            pivotPanel.Location = new Point(pivotPanel.Bounds.X, pivotPanel.Bounds.Y + 2);

            QOL.ClampControlWidth(gravityPanel, 40);
            QOL.Align.Right(gravityPanel, pivotPanel, 16);
        }
    }
}
