using Graphics2D.Draw2DBases;
using System.Collections.Specialized;

namespace Graphics2D
{
    /// <summary>
    /// Model 模式
    /// </summary>
    public class Model : Composite
    {
        /// <summary>
        /// 文件
        /// </summary>
        public Document Document { get; private set; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="doc"></param>
        public Model(Document doc)
        {
            Document = doc;
            Name = "<MODEL>";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public override void Add(Draw2DBase item)
        {
            item.InModel = true;
            base.Add(item);
        }

        public override bool Remove(Draw2DBase item)
        {
            item.InModel = false;
            return base.Remove(item);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

        }

    }
}
