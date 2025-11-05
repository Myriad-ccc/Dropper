using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class ToolBarPanel : CustomPanel
    {
        private readonly Gravity Gravity;

        public WeightPanel weightPanel;
        public WeightSlider weightSlider;
        public ExpandedWeightMenu expandedWeightMenu;
        public PivotPanel pivotPanel;
        public GravityPanel gravityPanel;

        public List<CustomPanel> Values = new List<CustomPanel>();

        private bool built = false;
        private Block targetBlock;

        private bool LayoutInitialized = false;

        public void SetTarget(Block block)
        {
            targetBlock = block ?? throw new ArgumentNullException();

            weightPanel.SetTarget(targetBlock);
            weightSlider.SetTarget(targetBlock);
            expandedWeightMenu.SetTarget(targetBlock);
            pivotPanel.SetTarget(targetBlock);
            gravityPanel.SetTarget(targetBlock);

            if (!LayoutInitialized)
            {
                InitializeLayout();
                LayoutInitialized = true;
            }

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

        public ToolBarPanel(Gravity gravity)
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

        private void InitializeLayout()
        {
            foreach (var value in Values)
            {
                value.Draggable = true;
                value.ParentBounds = ClientRectangle;

                if (value == weightSlider) continue;
                QOL.ClampControlSize(value);
            }

            if (!weightPanel.Added)
            {
                weightPanel.Location = new Point(0, 0);
                weightPanel.Added = true;
            }

            weightSlider.Width = 144;
            if (!weightSlider.Added)
            {
                QOL.Align.Bottom.Center(weightSlider, weightPanel, 8);
                weightSlider.Added = true;
            }

            if (!expandedWeightMenu.Added)
            {
                QOL.Align.Bottom.Center(expandedWeightMenu, weightPanel, 2);
                expandedWeightMenu.Added = true;
            }

            if (!pivotPanel.Added)
            {
                pivotPanel.Location = new Point(weightPanel.Right + 16, 2);
                pivotPanel.Added = true;
            }

            if (!gravityPanel.Added)
            {
                QOL.Align.Right(gravityPanel, pivotPanel, 16);
                gravityPanel.Added = true;
            }
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
        public new void Hide()
        {
            Visible = false;
            Values.ForEach(x => x.Visible = false);
        }
    }
}