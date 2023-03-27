using Graphics2D.Draw2DBases;
using Graphics2D.Geometry;
using System.Threading.Tasks;

namespace Graphics2D.Commands
{
    /// <summary>
    /// 绘制点命令
    /// </summary>
    public class DrawPoint : Command
    {
        public override string RegisteredName => "Primitives.Point";
        public override string Name => "Point";

        public override async Task Apply(Document doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            while (true)
            {
                var p1 = await ed.GetPoint("Location: ");
                if (p1.Result != ResultMode.OK) return;

                if (p1.Result == ResultMode.OK)
                {
                    Draw2DBase point = new Point(p1.Value);
                    doc.Model.Add(point);
                }
                else
                {
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 绘制线命令
    /// </summary>
    public class DrawLine : Command
    {
        public override string RegisteredName => "Primitives.Line";
        public override string Name => "Line";

        public override async Task Apply(Document doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            var p1 = await ed.GetPoint("First point: ");
            if (p1.Result != ResultMode.OK) return;
            Point2D lastPt = p1.Value;

            int i = 0;
            while (true)
            {
                var opts = new PointOptions("Next point: ", lastPt);
                if (i > 1)
                    opts.AddKeyword("Close");
                var p3 = await ed.GetPoint(opts);

                if (p3.Result == ResultMode.OK)
                {
                    Draw2DBase nextLine = new Line(lastPt, p3.Value);
                    doc.Model.Add(nextLine);

                    lastPt = p3.Value;
                }
                else if (p3.Result == ResultMode.Keyword && p3.Keyword == "Close")
                {
                    Draw2DBase nextLine = new Line(lastPt, p1.Value);
                    doc.Model.Add(nextLine);

                    lastPt = p3.Value;
                    return;
                }
                else
                {
                    return;
                }
                i++;
            }
        }
    }   
}

