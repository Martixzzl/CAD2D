using System.Collections.Generic;
using System.Globalization;
using Graphics2D.Graphics;

namespace Graphics2D
{
    /// <summary>
    /// 全局设置类
    /// </summary>
    public partial class Settings : IPersistable
    {
        public static Dictionary<string, object> Defaults
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { "DisplayPrecision", 2 },
                    { "BackColor", Color.FromArgb(33,40,48) },
                    { "CursorPromptBackColor", Color.FromArgb(84,58,84) },
                    { "CursorPromptForeColor", Color.FromArgb(128,Color.White) },
                    { "SelectionWindowColor", Color.FromArgb(64,46,116,251) },
                    { "SelectionWindowBorderColor", Color.White },
                    { "ReverseSelectionWindowColor", Color.FromArgb(64,46,251,116) },
                    { "ReverseSelectionWindowBorderColor", Color.White },
                    { "SelectionHighlightColor", Color.FromArgb(64,46,116,251) },
                    { "JigColor", Color.Orange },
                    { "ControlPointColor", Color.FromArgb(46,116,251) },
                    { "ActiveControlPointColor", Color.FromArgb(251,116,46) },
                    { "SnapPointColor", Color.FromArgb(251,251,116) },
                    { "MinorGridColor", Color.FromArgb(64,64,64) },
                    { "MajorGridColor", Color.FromArgb(96,96,96) },
                    { "AxisColor", Color.FromArgb(128,128,64) },
                    { "PickBoxSize", 10 },
                    { "ControlPointSize", 7 },
                    { "PointSize", 6 },
                    { "Snap", true },
                    { "SnapPointSize", 11 },
                    { "SnapDistance", 25 },
                    { "SnapMode", SnapPointType.All },
                    { "AngleMode", AngleMode.Degrees },
                };
            }
        }

        private Dictionary<string, object> items = new Dictionary<string, object>();

        public NumberFormatInfo NumberFormat { get; private set; }

        public Settings()
        {
            Reset();
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set(string name, object value)
        {
            if (items.ContainsKey(name))
            {
                items[name] = value;
            }
            else
            {
                items.Add(name, value);
            }
        }

        public object Get(string name)
        {
            return items[name];
        }

        /// <summary>
        /// 泛型属性获取器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name)
        {
            return (T)Get(name);
        }

        /// <summary>
        /// 重置设置
        /// </summary>
        public void Reset()
        {
            items.Clear();
            foreach (var pair in Defaults)
            {

                items.Add(pair.Key, pair.Value);
            }

            UpdateSettings();
        }

        public void Load(DocumentReader reader)
        {
            items = new Dictionary<string, object>();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                string valueType = reader.ReadString();
                object value = null;
                if (valueType == "bool")
                {
                    value = reader.ReadBoolean();
                }
                else if (valueType == "int")
                {
                    value = reader.ReadInt();
                }
                else if (valueType == "color")
                {
                    value = reader.ReadColor();
                }

                items.Add(name, value);
            }

            UpdateSettings();
        }

        public void Save(DocumentWriter writer)
        {
            writer.Write(items.Count);
            foreach (var pair in items)
            {
                writer.Write(pair.Key);
                if (pair.Value is bool)
                {
                    writer.Write("bool");
                    writer.Write((bool)pair.Value);
                }
                else if (pair.Value is int || pair.Value is System.Enum)
                {
                    writer.Write("int");
                    writer.Write((int)pair.Value);
                }
                else if (pair.Value is Color)
                {
                    writer.Write("color");
                    writer.Write((Color)pair.Value);
                }
            }
        }

        private void UpdateSettings()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = Get<int>("DisplayPrecision");
            nfi.NumberDecimalSeparator = ".";
            NumberFormat = nfi;
        }

        public System.Int32 DisplayPrecision
        {
            get => Get<System.Int32>("DisplayPrecision");
            set => Set("DisplayPrecision", value);
        }
        public Graphics2D.Graphics.Color BackColor
        {
            get => Get<Graphics2D.Graphics.Color>("BackColor");
            set => Set("BackColor", value);
        }
        public Graphics2D.Graphics.Color CursorPromptBackColor
        {
            get => Get<Graphics2D.Graphics.Color>("CursorPromptBackColor");
            set => Set("CursorPromptBackColor", value);
        }
        public Graphics2D.Graphics.Color CursorPromptForeColor
        {
            get => Get<Graphics2D.Graphics.Color>("CursorPromptForeColor");
            set => Set("CursorPromptForeColor", value);
        }
        public Graphics2D.Graphics.Color SelectionWindowColor
        {
            get => Get<Graphics2D.Graphics.Color>("SelectionWindowColor");
            set => Set("SelectionWindowColor", value);
        }
        public Graphics2D.Graphics.Color SelectionWindowBorderColor
        {
            get => Get<Graphics2D.Graphics.Color>("SelectionWindowBorderColor");
            set => Set("SelectionWindowBorderColor", value);
        }
        public Graphics2D.Graphics.Color ReverseSelectionWindowColor
        {
            get => Get<Graphics2D.Graphics.Color>("ReverseSelectionWindowColor");
            set => Set("ReverseSelectionWindowColor", value);
        }
        public Graphics2D.Graphics.Color SelectionHighlightColor
        {
            get => Get<Graphics2D.Graphics.Color>("SelectionHighlightColor");
            set => Set("SelectionHighlightColor", value);
        }
        public Graphics2D.Graphics.Color JigColor
        {
            get => Get<Graphics2D.Graphics.Color>("JigColor");
            set => Set("JigColor", value);
        }
        public Graphics2D.Graphics.Color ControlPointColor
        {
            get => Get<Graphics2D.Graphics.Color>("ControlPointColor");
            set => Set("ControlPointColor", value);
        }
        public Graphics2D.Graphics.Color ActiveControlPointColor
        {
            get => Get<Graphics2D.Graphics.Color>("ActiveControlPointColor");
            set => Set("ActiveControlPointColor", value);
        }
        public Graphics2D.Graphics.Color SnapPointColor
        {
            get => Get<Graphics2D.Graphics.Color>("SnapPointColor");
            set => Set("SnapPointColor", value);
        }
        public Graphics2D.Graphics.Color MinorGridColor
        {
            get => Get<Graphics2D.Graphics.Color>("MinorGridColor");
            set => Set("MinorGridColor", value);
        }
        public Graphics2D.Graphics.Color MajorGridColor
        {
            get => Get<Graphics2D.Graphics.Color>("MajorGridColor");
            set => Set("MajorGridColor", value);
        }
        public Graphics2D.Graphics.Color AxisColor
        {
            get => Get<Graphics2D.Graphics.Color>("AxisColor");
            set => Set("AxisColor", value);
        }
        public System.Int32 PickBoxSize
        {
            get => Get<System.Int32>("PickBoxSize");
            set => Set("PickBoxSize", value);
        }
        public System.Int32 ControlPointSize
        {
            get => Get<System.Int32>("ControlPointSize");
            set => Set("ControlPointSize", value);
        }
        public System.Int32 PointSize
        {
            get => Get<System.Int32>("PointSize");
            set => Set("PointSize", value);
        }
        public System.Boolean Snap
        {
            get => Get<System.Boolean>("Snap");
            set => Set("Snap", value);
        }
        public System.Int32 SnapPointSize
        {
            get => Get<System.Int32>("SnapPointSize");
            set => Set("SnapPointSize", value);
        }
        public System.Int32 SnapDistance
        {
            get => Get<System.Int32>("SnapDistance");
            set => Set("SnapDistance", value);
        }
        public Graphics2D.SnapPointType SnapMode
        {
            get => Get<Graphics2D.SnapPointType>("SnapMode");
            set => Set("SnapMode", value);
        }
        public Graphics2D.AngleMode AngleMode
        {
            get => Get<Graphics2D.AngleMode>("AngleMode");
            set => Set("AngleMode", value);
        }
    }
}
