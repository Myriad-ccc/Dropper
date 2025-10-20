using System;
using System.Drawing;

namespace Dropper
{
    public class ToolbarPanel : CustomPanel
    {
        public WeightPanel weightPanel;
        public WeightSlider weightSlider;
        public ExpandedWeightMenu expandedWeightMenu;
        public PivotPanel pivotPanel;
        public GravityPanel gravityPanel;

        public ToolbarPanel(Block block, Gravity gravity)
        {
            Height = 98; //96
            BackColor = QOL.RGB(50);

            weightPanel = new WeightPanel(block);
            weightSlider = new WeightSlider(block);
            expandedWeightMenu = new ExpandedWeightMenu(block);
            pivotPanel = new PivotPanel(gravity);
            gravityPanel = new GravityPanel(block);

            Controls.Add(weightPanel);
            Controls.Add(weightSlider);
            Controls.Add(expandedWeightMenu);
            Controls.Add(pivotPanel);
            Controls.Add(gravityPanel);

            QOL.ClampControlWidth(weightPanel);
            weightPanel.Location = new Point();
            weightPanel.CollapseExpandedWeightPanel += (s, ev) =>
            {
                expandedWeightMenu.Visible = !expandedWeightMenu.Visible;
                weightPanel.collapsableMenu.Text = expandedWeightMenu.Visible ? "-" : "+";
            };

            void OnWeightChanged(float newWeight)
            {
                if (QOL.ValidFloat32(newWeight))
                    block.Weight = newWeight;
                else expandedWeightMenu.ResetWeight += () =>
                {
                    block.Weight = weightPanel.originalWeight;
                    weightPanel.weightDisplay.Text = $"{block.Weight:F1}";
                };

                if (block.Weight > -100 && block.Weight < 100)
                    weightPanel.weightDisplay.Text = $"{newWeight:F1}";
                else if (block.Weight == (float)Math.PI)
                    weightPanel.weightDisplay.Text = $"{Math.PI:F5}";
                else if (block.Weight == (float)Math.E)
                    weightPanel.weightDisplay.Text = $"{Math.E:F5}";
                else
                    weightPanel.weightDisplay.Text = $"{newWeight:F0}";
            }

            QOL.ClampControlWidth(weightSlider);
            QOL.Align.Bottom.Center(weightSlider, weightPanel, 8);
            weightSlider.WeightChanged += OnWeightChanged;

            QOL.Align.Bottom.Center(expandedWeightMenu, weightPanel, 2);
            expandedWeightMenu.BringToFront();
            expandedWeightMenu.MouseClick += (s, ev) => weightSlider.Visible = false;
            expandedWeightMenu.WeightChanged += OnWeightChanged;
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
