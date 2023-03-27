using Graphics2D.Geometry;
using Graphics2D.Graphics;

namespace Graphics2D.View
{
    /// <summary>
    /// XY轴，用于画横向X和竖直向Y轴
    /// </summary>
    internal class XYAxis : Draw2DBase
    {
        public override void Draw(Renderer renderer)
        {
            var view = renderer.View;
            var doc = view.Document;

            Extents2D bounds = view.GetViewport();
            Color axisColor = doc.Settings.AxisColor;

            renderer.DrawLine(new Style(axisColor), new Point2D(0, bounds.Ymin), new Point2D(0, bounds.Ymax),1);
            renderer.DrawLine(new Style(axisColor), new Point2D(bounds.Xmin, 0), new Point2D(bounds.Xmax, 0),1);
        }

        public override Extents2D GetExtents()
        {
            return Extents2D.Empty;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            ;
        }
    }
}
