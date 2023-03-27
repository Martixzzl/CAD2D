using Graphics2D.Geometry;
using System;
using System.Windows.Forms;

namespace Graphics2D
{
    public delegate void CursorEventHandler(object sender, CursorEventArgs e);
    /// <summary>
    /// 视图事件类
    /// </summary>
    public class CursorEventArgs : EventArgs
    {
        /// <summary>
        /// 鼠标按钮
        /// </summary>
        public MouseButtons Button { get; private set; }
        /// <summary>
        /// 位置
        /// </summary>
        public Point2D Location { get; private set; }
        /// <summary>
        /// 点击
        /// </summary>
        public int Clicks { get; private set; }
        public int Delta { get; private set; }

        public float X { get { return Location.X; } }
        public float Y { get { return Location.Y; } }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="button"></param>
        /// <param name="clicks"></param>
        /// <param name="location"></param>
        /// <param name="delta"></param>
        public CursorEventArgs(MouseButtons button, int clicks, Point2D location, int delta)
        {
            Button = button;
            Location = location;
            Clicks = clicks;
            Delta = delta;
        }

        public CursorEventArgs(MouseButtons button, int clicks, float x, float y, int delta)
            : this(button, clicks, new Point2D(x, y), delta)
        {
            ;
        }
    }
}
