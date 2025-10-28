using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dropper
{
    public static class QOL
    {
        private static readonly Random random = new Random();
        public static readonly string VCROSDMONO = "VCR OSD Mono";

        public static int RandomInt(int max, int? min = null) => random.Next(min ?? 0, max);

        public static Color RandomColor() => Color.FromArgb(255, RandomInt(256), RandomInt(256), RandomInt(256));
        public static Color RGB(int v1, int? v2 = null, int? v3 = null) => Color.FromArgb(255, v1, v2 ?? v1, v3 ?? v1);

        public static void WriteOut(object o) => MessageBox.Show($"{o}");
        public static void QuickWriteOut(Func<object> getter, Control surface)
        {
            surface.MouseClick += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Right)
                    WriteOut(getter());
            };
        }

        public static void ClampControlWidth(Control control, int? gap = null)
        {
            if (control.Controls.Count == 0) return;
            int rightMost = control.Controls.Cast<Control>().Max(x => x.Right);
            control.Bounds = new Rectangle(control.Location, new Size(rightMost + (gap ?? 0), control.Height));
        }

        public static bool ValidInt32(int num) => num > int.MinValue && num < int.MaxValue;
        public static bool ValidFloat32(float num) => !float.IsNaN(num) && !float.IsInfinity(num);
        public static bool ValidDouble64(float num) => num > double.MinValue && num < double.MaxValue;

        public static int Factorial(int num)
        {
            if (num == 0 || num == 1) return 1;
            return num *= Factorial(num - 1);
        }

        public static class Align
        {
            private static int CheckGap(int? gap) => gap ?? 0;

            public static void Left(Control thisControl, Control otherControl, int? gap = null, bool? top = null)
            {
                int multiplier = top == false ? 1 : 0;
                thisControl.Location = new Point(
                    otherControl.Location.X - otherControl.Width - CheckGap(gap),
                    otherControl.Location.Y + multiplier * (otherControl.Height - thisControl.Height));
            }

            public static void Right(Control thisControl, Control otherControl, int? gap = null, bool? top = null)
            {
                int multiplier = top == false ? 1 : 0;
                thisControl.Location = new Point(
                    otherControl.Location.X + otherControl.Width + CheckGap(gap),
                    otherControl.Location.Y + multiplier * (otherControl.Height - thisControl.Height));
            }

            public static class Top
            {
                public static void Left(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X - thisControl.Width,
                        otherControl.Location.Y - thisControl.Height - CheckGap(gap));

                public static void Center(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X + otherControl.Width / 2 - thisControl.Width / 2,
                        otherControl.Location.Y - thisControl.Height - CheckGap(gap));

                public static void LeftCenter(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X - thisControl.Width / 2,
                        otherControl.Location.Y - thisControl.Height - CheckGap(gap));

                public static void RightCenter(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X + otherControl.Width - thisControl.Width / 2,
                        otherControl.Location.Y - thisControl.Height - CheckGap(gap));

                public static void Right(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X + otherControl.Width,
                        otherControl.Location.Y - thisControl.Height - CheckGap(gap));
            }

            public static class Bottom
            {
                public static void Left(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X,
                        otherControl.Location.Y + otherControl.Height + CheckGap(gap));

                public static void Center(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X,
                        otherControl.Location.Y + otherControl.Height + CheckGap(gap));

                public static void LeftCenter(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X - thisControl.Width / 2,
                        otherControl.Location.Y + otherControl.Height + CheckGap(gap));

                public static void RightCenter(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X + otherControl.Width - thisControl.Width / 2,
                        otherControl.Location.Y + otherControl.Height + CheckGap(gap));

                public static void Right(Control thisControl, Control otherControl, int? gap = null) =>
                    thisControl.Location = new Point(
                        otherControl.Location.X + otherControl.Width,
                        otherControl.Location.Y + otherControl.Height + CheckGap(gap));
            }
        }

        public static class GenericControls
        {
            public static CustomButton Button(float? fontSize = null, string text = null, Color? forecolor = null, Size? size = null)
            {
                return new CustomButton()
                {
                    UseCompatibleTextRendering = true,
                    TabStop = false,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = RGB(20),
                    Size = size ?? new Size(24, 24),
                    Font = new Font(VCROSDMONO, fontSize ?? 20f, FontStyle.Regular),
                    ForeColor = forecolor ?? Color.White,
                    Text = text ?? string.Empty,
                };
            }
        }

        public static class Obscure
        {
            public static CustomPanel DrawOverTrackBarRail(TrackBar bar)
            {
                return new CustomPanel()
                {
                    Location = new Point(bar.Bounds.X + 8, bar.Bounds.Y + 8),
                    Size = new Size(bar.Width - 16, 4),
                    BackColor = Color.RoyalBlue,
                };
            }
        }
    }
}
