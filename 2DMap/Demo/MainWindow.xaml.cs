using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 文件
        /// </summary>
        private Graphics2D.Document doc;
        /// <summary>
        /// 编辑器
        /// </summary>
        private Graphics2D.Editor ed;
        /// <summary>
        /// 当前选中对象
        /// </summary>
        private Graphics2D.SelectionSet currentSelection;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private string SaveFileName
        {
            get
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                return System.IO.Path.Combine(path, "save.2dGra");
            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            windowsFormsHost.Width = this.ActualWidth - host2.ActualWidth;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
           
            doc = mainWindow.Document;
            //mainWindow.Height = (int)(mainWindow.Height - status.ActualHeight);
            ed = doc.Editor;

            doc.DocumentChanged += Doc_DocumentChanged; ;
            doc.SelectionChanged += Doc_SelectionChanged; ;
            mainWindow.MouseMove += MainWindow_MouseMove;
        }
        /// <summary>
        /// 窗体鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //实时显示鼠标位置
            statusCoords.Text = $"实时位置：{ mainWindow.View.CursorLocation.ToString(doc.Settings.NumberFormat)}";
        }

        /// <summary>
        /// 选中切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Doc_SelectionChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObjects = ed.PickedSelection.ToArray();
            currentSelection = ed.PickedSelection;
            UpdateUI();
        }
        /// <summary>
        /// 文件改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Doc_DocumentChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 画点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drawPoint_Click(object sender, RoutedEventArgs e)
        {
            ed.RunCommand("Primitives.Point");
        }
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drawLine_Click(object sender, RoutedEventArgs e)
        {
            ed.RunCommand("Primitives.Line");
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            ed.RunCommand("Edit.Delete");
        }

        public void OuputDatas(object sender, RoutedEventArgs e)
        {

            if (ed.Document.Model.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(
                          "不存在数据！",
                          "提示!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "点线数据（*.json）|*.json";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                Datas datas = new Datas();
                foreach (var item in doc.Model)
                {
                    if (item.GetType().Name.Equals("Point"))
                    {
                        Point point = new Point()
                        {
                            X = (item as dynamic).X,
                            Y = (item as dynamic).Y
                        };
                        datas.Point.Add(point);
                    }
                    else
                    {
                        Line line = new Line()
                        {
                            StartPoint_X = (item as dynamic).X1,
                            StartPoint_Y = (item as dynamic).Y1,
                            EndPoint_X = (item as dynamic).X2,
                            EndPoint_Y = (item as dynamic).Y2
                        };
                        datas.Line.Add(line);
                    }
                }

                File.WriteAllText(sfd.FileName, JsonConvert.SerializeObject(datas));
            }


        }

        private void UpdateUI()
        {
            if (ed.PickedSelection.Count == 0)
            {
                lblSelection.Content = "No selection";
                return;
            }


            try
            {
                string obj = ed.PickedSelection.ToArray().Last().GetType().Name;
                if (obj.Equals("Line"))
                {
                    lblSelection.Content = $"已选择Line;起点：(X:{(ed.PickedSelection.Last() as dynamic).X1};Y:{(ed.PickedSelection.Last() as dynamic).Y1});终点：(X:{(ed.PickedSelection.Last() as dynamic).X2};Y:{(ed.PickedSelection.Last() as dynamic).Y2})";
                }
                else if (obj.Equals("Point"))
                {
                    lblSelection.Content = $"已选择Point;坐标：(X:{(ed.PickedSelection.Last() as dynamic).X};Y:{(ed.PickedSelection.ToArray()[0] as dynamic).Y})";
                }
            }
            catch (Exception)
            {


            }

        }
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            if (EnsureDocumentSaved())
                ed.RunCommand("Document.Open", SaveFileName);
            UpdateUI();
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(doc.FileName))
                ed.RunCommand("Document.SaveAs", SaveFileName);
            else
                ed.RunCommand("Document.Save");
            UpdateUI();
        }

        private bool EnsureDocumentSaved()
        {
            if (!doc.IsModified)
                return true;

            DialogResult res = System.Windows.Forms.MessageBox.Show(
                "是否保存文件?",
                "提示!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (res == System.Windows.Forms.DialogResult.Cancel)
                return false;
            else if (res == System.Windows.Forms.DialogResult.No)
                return true;
            else
            {
                ed.RunCommand("Document.Save");
                return !doc.IsModified;
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            mainWindow.Refresh();
        }

        private void mainWindow_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ed.RunCommand("Edit.Delete");
            }
        }
    }

    public class Datas
    {
        public List<Point> Point { get; set; } = new List<Point>();

        public List<Line> Line { get; set; } = new List<Line>();
    }

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Line
    {
        public double StartPoint_X { get; set; }
        public double StartPoint_Y { get; set; }
        public double EndPoint_X { get; set; }
        public double EndPoint_Y { get; set; }
    }
}
