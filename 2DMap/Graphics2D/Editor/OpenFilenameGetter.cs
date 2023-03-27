using System.IO;
using System.Windows.Forms;

namespace Graphics2D
{
    internal class OpenFilenameGetter : EditorGetter<FilenameOptions, string>
    {
        protected override void Init(InitArgs<string> args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = Options.Message;
            ofd.Filter = Options.Filter;
            ofd.DefaultExt = "2dGra";
            string filename = "";
            string path = "";
            if (File.Exists(Options.FileName))
            {
                filename = Path.GetFileName(Options.FileName);
                path = Path.GetDirectoryName(Options.FileName);
            }
            if (!string.IsNullOrEmpty(filename)) ofd.FileName = filename;
            if (!string.IsNullOrEmpty(path)) ofd.InitialDirectory = path;

            if (ofd.ShowDialog() == DialogResult.OK)
                args.Value = ofd.FileName;
            else
                args.InputValid = false;

            args.ContinueAsync = false;
        }
    }
}
