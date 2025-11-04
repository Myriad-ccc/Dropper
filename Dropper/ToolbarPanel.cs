using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class ToolbarPanel : CustomPanel
    {
        private Gravity Gravity;

        public WeightPanel weightPanel;
        public WeightSlider weightSlider;
        public ExpandedWeightMenu expandedWeightMenu;
        public PivotPanel pivotPanel;
        public GravityPanel gravityPanel;

        public List<CustomPanel> Values = new List<CustomPanel>();

        private bool built = false;
        private Block targetBlock;

        public void SetTarget(Block block)
        {
            targetBlock = block ?? throw new ArgumentNullException();

            weightPanel.SetTarget(targetBlock);
            weightSlider.SetTarget(targetBlock);
            expandedWeightMenu.SetTarget(targetBlock);
            pivotPanel.SetTarget(targetBlock);
            gravityPanel.SetTarget(targetBlock);

            Gravity.VXChanged += newVX =>
            {
                if (block.Gravity == Block.GravityMode.Dynamic)
                    gravityPanel.displayVX.Text = $"{newVX:F1}";
            };
            Gravity.VYChanged += newVY =>
            {
                if (block.Gravity == Block.GravityMode.Dynamic)
                    gravityPanel.displayVY.Text = $"{newVY:F1}";
            };

            Gravity.Redraw += () =>
            {
                if (block.Gravity != Block.GravityMode.Dynamic)
                {
                    gravityPanel.displayVX.Text = "";
                    gravityPanel.displayVY.Text = "";
                }
            };
        }

        public ToolbarPanel(Gravity gravity)
        {
            Gravity = gravity;
            BackColor = QOL.RGB(50);

            weightPanel = new WeightPanel();
            weightSlider = new WeightSlider();
            expandedWeightMenu = new ExpandedWeightMenu();
            pivotPanel = new PivotPanel(gravity);
            gravityPanel = new GravityPanel();

            Values.AddRange(new CustomPanel[] { weightPanel, weightSlider, expandedWeightMenu, pivotPanel, gravityPanel });

            foreach (var value in Values)
                Controls.Add(value);

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
                {
                    if (newWeight > 0)
                        targetBlock.Weight = Math.Min(newWeight, 100000);
                    if (newWeight < 0)
                        targetBlock.Weight = Math.Max(newWeight, -100000);
                }
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

            weightPanel.Size = new Size(264, 24);
            QOL.ClampControlWidth(weightPanel);

            weightSlider.Width = 144;
            QOL.ClampControlWidth(weightSlider);
            QOL.Align.Bottom.Center(weightSlider, weightPanel, 8);

            expandedWeightMenu.Size = new Size(144, 72);
            QOL.Align.Bottom.Center(expandedWeightMenu, weightPanel, 2);

            pivotPanel.Size = new Size(120, 100);
            QOL.ClampControlWidth(pivotPanel);
            pivotPanel.Location = new Point(weightPanel.Right + 16, 2);

            gravityPanel.Size = new Size(200, 100);
            QOL.ClampControlWidth(gravityPanel, 40);
            QOL.Align.Right(gravityPanel, pivotPanel, 16);
        }
    }
}