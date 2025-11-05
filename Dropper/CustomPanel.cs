using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class CustomPanel : Panel
    {
        public bool Draggable { get; set; } = false;
        public Rectangle ParentBounds { get; set; } = Rectangle.Empty; // intended to be used in tandem with the drag logic
        public bool Added { get; set; } = false;

        public CustomPanel()
        {
            DoubleBuffered = true;
            TabStop = false;
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
        protected override bool ShowFocusCues => false;

        private bool Dragging;
        private Point CursorPosition;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!Draggable) return;
            CursorPosition = Cursor.Position;
            if (e.Button == MouseButtons.Right)
            {
                Dragging = true;
                CursorPosition = Cursor.Position;
            }
            //if (HasChildren)
            //    foreach (Control control in Controls)
            //        control.MouseDown += (s, ev) =>
            //        {
            //            if (ev.Button == MouseButtons.Right)
            //            {
            //                Dragging = true;
            //                CursorPosition = Cursor.Position;
            //            }
            //        };
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!Draggable) return;
            Dragging = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!Draggable) return;
            if (ParentBounds == Rectangle.Empty) return;

            if (Dragging)
            {
                int deltaX = Cursor.Position.X - CursorPosition.X;
                int deltaY = Cursor.Position.Y - CursorPosition.Y;

                int nx = Math.Max(ParentBounds.Left, Math.Min(Location.X + deltaX, ParentBounds.Right - Width));
                int ny = Math.Max(ParentBounds.Top, Math.Min(Location.Y + deltaY, ParentBounds.Bottom - Height));

                Bounds = new Rectangle(
                    new Point(
                        nx,
                        ny),
                    Size);

                CursorPosition = Cursor.Position;
                Invalidate();
            }
        }
    }
}
