using Graphics2D.Geometry;
using Graphics2D.Graphics;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Graphics2D
{
    /// <summary>
    /// 二维场景类
    /// </summary>
    public sealed class View2D : IDisposable, IPersistable
    {
        private bool panning;
        private Point2D lastMouseLocationWorld;
        private Draw2DBase mouseDownItem;
        private Draw2DBase mouseDownCPItem;
        private ControlPoint mouseDownCP;
        private ControlPoint activeCP;
        private Renderer renderer;

        private View.Grid viewGrid = new View.Grid();
        private View.XYAxis viewXYAxis = new View.XYAxis();
        private View.Cursor viewCursor = new View.Cursor();
        private bool showGrid = true;
        private bool showXYAxis = true;
        private bool showCursor = true;

        [Category("Behavior"), DefaultValue(true), Description("指示控件是否响应交互式用户输入.")]
        public bool Interactive { get; set; } = true;

        [Browsable(false)]
        [Category("Appearance"), DefaultValue(true), Description("确定查看位置.")]
        public Camera Camera { get; private set; }

        [Category("Appearance"), DefaultValue(true), Description("确定是否显示笛卡尔网格")]
        public bool ShowGrid
        {
            get => showGrid;
            set
            {
                showGrid = value;
                Redraw();
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("确定是否显示X轴和Y轴.")]
        public bool ShowXYAxis
        {
            get => showXYAxis;
            set
            {
                showXYAxis = value;
                Redraw();
            }
        }

        [Category("Appearance"), DefaultValue(true), Description("确定是否显示光标.")]
        public bool ShowCursor
        {
            get => showCursor;
            set
            {
                showCursor = value;
                Redraw();
            }
        }

        [Browsable(false)]
        public int Width { get; private set; }
        [Browsable(false)]
        public int Height { get; private set; }

        [Browsable(false)]
        public Control Control { get; private set; }

        [Browsable(false)]
        public Document Document { get; private set; }

        [Browsable(false)]
        public Point2D CursorLocation { get; private set; }

        internal Draw2DBases.Draw2DBaseList VisibleItems { get; private set; } = new Draw2DBases.Draw2DBaseList();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="ctrl">控件</param>
        /// <param name="document">文档</param>
        public View2D(Control ctrl, Document document)
        {
            Control = ctrl;
            Document = document;

            Width = 1;
            Height = 1;

            Camera = new Camera(new Point2D(0, 0), 5.0f / 3.0f);
            renderer = new Renderer(this);
            renderer.Init(Control);
            Redraw();

            panning = false;

            Width = ctrl.ClientRectangle.Width;
            Height = ctrl.ClientRectangle.Height;

            Control.Resize += View2D_Resize;
            Control.MouseDown += View2D_MouseDown;
            Control.MouseUp += View2D_MouseUp;
            Control.MouseMove += View2D_MouseMove;
            Control.MouseClick += View2D_MouseClick;
            Control.MouseDoubleClick += View2D_MouseDoubleClick;
            Control.MouseWheel += View2D_MouseWheel;
            Control.KeyDown += View2D_KeyDown;
            Control.KeyPress += View2D_KeyPress;
            Control.Paint += View2D_Paint;
            Control.MouseEnter += View2D_MouseEnter;
            Control.MouseLeave += View2D_MouseLeave;

            Document.DocumentChanged += Document_Changed;
            Document.TransientsChanged += Document_TransientsChanged;
            Document.SelectionChanged += Document_SelectionChanged;
            Document.Editor.Prompt += Editor_Prompt;
            Document.Editor.Error += Editor_Error;
        }

        public void Redraw()
        {
            Control.Invalidate();
        }

        public void Render(System.Drawing.Graphics graphics)
        {
            renderer.InitFrame(graphics);
            renderer.Clear(Document.Settings.BackColor);

            if (showGrid && viewGrid.Visible) renderer.Draw(viewGrid);
            if (showXYAxis && viewXYAxis.Visible) renderer.Draw(viewXYAxis);

            VisibleItems.Clear();
            renderer.Draw(Document.Model);

            DrawSelection(renderer);

            DrawJigged(renderer);

            renderer.Draw(Document.Transients);

            if (showCursor && viewCursor.Visible) renderer.Draw(viewCursor);

            DrawSnapPoint(renderer);

            renderer.EndFrame(graphics);
        }

        private void DrawSelection(Renderer renderer)
        {
            renderer.StyleOverride = new Style(Document.Settings.SelectionHighlightColor, 5, DashStyle.Solid);
            // Current selection
            foreach (Draw2DBase selected in Document.Editor.CurrentSelection)
            {
                renderer.Draw(selected);
            }
            // Picked objects
            foreach (Draw2DBase selected in Document.Editor.PickedSelection)
            {
                renderer.Draw(selected);
            }
            renderer.StyleOverride = null;

            Style cpStyle = new Style(Document.Settings.ControlPointColor, 2);
            Style cpActiveStyle = new Style(Document.Settings.ActiveControlPointColor, 2);
            float cpSize = ScreenToWorld(new Vector2D(Document.Settings.ControlPointSize, 0)).X;

            foreach (Draw2DBase selected in Document.Editor.PickedSelection)
            {
                foreach (ControlPoint pt in selected.GetControlPoints())
                {
                    renderer.DrawRectangle(pt.Equals(activeCP) ? cpActiveStyle : cpStyle,
                        new Point2D(pt.Location.X - cpSize / 2, pt.Location.Y - cpSize / 2),
                        new Point2D(pt.Location.X + cpSize / 2, pt.Location.Y + cpSize / 2));
                }
            }
        }
        /// <summary>
        /// 绘制捕捉点
        /// </summary>
        /// <param name="renderer"></param>
        private void DrawSnapPoint(Renderer renderer)
        {
            if (!Document.Editor.SnapPoints.IsEmpty)
            {
                var pt = Document.Editor.SnapPoints.Current();
                Style style = new Style(Document.Settings.SnapPointColor, 2);
                float size = ScreenToWorld(new Vector2D(Document.Settings.SnapPointSize, 0)).X;

                switch (pt.Type)
                {
                    case SnapPointType.End:
                        renderer.DrawRectangle(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y + size / 2));
                        break;
                    case SnapPointType.Middle:
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y - size / 2),0);
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2),0);
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2),
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y - size / 2),0);
                        break;
                    case SnapPointType.Point:
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y + size / 2),0);
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y + size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y - size / 2),0);
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2),0);
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y),0);
                        break;
                    case SnapPointType.Center:
                        renderer.DrawCircle(style, pt.Location, size / 2);
                        break;
                    case SnapPointType.Quadrant:
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X, pt.Location.Y - size / 2),
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y),0);
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X + size / 2, pt.Location.Y),
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2),0);
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X, pt.Location.Y + size / 2),
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y),0);
                        renderer.DrawLine(style,
                            new Point2D(pt.Location.X - size / 2, pt.Location.Y),
                            new Point2D(pt.Location.X, pt.Location.Y - size / 2),0);
                        break;
                }
            }
        }

        private void DrawJigged(Renderer renderer)
        {
            renderer.StyleOverride = new Style(Document.Settings.JigColor, 0, DashStyle.Dash);
            renderer.Draw(Document.Jigged);
            renderer.StyleOverride = null;
        }
        #region 坐标转换
        /// <summary>
        /// 将给定点从世界坐标转换为屏幕坐标。
        /// </summary>
        /// <param name="x">X </param>
        /// <param name="y">Y </param>
        /// <returns>A Point in screen coordinates.</returns>
        public Point2D WorldToScreen(float x, float y)
        {
            return new Point2D(((x - Camera.Position.X) / Camera.Zoom) + Width / 2,
                -((y - Camera.Position.Y) / Camera.Zoom) + Height / 2);
        }
        /// <summary>
        /// 将给定点从世界坐标转换为屏幕坐标。
        /// </summary>
        /// <param name="pt">世界坐标系位置</param>
        /// <returns>屏幕坐标</returns>
        public Point2D WorldToScreen(Point2D pt) { return WorldToScreen(pt.X, pt.Y); }
        /// <summary>
        /// 将给定向量从世界坐标转换为屏幕坐标.
        /// </summary>
        /// <param name="sz">世界坐标Vector2D</param>
        /// <returns>屏幕坐标系Vector2D</returns>
        public Vector2D WorldToScreen(Vector2D sz)
        {
            Point2D pt1 = WorldToScreen(0.0f, 0.0f);
            Point2D pt2 = WorldToScreen(sz.X, sz.Y);
            return (pt2 - pt1);
        }

        /// <summary>
        /// 将给定点从屏幕坐标转换为世界坐标。
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns></returns>
        public Point2D ScreenToWorld(float x, float y)
        {
            return new Point2D((x - Width / 2) * Camera.Zoom + Camera.Position.X,
                -(y - Height / 2) * Camera.Zoom + Camera.Position.Y);
        }
        /// <summary>
        /// 将给定点从屏幕坐标转换为世界坐标
        /// </summary>
        /// <param name="pt">屏幕坐标系</param>
        /// <returns></returns>
        public Point2D ScreenToWorld(Point2D pt) { return ScreenToWorld(pt.X, pt.Y); }
        /// <summary>
        /// 将给定向量从屏幕坐标转换为世界坐标。 
        /// </summary>
        /// <param name="sz"></param>
        /// <returns></returns>
        public Vector2D ScreenToWorld(Vector2D sz)
        {
            Point2D pt1 = ScreenToWorld(0, 0);
            Point2D pt2 = ScreenToWorld(sz.X, sz.Y);
            return (pt2 - pt1);
        }
        #endregion
        /// <summary>
        /// 返回视口在世界坐标中的坐标。 
        /// </summary>
        public Extents2D GetViewport()
        {
            Extents2D ex = new Extents2D();
            ex.Add((ScreenToWorld(new Point2D(0, 0))));
            ex.Add((ScreenToWorld(new Point2D(Width, Height))));
            return ex;
        }

        /// <summary>
        /// 将视口设置为给定的模型坐标。
        /// </summary>
        /// <param name="x1"> 模型坐标中视口左下角的X坐标 </param>
        /// <param name="y1"> 模型坐标中视口左下角的Y坐标</param>
        /// <param name="x2">模型坐标中视口右上角的X坐标.</param>
        /// <param name="y2">模型坐标中视口右上角的Y坐标</param>
        public void SetViewport(float x1, float y1, float x2, float y2)
        {
            SetViewport(new Extents2D(x1, y1, x2, y2));
        }

        /// <summary>
        /// 将视口设置为给定的模型坐标
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public void SetViewport(Point2D p1, Point2D p2)
        {
            SetViewport(new Extents2D(p1.X, p1.Y, p2.X, p2.Y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limits"></param>
        public void SetViewport(Extents2D limits)
        {
            Camera.Position = limits.Center;
            if ((Height != 0) && (Width != 0))
                Camera.Zoom = Math.Max(limits.Height / Height, limits.Width / Width);
            else
                Camera.Zoom = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetViewport()
        {
            Extents2D limits = Document.Model.GetExtents();
            if (limits.IsEmpty) limits = new Extents2D(-250, -250, 250, 250);

            SetViewport(limits);
            ZoomOut();
        }
        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="zoomFactor"></param>
        public void Zoom(float zoomFactor)
        {
            Zoom(zoomFactor, Camera.Position);
        }
        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="pt"></param>
        public void Zoom(float zoomFactor, Point2D pt)
        {
            Camera.Zoom *= zoomFactor;
            Camera.Position = Camera.Position - (pt - Camera.Position) * (zoomFactor - 1F);
        }
        /// <summary>
        /// 缩小
        /// </summary>
        public void ZoomIn()
        {
            Zoom(0.9f);
        }
        /// <summary>
        /// 缩小
        /// </summary>
        /// <param name="pt"></param>
        public void ZoomIn(Point2D pt)
        {
            Zoom(0.9f, pt);
        }
        /// <summary>
        /// 放大
        /// </summary>
        public void ZoomOut()
        {
            Zoom(1.1f);
        }
        /// <summary>
        /// 放大
        /// </summary>
        /// <param name="pt"></param>
        public void ZoomOut(Point2D pt)
        {
            Zoom(1.1f, pt);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        public void Pan(Vector2D distance)
        {
            Camera.Position += distance;
        }
        /// <summary>
        /// 重置尺寸
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;

            if (renderer != null)
                renderer.Resize(width, height);
        }
        /// <summary>
        /// 文件选中切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Document_SelectionChanged(object sender, EventArgs e)
        {
            Redraw();
        }
        /// <summary>
        /// 文件改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Document_Changed(object sender, EventArgs e)
        {
            Redraw();
        }

        private void Document_TransientsChanged(object sender, EventArgs e)
        {
            Redraw();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_Prompt(object sender, EditorPromptEventArgs e)
        {
            viewCursor.Message = e.Status;
            Redraw();
        }
        /// <summary>
        /// 编辑器错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_Error(object sender, EditorErrorEventArgs e)
        {
            viewCursor.Message = e.Error.Message;
            Redraw();
        }
        /// <summary>
        /// 重置控件尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View2D_Resize(object sender, EventArgs e)
        {
            Resize(Control.ClientRectangle.Width, Control.ClientRectangle.Height);
            Redraw();
        }
        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View2D_MouseDown(object sender, MouseEventArgs e)
        {
            View2D_CursorDown(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }
        /// <summary>
        /// 鼠标抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View2D_MouseUp(object sender, MouseEventArgs e)
        {
            View2D_CursorUp(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }
        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View2D_MouseMove(object sender, MouseEventArgs e)
        {
            View2D_CursorMove(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }
        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View2D_MouseClick(object sender, MouseEventArgs e)
        {
            View2D_CursorClick(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }
        /// <summary>
        /// 鼠标滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View2D_MouseWheel(object sender, MouseEventArgs e)
        {
            View2D_CursorWheel(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }
        /// <summary>
        /// 鼠标双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View2D_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            View2D_CursorDoubleClick(sender, new CursorEventArgs(e.Button, e.Clicks, ScreenToWorld(e.X, e.Y), e.Delta));
        }
        /// <summary>
        /// 光标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View2D_CursorDown(object sender, CursorEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && Interactive)
            {
                panning = true;
                lastMouseLocationWorld = e.Location;
            }
            else if (e.Button == MouseButtons.Left && Interactive && !Document.Editor.InputMode)
            {
                mouseDownItem = FindItem(e.Location, ScreenToWorld(new Vector2D(Document.Settings.PickBoxSize, 0)).X);
                Tuple<Draw2DBase, ControlPoint> find = FindControlPoint(e.Location, ScreenToWorld(new Vector2D(Document.Settings.ControlPointSize, 0)).X);
                mouseDownCPItem = find.Item1;
                mouseDownCP = find.Item2;
                
            }

        }
        /// <summary>
        /// 光标抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void View2D_CursorUp(object sender, CursorEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && Interactive && panning)
            {
                panning = false;
                Redraw();
            }
            else if (e.Button == MouseButtons.Left && Interactive && !Document.Editor.InputMode)
            {
                if (mouseDownItem != null)
                {
                    Draw2DBase mouseUpItem = FindItem(e.Location, ScreenToWorld(new Vector2D(Document.Settings.PickBoxSize, 0)).X);
                    if (mouseUpItem != null && ReferenceEquals(mouseDownItem, mouseUpItem))//&& !Document.Editor.PickedSelection.Contains(mouseDownItem)
                    {
                        if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
                        {
                            Document.Editor.PickedSelection.Remove(mouseDownItem);
                        }
                        else
                        {
                            float cpSize = ScreenToWorld(new Vector2D(Document.Settings.ControlPointSize + 4, 0)).X;
                            Document.Editor.PickedSelection.Clear();
                            
                            Document.Editor.PickedSelection.Add(mouseDownItem);
                            Document.Editor.InputMode = false;
                        }
                    }
                }

                if (mouseDownCP != null)
                {
                    if (!Document.Editor.InputMode)
                    {
                        mouseDownItem = null;
                        mouseDownCPItem = null;
                        mouseDownCP = null;
                        return;
                    }
                    Tuple<Draw2DBase, ControlPoint> find = FindControlPoint(e.Location, ScreenToWorld(new Vector2D(Document.Settings.ControlPointSize, 0)).X);
                    Draw2DBase item = find.Item1;
                    ControlPoint mouseUpCP = find.Item2;
                    if (ReferenceEquals(item, mouseDownCPItem) && mouseDownCP.Index == mouseUpCP.Index)
                    {
                        activeCP = mouseDownCP;
                        ControlPoint cp = mouseDownCP;
                        Draw2DBase consItem = item.Clone();
                        Document.Transients.Add(consItem);
                        ResultMode result = ResultMode.Cancel;
                        Matrix2D trans = Matrix2D.Identity;
                        if (cp.Type == ControlPointType.Point)
                        {
                            var res = await Document.Editor.GetPoint(cp.Name, cp.BasePoint,
                                (p) =>
                                {
                                    consItem.TransformControlPoints(new int[] { cp.Index }, trans.Inverse);
                                    trans = Matrix2D.Translation(p - cp.BasePoint);
                                    consItem.TransformControlPoints(new int[] { cp.Index }, trans);
                                });
                            trans = Matrix2D.Translation(res.Value - cp.BasePoint);
                            result = res.Result;
                            Document.Editor.InputMode = false;
                        }
                       
                        // Transform the control point
                        if (result == ResultMode.OK)
                        {
                            item.TransformControlPoints(new int[] { cp.Index }, trans);
                        }
                        Document.Transients.Remove(consItem);
                        activeCP = null;
                    }
                }
                mouseDownItem = null;
                mouseDownCPItem = null;
                mouseDownCP = null;                
            }
        }
        /// <summary>
        ///二维场景 光标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View2D_CursorMove(object sender, CursorEventArgs e)
        {
            CursorLocation = e.Location;
            viewCursor.Location = CursorLocation;
            Redraw();

            if (e.Button == MouseButtons.Middle && panning)
            {
                // Relative mouse movement
                Point2D scrPt = WorldToScreen(e.Location);
                Pan(lastMouseLocationWorld - CursorLocation);
                lastMouseLocationWorld = ScreenToWorld(scrPt);
                Redraw();
            }

            if (Document.Editor.InputMode)
            {
                Document.Editor.OnViewMouseMove(this, e);
            }
        }
        /// <summary>
        /// 二维场景光标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View2D_CursorClick(object sender, CursorEventArgs e)
        {
            if (Document.Editor.InputMode)
            {                
                Document.Editor.OnViewMouseClick(this, e);
            }
            Document.Editor.PickedSelection.Clear();
        }
        /// <summary>
        /// 二维场景光标滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View2D_CursorWheel(object sender, CursorEventArgs e)
        {
            if (Interactive)
            {
                if (e.Delta > 0)
                {
                    ZoomIn(e.Location);
                }
                else
                {
                    ZoomOut(e.Location);
                }
                Redraw();
            }
        }
        /// <summary>
        /// 二维场景双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void View2D_CursorDoubleClick(object sender, CursorEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && Interactive)
            {
                SetViewport();
            }
        }
        /// <summary>
        /// 二维场景鼠标移开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View2D_MouseLeave(object sender, EventArgs e)
        {
            viewCursor.Visible = false;
            Cursor.Show();

            Redraw();
        }
        /// <summary>
        /// 二维场景鼠标进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View2D_MouseEnter(object sender, EventArgs e)
        {
            viewCursor.Visible = true;
            Cursor.Hide();

            if (!ReferenceEquals(Document.ActiveView, this))
                Document.ActiveView = this;

            Redraw();
        }
        /// <summary>
        /// 二维场景键入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View2D_KeyDown(object sender, KeyEventArgs e)
        {
            if (Document.Editor.InputMode)
            {
                Document.Editor.OnViewKeyDown(this, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Document.Editor.PickedSelection.Clear();
                viewCursor.Message = "";
            }
        }

        private void View2D_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Document.Editor.InputMode)
            {
                Document.Editor.OnViewKeyPress(this, e);
            }
        }

        void View2D_Paint(object sender, PaintEventArgs e)
        {
            Render(e.Graphics);
        }
        /// <summary>
        /// 查找项
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="pickBox"></param>
        /// <returns></returns>
        private Draw2DBase FindItem(Point2D pt, float pickBox)
        {
            float pickBoxWorld = ScreenToWorld(new Vector2D(pickBox, 0)).X;
            foreach (Draw2DBase d in VisibleItems)
            {
                if (d.Contains(pt, pickBoxWorld)) 
                    return d;
            }
            return null;
        }
        /// <summary>
        /// 查找控件中点
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="controlPointSize"></param>
        /// <returns></returns>
        private Tuple<Draw2DBase, ControlPoint> FindControlPoint(Point2D pt, float controlPointSize)
        {
            foreach (Draw2DBase item in Document.Editor.PickedSelection)
            {
                int i = 0;
                foreach (ControlPoint cp in item.GetControlPoints())
                {
                    cp.Index = i;
                    i++;
                    if (pt.X >= cp.Location.X - controlPointSize / 2 && pt.X <= cp.Location.X + controlPointSize / 2 &&
                        pt.Y >= cp.Location.Y - controlPointSize / 2 && pt.Y <= cp.Location.Y + controlPointSize / 2)
                        return new Tuple<Draw2DBase, ControlPoint>(item, cp);
                }
            }
            return new Tuple<Draw2DBase, ControlPoint>(null, null);
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (Document != null)
            {
                Document.DocumentChanged -= Document_Changed;
                Document.TransientsChanged -= Document_TransientsChanged;
                Document.SelectionChanged -= Document_SelectionChanged;
                Document.Editor.Prompt -= Editor_Prompt;
                Document.Editor.Error -= Editor_Error;
            }

            if (Control != null)
            {
                Control.Resize -= View2D_Resize;
                Control.MouseDown -= View2D_MouseDown;
                Control.MouseUp -= View2D_MouseUp;
                Control.MouseMove -= View2D_MouseMove;
                Control.MouseClick -= View2D_MouseClick;
                Control.MouseDoubleClick -= View2D_MouseDoubleClick;
                Control.MouseWheel -= View2D_MouseWheel;
                Control.KeyDown -= View2D_KeyDown;
                Control.KeyPress -= View2D_KeyPress;
                Control.Paint -= View2D_Paint;
                Control.MouseEnter -= View2D_MouseEnter;
                Control.MouseLeave -= View2D_MouseLeave;
            }

            if (renderer != null)
            {
                renderer.Dispose();
                renderer = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Image ToBitmap()
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                Render(g);
                g.Flush();
            }
            return bmp;
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="reader"></param>
        public void Load(DocumentReader reader)
        {
            Camera = reader.ReadCamera();
            ShowGrid = reader.ReadBoolean();
            ShowXYAxis = reader.ReadBoolean();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="writer"></param>
        public void Save(DocumentWriter writer)
        {
            writer.Write(Camera);
            writer.Write(ShowGrid);
            writer.Write(ShowXYAxis);
        }
    }
}
