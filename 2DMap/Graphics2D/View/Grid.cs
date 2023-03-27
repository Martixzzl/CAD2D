using Graphics2D.Geometry;
using Graphics2D.Graphics;

namespace Graphics2D.View
{
    /// <summary>
    /// 网格实现类
    /// </summary>
    internal class Grid : Draw2DBase
    {
        /// <summary>
        /// 画网格
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(Renderer renderer)
        {
            var view = renderer.View;
            var doc = view.Document;

            float spacing = 1;
            while (view.WorldToScreen(new Vector2D(spacing, 0)).X > 12)
                spacing /= 10;

            while (view.WorldToScreen(new Vector2D(spacing, 0)).X < 4)
                spacing *= 10;

            Extents2D bounds = view.GetViewport();
            Style majorStyle = new Style(doc.Settings.MajorGridColor);
            Style minorStyle = new Style(doc.Settings.MinorGridColor);

            int k = 0;
            for (float i = 0; i > bounds.Xmin; i -= spacing)
            {
                Style style = (k == 0 ? majorStyle : minorStyle);
                k = (k + 1) % 10;
                renderer.DrawLine(style, new Point2D(i, bounds.Ymax), new Point2D(i, bounds.Ymin),0);
            }
            k = 0;
            for (float i = 0; i < bounds.Xmax; i += spacing)
            {
                Style style = (k == 0 ? majorStyle : minorStyle);
                k = (k + 1) % 10;
                renderer.DrawLine(style, new Point2D(i, bounds.Ymax), new Point2D(i, bounds.Ymin),0);
            }
            k = 0;
            for (float i = 0; i < bounds.Ymax; i += spacing)
            {
                Style style = (k == 0 ? majorStyle : minorStyle);
                k = (k + 1) % 10;
                renderer.DrawLine(style, new Point2D(bounds.Xmin, i), new Point2D(bounds.Xmax, i),0);
            }
            k = 0;
            for (float i = 0; i > bounds.Ymin; i -= spacing)
            {
                Style style = (k == 0 ? majorStyle : minorStyle);
                k = (k + 1) % 10;
                renderer.DrawLine(style, new Point2D(bounds.Xmin, i), new Point2D(bounds.Xmax, i),0);
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
