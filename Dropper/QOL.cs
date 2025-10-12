using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public static class QOL
    {
        private static readonly Random random = new Random();
        public static readonly string VCROSDMONO = "VCR OSD Mono";

        public static Color RandomColor() => Color.FromArgb(255, random.Next(256), random.Next(256), random.Next(256));

        public static void WriteOut(object o) => MessageBox.Show($"{o}");
        public static void QuickWriteOut(Func<object> getter, Control surface)
        {
            surface.MouseClick += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Right)
                    WriteOut(getter());
            };
        }

        public static class Colors
        {
            public static Color SameRGB(int value) => Color.FromArgb(255, value, value, value);
            public static Color SameRG(int value) => Color.FromArgb(255, value, value, random.Next(256));
            public static Color SameRB(int value) => Color.FromArgb(255, value, random.Next(256), value);
            public static Color SameGB(int value) => Color.FromArgb(255, random.Next(256), value, value);
            public static Color SameR(int value) => Color.FromArgb(255, value, random.Next(256), random.Next(256));
            public static Color SameG(int value) => Color.FromArgb(255, random.Next(256), value, random.Next(256));
            public static Color SameB(int value) => Color.FromArgb(255, random.Next(256), random.Next(256), value);
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
                        otherControl.Location.X + otherControl.Width / 2 - thisControl.Width / 2,
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
    }
}
