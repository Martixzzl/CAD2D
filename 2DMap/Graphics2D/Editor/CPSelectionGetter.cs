using Graphics2D.Draw2DBases;
using Graphics2D.Geometry;
using Graphics2D.Graphics;

namespace Graphics2D
{
    internal class CPSelectionGetter : EditorGetter<CPSelectionOptions, CPSelectionSet>
    {
        protected override void Init(InitArgs<CPSelectionSet> args)
        {
            Editor.PickedSelection.Clear();
           
        }

        protected override void CoordsChanged(Point2D pt)
        {
            SetCursorText(pt.ToString(Editor.Document.Settings.NumberFormat));           
        }

        protected override void AcceptCoordsInput(InputArgs<Point2D, CPSelectionSet> args)
        {
           
        }

        protected override void AcceptTextInput(InputArgs<string, CPSelectionSet> args)
        {
           
        }

        protected override void CancelInput()
        {
           
        }
    }
}
