using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public class GravityPanel : CustomPanel
    {
        private readonly Block Block;
        private readonly Gravity Gravity;

        public Label displayVX, displayVY;

        public GravityPanel(Block block, Gravity gravity)
        {
            Block = block;
            Gravity = gravity;
            BuildGravityPanel();
        }

        private void BuildGravityPanel()
        {
            ForeColor = Color.Transparent;
            BackColor = QOL.Colors.SameRGB(35);
            Width = 1024;
            Height = 72;
            Paint += (s, ev) =>
            {
                using (var pen = new Pen(BackColor, 1f))
                    ev.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            };

            var gravityLabel = new Label()
            {
                Font = new Font(QOL.VCROSDMONO, 20f),
                Text = "Gravity",
                AutoSize = true,
            };
            Controls.Add(gravityLabel);

            var gravityModes = Enum.GetValues(typeof(Block.GravityMode)).Cast<object>().ToArray();

            int gravityModeIndex = 0;
            Block.Gravity = (Block.GravityMode)gravityModes[gravityModeIndex];
            var gravityChoice = new Button()
            {
                UseCompatibleTextRendering = true,
                TabStop = false,
                Font = new Font(QOL.VCROSDMONO, 10f),
                FlatStyle = FlatStyle.Flat,
                Text = gravityModes[gravityModeIndex].ToString(),
                AutoSize = true,
            };
            QOL.Align.Bottom.Center(gravityChoice, gravityLabel, 1);
            gravityChoice.MouseClick += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Left)
                {
                    gravityModeIndex++;
                    if (gravityModeIndex == gravityModes.Length)
                        gravityModeIndex = 0;

                    Block.Gravity = (Block.GravityMode)gravityModes[gravityModeIndex];
                    gravityChoice.Text = gravityModes[gravityModeIndex].ToString();
                }
            };
            Controls.Add(gravityChoice);

            displayVX = new Label()
            {
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 14f)
            };
            QOL.Align.Right(displayVX, gravityLabel, 1);
            Controls.Add(displayVX);

            displayVY = new Label()
            {
                ForeColor = Color.White,
                Font = new Font(QOL.VCROSDMONO, 14f),
                AutoSize = true,
            };
            QOL.Align.Bottom.Center(displayVY, displayVX, 1);
            Controls.Add(displayVY);

            foreach (var button in Controls.OfType<Button>())
                button.BackColor = QOL.Colors.SameRGB(20);
        }
    }
}
