using Graphics2D.Geometry;
using Graphics2D.Graphics;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Graphics2D.Draw2DBases
{
    public class Composite : Draw2DBase, ICollection<Draw2DBase>, INotifyCollectionChanged
    {
        public string Name { get; set; }

        List<Draw2DBase> items = new List<Draw2DBase>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Composite() { }

        public Composite(string name)
        {
            Name = name;
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            Name = reader.ReadString();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Draw2DBase item = reader.ReadPersistable<Draw2DBase>();
                item.InModel = true;
                items.Add(item);
            }
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Name);
            writer.Write(items.Count);
            foreach (var item in items)
            {
                writer.Write(item);
            }
        }

        public override void Draw(Renderer renderer)
        {
            foreach (Draw2DBase item in items)
            {
                if (item.Visible && (item.Layer == null || item.Layer.Visible))
                {
                    renderer.Draw(item);
                }
            }
        }

        /// <summary>
        /// 获取2D范围
        /// </summary>
        /// <returns></returns>
        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();
            foreach (Draw2DBase item in items)
            {
                if (item.Visible && (item.Layer == null || item.Layer.Visible))
                    extents.Add(item.GetExtents());
            }
            return extents;
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="pickBoxSize"></param>
        /// <returns></returns>
        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            foreach (Draw2DBase d in items)
            {
                if (d.Visible && (d.Layer == null || d.Layer.Visible) && d.Contains(pt, pickBoxSize))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取捕捉点
        /// </summary>
        /// <returns></returns>
        public override SnapPoint[] GetSnapPoints()
        {
            List<SnapPoint> points = new List<SnapPoint>();
            foreach (Draw2DBase d in items)
            {
                if (d.Visible && (d.Layer == null || d.Layer.Visible))
                    points.AddRange(d.GetSnapPoints());
            }
            return points.ToArray();
        }

        public override void TransformBy(Matrix2D transformation)
        {
            foreach (Draw2DBase item in items)
            {
                item.TransformBy(transformation);
            }
        }

        public override Draw2DBase Clone()
        {
            Composite newComposite = (Composite)base.Clone();
            foreach (Draw2DBase d in items)
            {
                newComposite.Add(d.Clone());
            }
            return newComposite;
        }

        public virtual void Add(Draw2DBase item)
        {
            items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public virtual void Clear()
        {
            items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public virtual bool Contains(Draw2DBase item)
        {
            return items.Contains(item);
        }

        public virtual void CopyTo(Draw2DBase[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public virtual int Count => items.Count;
        public bool IsReadOnly => false;

        public virtual bool Remove(Draw2DBase item)
        {
            bool check = items.Remove(item);
            if (check)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
            return check;
        }

        public virtual IEnumerator<Draw2DBase> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Draw2DBase item in e.NewItems)
                {
                    item.PropertyChanged += Draw2DBase_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Draw2DBase item in e.OldItems)
                    item.PropertyChanged -= Draw2DBase_PropertyChanged;
            }

            CollectionChanged?.Invoke(this, e);
        }

        void Draw2DBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender));
        }
    }
}
