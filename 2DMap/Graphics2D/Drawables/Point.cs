using Graphics2D.Geometry;
using Graphics2D.Graphics;
using System.ComponentModel;

namespace Graphics2D.Draw2DBases
{
    public class Point : Draw2DBase
    {
        private Point2D p;

        public Point2D Location { get => p; set { p = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return Location.X; } }
        [Browsable(false)]
        public float Y { get { return Location.Y; } }

        public Point() { }

        public Point(Point2D location)
        {
            Location = location;
        }

        public Point(float x, float y)
            : this(new Point2D(x, y))
        {
            ;
        }

        public override void Draw(Renderer renderer)
        {
            float size = renderer.View.ScreenToWorld(new Vector2D(renderer.View.Document.Settings.PointSize, 0)).X ;
            renderer.DrawCircle(Style.ApplyLayer(Layer), Location, size);
        }

        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();
            extents.Add(X, Y);
            return extents;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            Location = Location.Transform(transformation);
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            float dist = (pt - Location).Length;
            return dist <= pickBoxSize / 2;
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Location", Location),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            return new[]
            {
                new SnapPoint("Start point", SnapPointType.Point, Location),
            };
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                if (index == 0)
                    Location = Location.Transform(transformation);
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            Location = reader.ReadPoint2D();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Location);
        }
    }
}
