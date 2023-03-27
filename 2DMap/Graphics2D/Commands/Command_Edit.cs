using System.Collections.Generic;
using System.Threading.Tasks;

namespace Graphics2D.Commands
{
    /// <summary>
    /// 编辑器删除对象类
    /// </summary>
    public class EditDelete : Command
    {
        public override string RegisteredName => "Edit.Delete";
        public override string Name => "Delete";

        public override async Task Apply(Document doc, params string[] args)
        {
            Editor ed = doc.Editor;

            var s = await ed.GetSelection("Select objects: ");
            if (s.Result != ResultMode.OK || s.Value.Count == 0) return;
            List<Draw2DBase> toDelete = new List<Draw2DBase>();
            foreach (Draw2DBase item in s.Value)
            {
                toDelete.Add(item);
            }
            foreach (Draw2DBase item in toDelete)
            {
                doc.Model.Remove(item);
            }
        }
    }
}
