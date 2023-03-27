using Graphics2D.Geometry;
using Graphics2D.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Graphics2D.Draw2DBases
{
    public class Draw2DBaseDictionary : Draw2DBase, IDict<Draw2DBase>
    {
        Dictionary<string, Draw2DBase> items = new Dictionary<string, Draw2DBase>();

        public Draw2DBase this[string key] { get => items[key]; set => items[key] = value; }
        public ICollection<string> Keys => items.Keys;
        public ICollection<Draw2DBase> Values => items.Values;
        public int Count => items.Count;
        public bool IsReadOnly => false;

        public Draw2DBaseDictionary() { }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string name = reader.ReadString();
                Type itemType = Type.GetType(name);
                Draw2DBase item = (Draw2DBase)Activator.CreateInstance(itemType, reader);
                items.Add(key, item);
            }
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(items.Count);
            foreach (KeyValuePair<string, Draw2DBase> item in items)
            {
                writer.Write(item.Key);
                writer.Write(item.Value.GetType().FullName);
                item.Value.Save(writer);
            }
        }

        public void Add(string key, Draw2DBase value)
        {
            items.Add(key, value);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool ContainsKey(string key)
        {
            return items.ContainsKey(key);
        }

        public override void Draw(Renderer renderer)
        {
            foreach (Draw2DBase item in items.Values)
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
            foreach (Draw2DBase item in items.Values)
            {
                if (item.Visible && item.Layer.Visible)
                    extents.Add(item.GetExtents());
            }
            return extents;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            foreach (Draw2DBase d in items.Values)
            {
                if (d.Visible && d.Layer.Visible && d.Contains(pt, pickBoxSize)) return true;
            }
            return false;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            foreach (Draw2DBase item in items.Values)
            {
                item.TransformBy(transformation);
            }
        }

        public override Draw2DBase Clone()
        {
            Draw2DBaseDictionary newDict = (Draw2DBaseDictionary)base.Clone();
            foreach (KeyValuePair<string, Draw2DBase> item in items)
            {
                newDict.Add(item.Key, item.Value.Clone());
            }
            return newDict;
        }

        public IEnumerator<Draw2DBase> GetEnumerator()
        {
            foreach (Draw2DBase item in items.Values)
                yield return item;
        }

        public bool Remove(string key)
        {
            return items.Remove(key);
        }

        public bool TryGetValue(string key, out Draw2DBase value)
        {
            return items.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
