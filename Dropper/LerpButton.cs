using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class LerpButton : Button
    {
        public bool On { get; set; }
        public bool Animate { get; set; } = true;
        public bool ShowBorder { get; set; } = false;

        public Color BaseColor { get; set; } = QOL.RGB(35);
        private readonly Color HoverColor = QOL.RGB(60);
        private readonly Color BorderHoverColor = QOL.RGB(80);
        public Color? ClickColor { get; set; }
        public Color CurrentColor { get; set; }

        private Color TargetColor;
        private readonly Timer AnimationTimer = new() { Interval = 10 };
        private float AnimationProgress;
        private float AnimationSpeed { get; set; } = 0.0175f;

        public LerpButton(float? animationSpeed = null, Color? clickColor = null)
        {
            ClickColor = clickColor;
            AnimationSpeed = animationSpeed ?? AnimationSpeed;

            DoubleBuffered = true;
            TabStop = false;
            FlatAppearance.BorderSize = 0;
            FlatStyle = FlatStyle.Flat;
            ForeColor = Color.White;
            Font = new Font(QOL.VCROSDMONO, 20f);

            CurrentColor = TargetColor = BaseColor;
            AnimationTimer.Tick += AnimateColor;
        }
        protected override bool ShowFocusCues => false;
        protected override bool ShowKeyboardCues => false;

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.Clear(CurrentColor);
            TextRenderer.DrawText(
                g,
                Text,
                Font,
                ClientRectangle,
                ForeColor,
                TextFormatFlags.HorizontalCenter
                | TextFormatFlags.VerticalCenter);

            if (ShowBorder)
                using (var borderPen = new Pen(BorderHoverColor, 2f))
                    g.DrawRectangle(borderPen, ClientRectangle.X, ClientRectangle.Y, ClientSize.Width - 1, ClientSize.Height - 1);
        }

        private Color LerpColor(Color from, Color to, float t)
        {
            int r = (int)(from.R + (to.R - from.R) * t);
            int g = (int)(from.G + (to.G - from.G) * t);
            int b = (int)(from.B + (to.B - from.B) * t);
            return Color.FromArgb(255, r, g, b);
        }

        private void StartAnimation(Color @new)
        {
            if (!Animate) return;
            TargetColor = @new;
            AnimationProgress = 0f;
            AnimationTimer.Start();
        }

        private void AnimateColor(object sender, EventArgs e)
        {
            AnimationProgress += AnimationSpeed;
            if (AnimationProgress >= 1f)
            {
                AnimationProgress = 1f;
                AnimationTimer.Stop();
            }
            CurrentColor = LerpColor(CurrentColor, TargetColor, AnimationProgress);
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            StartAnimation(HoverColor);
            //ShowBorder = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            StartAnimation(BaseColor);
            //ShowBorder = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (ClickColor is Color clickColor)
                StartAnimation(clickColor);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            StartAnimation(HoverColor);
        }
    }
}