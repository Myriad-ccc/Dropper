using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class GravityPanel : CustomPanel
    {
        public Label displayVX, displayVY;
        //public Label displayVTX, displayVTY;
        //private Action GravityModeUpdated;

        private bool built;
        private Block targetBlock;

        public void SetTarget(Block block)
        {
            targetBlock = block ?? throw new ArgumentNullException();
            if (!built)
            {
                Build();
                built = true;
            }

            //block.VTXChanged += newVTX => displayVTX.Text = $"{block.VTX:F1}";
            //block.VTYChanged += newVTY => displayVTY.Text = $"{block.VTY:F1}";
        }

        private void Build()
        {
            ForeColor = Color.White;
            BackColor = Color.Transparent;
            Width = 1024;
            Height = 100;
            Paint += (s, ev) =>
            {
                using (var pen = new Pen(BackColor, 1f))
                    ev.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            };

            var gravityModes = Enum.GetValues(typeof(Block.GravityMode)).Cast<object>().ToArray();

            int gravityModeIndex = Array.IndexOf(gravityModes, Block.GravityMode.Dynamic);
            targetBlock.Gravity = (Block.GravityMode)gravityModes[gravityModeIndex];
            var gravityChoice = new CustomButton()
            {
                UseCompatibleTextRendering = true,
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(QOL.VCROSDMONO, 20f),
                BackColor = QOL.RGB(20),
                Text = gravityModes[gravityModeIndex].ToString(),
                AutoSize = true,
            };
            gravityChoice.MouseClick += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    gravityModeIndex++;
                    if (gravityModeIndex == gravityModes.Length)
                        gravityModeIndex = 0;

                    targetBlock.Gravity = (Block.GravityMode)gravityModes[gravityModeIndex];
                    gravityChoice.Text = gravityModes[gravityModeIndex].ToString();
                    //GravityModeUpdated.Invoke();
                }
            };
            Controls.Add(gravityChoice);

            displayVX = new Label()
            {
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 16f)
            };
            QOL.Align.Bottom.Center(displayVX, gravityChoice, 1);
            Controls.Add(displayVX);

            displayVY = new Label()
            {
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 16f),
                AutoSize = true,
            };
            QOL.Align.Bottom.Center(displayVY, displayVX, 1);
            Controls.Add(displayVY);

            //displayVTX = new Label()
            //{
            //    ForeColor = Color.White,
            //    Font = new Font(QOL.VCROSDMONO, 16f)
            //};
            //QOL.Align.Right(displayVTX, displayVX, 16);
            //Controls.Add(displayVTX);

            //displayVTY = new Label()
            //{
            //    ForeColor = Color.White,
            //    Font = new Font(QOL.VCROSDMONO, 16f),
            //    AutoSize = true,
            //};
            //QOL.Align.Bottom.Center(displayVTY, displayVTX, 1);
            //Controls.Add(displayVTY);
        }
    }
}