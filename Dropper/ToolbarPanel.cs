using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class ToolbarPanel : CustomPanel
    {
        public WeightPanel weightPanel;
        public WeightSlider weightSlider;
        public ExpandedWeightMenu expandedWeightMenu;
        public PivotPanel pivotPanel;
        public GravityPanel gravityPanel;

        private bool built = false;
        private Block targetBlock;

        public void SetActiveBlock(Block block)
        {
            targetBlock = block ?? throw new ArgumentNullException();

            weightPanel.SetActiveBlock(targetBlock);
            weightSlider.SetActiveBlock(targetBlock);
            expandedWeightMenu.SetActiveBlock(targetBlock);
            pivotPanel.SetActiveBlock(targetBlock);
            gravityPanel.SetActiveBlock(targetBlock);
        }

        public ToolbarPanel(Gravity gravity)
        {
            BackColor = QOL.RGB(50);

            weightPanel = new WeightPanel();
            weightSlider = new WeightSlider();
            expandedWeightMenu = new ExpandedWeightMenu();
            pivotPanel = new PivotPanel(gravity);
            gravityPanel = new GravityPanel();

            Controls.Add(weightPanel);
            Controls.Add(weightSlider);
            Controls.Add(expandedWeightMenu);
            Controls.Add(pivotPanel);
            Controls.Add(gravityPanel);

            if (!built)
                Build();
        }

        public void Build()
        {
            built = true;

            weightPanel.CollapseExpandedWeightPanel += (s, ev) =>
            {
                expandedWeightMenu.Visible = !expandedWeightMenu.Visible;
                weightPanel.collapsableMenu.Text = expandedWeightMenu.Visible ? "-" : "+";
            };

            void OnWeightChanged(float newWeight)
            {

                if (QOL.ValidFloat32(newWeight))
                    targetBlock.Weight = newWeight;
                else
                    targetBlock.Weight = targetBlock.OriginalWeight;

                if (targetBlock.Weight > -100 && targetBlock.Weight < 100)
                    weightPanel.weightDisplay.Text = $"{newWeight:F1}";
                else
                    weightPanel.weightDisplay.Text = $"{newWeight:F0}";
            }

            weightSlider.WeightChanged += OnWeightChanged;

            expandedWeightMenu.BringToFront();
            expandedWeightMenu.MouseClick += (s, ev) => weightSlider.Visible = false;
            expandedWeightMenu.WeightChanged += OnWeightChanged;
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            QOL.ClampControlWidth(weightPanel);

            QOL.ClampControlWidth(weightSlider);
            QOL.Align.Bottom.Center(weightSlider, weightPanel, 8);

            QOL.Align.Bottom.Center(expandedWeightMenu, weightPanel, 2);

            QOL.ClampControlWidth(pivotPanel);
            pivotPanel.Location = new Point(weightPanel.Right + 16, 2);

            QOL.ClampControlWidth(gravityPanel, 40);
            QOL.Align.Right(gravityPanel, pivotPanel, 16);
        }
    }
}
