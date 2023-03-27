using Graphics2D.Geometry;
using Graphics2D.Graphics;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Graphics2D
{
    /// <summary>
    /// 绘制基类，用于绘制点，线，轴，网格等
    /// </summary>
    public abstract class Draw2DBase : INotifyPropertyChanged, IPersistable
    {
        /// <summary>
        /// 布局
        /// </summary>
        public Lazy<Layer> layerRef = new Lazy<Layer>(() => Layer.Default);
        /// <summary>
        /// 样式
        /// </summary>
        public Style Style { get; set; } = Style.Default;
        /// <summary>
        /// 图层
        /// </summary>
        public Layer Layer { get => layerRef.Value; set => layerRef = new Lazy<Layer>(() => value); }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visible { get; set; } = true;
        /// <summary>
        /// 是否是模型,当加载文件时需设置为true,否则不可选中
        /// </summary>
        internal bool InModel { get; set; } = false;
        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 画
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void Draw(Renderer renderer);
        /// <summary>
        /// 获取2D范围
        /// </summary>
        /// <returns></returns>
        public abstract Extents2D GetExtents();
        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="pickBoxSize"></param>
        /// <returns></returns>
        public virtual bool Contains(Point2D pt, float pickBoxSize) { return GetExtents().Contains(pt); }
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="transformation"></param>
        public abstract void TransformBy(Matrix2D transformation);
        /// <summary>
        /// 获取点
        /// </summary>
        /// <returns></returns>
        public virtual ControlPoint[] GetControlPoints() { return new ControlPoint[0]; }
        public virtual ControlPoint[] GetStretchPoints() { return GetControlPoints(); }
        public virtual SnapPoint[] GetSnapPoints() { return new SnapPoint[0]; }
        /// <summary>
        /// 转换点
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="transformation"></param>
        public virtual void TransformControlPoints(int[] indices, Matrix2D transformation) { }
        public virtual void TransformStretchPoints(int[] indices, Matrix2D transformation) { TransformControlPoints(indices, transformation); }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public virtual Draw2DBase Clone() { return (Draw2DBase)MemberwiseClone(); }
        /// <summary>
        /// 构造
        /// </summary>
        protected Draw2DBase() { }
        /// <summary>
        /// 属性改变
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="reader"></param>
        public virtual void Load(DocumentReader reader)
        {
            var doc = reader.Document;
            string layerName = reader.ReadString();
            layerRef = new Lazy<Layer>(() => doc.Layers[layerName]);
            Style = reader.ReadPersistable<Style>();
            Visible = reader.ReadBoolean();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Save(DocumentWriter writer)
        {
            writer.Write(Layer.Name);
            writer.Write(Style);
            writer.Write(Visible);
        }
    }
}
