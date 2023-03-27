using System.Threading.Tasks;

namespace Graphics2D.Commands
{
    /// <summary>
    /// 清除所有选中类
    /// </summary>
    public class SelectionClear : Command
    {
        public override string RegisteredName => "Selection.Clear";
        public override string Name => "Clear Selection";

        public override Task Apply(Document doc, params string[] args)
        {
            doc.Editor.CurrentSelection.Clear();
            return Task.FromResult(default(object));
        }
    }
}
