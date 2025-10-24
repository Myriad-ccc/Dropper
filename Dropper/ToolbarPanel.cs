using System.Drawing;

namespace Dropper
{
    public class ToolbarPanel : CustomPanel
    {
        private readonly Form1 mainForm;

        public WeightPanel weightPanel;
        public WeightSlider weightSlider;
        public ExpandedWeightMenu expandedWeightMenu;
        public PivotPanel pivotPanel;
        public GravityPanel gravityPanel;

        public ToolbarPanel(Form1 form, Gravity gravity)
        {
            mainForm = form;

            Height = 98; //96
            BackColor = QOL.RGB(50);

            weightPanel = new WeightPanel(form.activeBlock);
            weightSlider = new WeightSlider(form.activeBlock);
            //expandedWeightMenu = new ExpandedWeightMenu(form.activeBlock);
            pivotPanel = new PivotPanel(form.activeBlock, gravity);
            gravityPanel = new GravityPanel(form.activeBlock);

            Controls.Add(weightPanel);
            Controls.Add(weightSlider);
            //Controls.Add(expandedWeightMenu);
            Controls.Add(pivotPanel);
            Controls.Add(gravityPanel);

            mainForm.ActiveBlockChanged += UpdateReference;
            UpdateReference(mainForm.activeBlock);

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
                    mainForm.activeBlock.Weight = newWeight;
                else expandedWeightMenu.ResetWeight += () =>
                {
                    mainForm.activeBlock.Weight = weightPanel.originalWeight;
                    weightPanel.weightDisplay.Text = $"{mainForm.activeBlock.Weight:F1}";
                };

                if (mainForm.activeBlock.Weight > -100 && mainForm.activeBlock.Weight < 100)
                    weightPanel.weightDisplay.Text = $"{newWeight:F1}";
                else
                    weightPanel.weightDisplay.Text = $"{newWeight:F0}";
            }

            QOL.Align.Bottom.Center(weightSlider, weightPanel);
            weightSlider.Width = weightPanel.Width;
            weightSlider.WeightChanged += OnWeightChanged;
            weightSlider.BringToFront();

            //QOL.Align.Bottom.Center(expandedWeightMenu, weightPanel, 2);
            ////expandedWeightMenu.BringToFront();
            //expandedWeightMenu.MouseClick += (s, ev) => weightSlider.Visible = false;
            //expandedWeightMenu.WeightChanged += OnWeightChanged;
            //expandedWeightMenu.ResetWeight += () =>
            //{
            //    mainForm.activeBlock.Weight = weightPanel.originalWeight;
            //    weightPanel.weightDisplay.Text = $"{mainForm.activeBlock.Weight:F1}";
            //};

            QOL.ClampControlWidth(pivotPanel);
            QOL.Align.Right(pivotPanel, weightPanel, 16);
            pivotPanel.Location = new Point(pivotPanel.Bounds.X, pivotPanel.Bounds.Y + 2);

            QOL.ClampControlWidth(gravityPanel, 40);
            QOL.Align.Right(gravityPanel, pivotPanel, 16);
        }

        private void UpdateReference(Block block)
        {
            weightPanel.SetActiveBlock(block);
            weightSlider.SetActiveBlock(block);
            //expandedWeightMenu.SetActiveBlock(block);
            pivotPanel.SetActiveBlock(block);
            gravityPanel.SetActiveBlock(block);
        }
    }
}
