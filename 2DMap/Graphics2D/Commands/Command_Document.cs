using System.Threading.Tasks;

namespace Graphics2D.Commands
{
    /// <summary>
    /// 场景文件新建类
    /// </summary>
    public class DocumentNew : Command
    {
        public override string RegisteredName => "Document.New";
        public override string Name => "New";

        public override Task Apply(Document doc, params string[] args)
        {
            doc.New();
            return Task.FromResult(default(object));
        }
    }
    /// <summary>
    /// 场景文件打开类
    /// </summary>
    public class DocumentOpen : Command
    {
        public override string RegisteredName => "Document.Open";
        public override string Name => "Open";

        public override async Task Apply(Document doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            var res = await ed.GetOpenFilename("Open file");
            if (res.Result == ResultMode.OK)
            {
                doc.Open(res.Value);
            }
        }
    }
    /// <summary>
    /// 场景文件保存类
    /// </summary>
    public class DocumentSave : Command
    {
        public override string RegisteredName => "Document.Save";
        public override string Name => "Save";

        public override async Task Apply(Document doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            string filename = doc.FileName;
            if (string.IsNullOrEmpty(filename))
            {
                var res = await ed.GetSaveFilename("Save file");
                if (res.Result == ResultMode.OK)
                    filename = res.Value;
                else
                    return;
            }
            doc.Save(filename);
        }
    }

    /// <summary>
    /// 场景文件另存类
    /// </summary>
    public class DocumentSaveAs : Command
    {
        public override string RegisteredName => "Document.SaveAs";
        public override string Name => "Save As";

        public override async Task Apply(Document doc, params string[] args)
        {
            Editor ed = doc.Editor;
            ed.PickedSelection.Clear();

            var res = await ed.GetSaveFilename("Save file");
            if (res.Result == ResultMode.OK)
            {
                doc.Save(res.Value);
            }
        }
    }
}
