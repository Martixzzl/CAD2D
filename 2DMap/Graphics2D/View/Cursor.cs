using Graphics2D.Geometry;
using Graphics2D.Graphics;
using System;

namespace Graphics2D.View
{
    /// <summary>
    /// 光标实现类
    /// </summary>
    internal class Cursor : Draw2DBase
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Point2D Location { get; set; }
        public TextStyle TextStyle { get; set; }
        public float TextHeight { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        public Cursor()
        {
            // Assign the default system font by default
            TextStyle = new TextStyle("_Cursor", System.Drawing.SystemFonts.MessageBoxFont.FontFamily.Name, FontStyle.Regular);
            // Default text height in pixels
            TextHeight = 12;
        }
        /// <summary>
        /// 画
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(Renderer renderer)
        {
            var view = renderer.View;
            var doc = view.Document;

            Extents2D ex = view.GetViewport();
            Color c = doc.Settings.BackColor;
            var luma = (int)Math.Sqrt(c.R * c.R * .299 + c.G * c.G * .587 + c.B * c.B * .114);
            Style cursorStyle = new Style(luma > 130 ? Color.Black : Color.White);
            float emptyBoxSize = view.ScreenToWorld(new Vector2D(doc.Settings.PickBoxSize + 4, 0)).X / 2;
            float pickBoxSize = view.ScreenToWorld(new Vector2D(doc.Settings.PickBoxSize, 0)).X / 2;
            float pxSize = view.ScreenToWorld(new Vector2D(1, 0)).X / 2;

            // Draw cursor
            if (!doc.Editor.InputMode)
            {
                renderer.DrawLine(cursorStyle, new Point2D(ex.Xmin, Location.Y), new Point2D(Location.X - emptyBoxSize, Location.Y),0);
                renderer.DrawLine(cursorStyle, new Point2D(Location.X + emptyBoxSize, Location.Y), new Point2D(ex.Xmax, Location.Y),0);
                renderer.DrawLine(cursorStyle, new Point2D(Location.X, ex.Ymin), new Point2D(Location.X, Location.Y - emptyBoxSize),0);
                renderer.DrawLine(cursorStyle, new Point2D(Location.X, Location.Y + emptyBoxSize), new Point2D(Location.X, ex.Ymax),0);
                renderer.DrawRectangle(cursorStyle, new Point2D(Location.X - pickBoxSize, Location.Y - pickBoxSize),
                    new Point2D(Location.X + pickBoxSize, Location.Y + pickBoxSize));
            }
            else
            {
                renderer.DrawLine(cursorStyle, new Point2D(ex.Xmin, Location.Y), new Point2D(Location.X - pxSize, Location.Y),0);
                renderer.DrawLine(cursorStyle, new Point2D(Location.X + pxSize, Location.Y), new Point2D(ex.Xmax, Location.Y),0);
                renderer.DrawLine(cursorStyle, new Point2D(Location.X, ex.Ymin), new Point2D(Location.X, Location.Y - pxSize),0);
                renderer.DrawLine(cursorStyle, new Point2D(Location.X, Location.Y + pxSize), new Point2D(Location.X, ex.Ymax),0);
                
            }

            if (!string.IsNullOrEmpty(Message))
            {
                float height = Math.Abs(view.ScreenToWorld(new Vector2D(0, TextHeight)).Y);
                float margin = Math.Abs(view.ScreenToWorld(new Vector2D(4, 0)).X);
                float offset = Math.Abs(view.ScreenToWorld(new Vector2D(2, 0)).X);

                float x = Location.X + margin + offset;
                float y = Location.Y - margin - offset;
                Vector2D sz = renderer.MeasureString(Message, TextStyle, height);
                Point2D lowerRight = new Point2D(ex.Xmax, ex.Ymin);
                if (x + sz.X + offset > lowerRight.X)
                {
                    x = Location.X - margin - offset - sz.X;
                }
                if (y - sz.Y - offset < lowerRight.Y)
                {
                    y = Location.Y + margin + offset + sz.Y;
                }

                Style fore = new Style(doc.Settings.CursorPromptForeColor);
                Style back = new Style(doc.Settings.CursorPromptBackColor);
                renderer.FillRectangle(back, new Point2D(x - offset, y + offset), new Point2D(x + offset + sz.X, y - offset - sz.Y));
                renderer.DrawRectangle(fore, new Point2D(x - offset, y + offset), new Point2D(x + offset + sz.X, y - offset - sz.Y));
                renderer.DrawString(fore, new Point2D(x, y), Message, TextStyle, height, 0,
                    TextHorizontalAlignment.Left, TextVerticalAlignment.Top);
            }
        }

        public override Extents2D GetExtents()
        {
            return Extents2D.Infinity;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            ;
        }
    }
}
