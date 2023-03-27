using Graphics2D.Geometry;
using Graphics2D.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Graphics2D.Draw2DBases
{
    public class Draw2DBaseList : Draw2DBase, IList<Draw2DBase>
    {
        List<Draw2DBase> items = new List<Draw2DBase>();

        public Draw2DBase this[int index] { get => items[index]; set => items[index] = value; }
        public int Count => items.Count;
        public bool IsReadOnly => false;

        public Draw2DBaseList() { }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                Type itemType = Type.GetType(name);
                Draw2DBase item = (Draw2DBase)Activator.CreateInstance(itemType, reader);
                items.Add(item);
            }
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(items.Count);
            foreach (Draw2DBase item in items)
            {
                writer.Write(item.GetType().FullName);
                item.Save(writer);
            }
        }

        public void Add(Draw2DBase value)
        {
            items.Add(value);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(Draw2DBase item)
        {
            return items.Contains(item);
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

        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();
            foreach (Draw2DBase item in items)
            {
                if (item.Visible && item.Layer.Visible)
                    extents.Add(item.GetExtents());
            }
            return extents;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            foreach (Draw2DBase d in items)
            {
                if (d.Visible && d.Layer.Visible && d.Contains(pt, pickBoxSize)) return true;
            }
            return false;
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
            Draw2DBaseList newList = (Draw2DBaseList)base.Clone();
            foreach (Draw2DBase item in items)
            {
                newList.Add(item.Clone());
            }
            return newList;
        }

        public IEnumerator<Draw2DBase> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public bool Remove(Draw2DBase item)
        {
            return items.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(Draw2DBase item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, Draw2DBase item)
        {
            items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public void CopyTo(Draw2DBase[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }
    }
}
