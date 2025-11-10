using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class CustomPanel : Panel
    {
        public bool Draggable { get; set; } = false;

        public CustomPanel(bool draggable = false)
        {
            Draggable = draggable;

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

        private void DoMouseDown(MouseEventArgs e)
        {
            if (!Draggable) return;
            CursorPosition = Cursor.Position;
            if (e.Button == MouseButtons.Right)
            {
                Dragging = true;
                CursorPosition = Cursor.Position;
            }
        }

        private void DoMouseUp(MouseEventArgs e)
        {
            if (!Draggable) return;
            Dragging = false;
        }

        private void DoMouseMove(MouseEventArgs e)
        {
            if (!Draggable) return;
            if (!Dragging) return;
            if (Parent == null && Parent.ClientRectangle == Rectangle.Empty) return;

            Rectangle parentBounds = Parent.ClientRectangle;

            if (Dragging)
            {
                int deltaX = Cursor.Position.X - CursorPosition.X;
                int deltaY = Cursor.Position.Y - CursorPosition.Y;

                int nx = Math.Max(parentBounds.Left, Math.Min(Location.X + deltaX, parentBounds.Right - Width));
                int ny = Math.Max(parentBounds.Top, Math.Min(Location.Y + deltaY, parentBounds.Bottom - Height));

                Bounds = new Rectangle(
                    new Point(
                        nx,
                        ny),
                    Size);

                CursorPosition = Cursor.Position;
                Invalidate();
                //Parent.Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            DoMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            DoMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            DoMouseMove(e);
        }

        private void WireControl(Control control)
        {
            control.MouseDown += DescendantMouseDown;
            control.MouseUp += DescendantMouseUp;
            control.MouseMove += DescendantMouseMove;

            if (control.HasChildren)
                foreach (Control c in control.Controls)
                    WireControl(c);
        }

        private void UnwireControl(Control control)
        {
            control.MouseDown -= DescendantMouseDown;
            control.MouseUp -= DescendantMouseUp;
            control.MouseMove -= DescendantMouseMove;

            if (control.HasChildren)
                foreach (Control c in control.Controls)
                    UnwireControl(c);
        }

        private void DescendantMouseDown(object sender, MouseEventArgs e) => DoMouseDown(e);
        private void DescendantMouseUp(object sender, MouseEventArgs e) => DoMouseUp(e);
        private void DescendantMouseMove(object sender, MouseEventArgs e) => DoMouseMove(e);

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            WireControl(e.Control);
        }
    }
}
