using System.Collections.Generic;
using System.Linq;

namespace Graphics2D
{
    public sealed class CPSelectionSet
    {
        Dictionary<Draw2DBase, HashSet<int>> items = new Dictionary<Draw2DBase, HashSet<int>>();

        public static CPSelectionSet Empty => new CPSelectionSet();

        public int[] this[Draw2DBase item] => items[item].ToArray();

        public CPSelectionSet()
        {
            ;
        }

        public bool Add(Draw2DBase item, int index)
        {
            if (items.TryGetValue(item, out var indices))
            {
                return indices.Add(index);
            }
            else
            {
                items.Add(item, new HashSet<int>() { index });
                return true;
            }
        }

        public void Add(Draw2DBase item, IEnumerable<int> indices)
        {
            foreach (int index in indices)
            {
                Add(item, index);
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        public int Count { get { return items.Count; } }

        public IEnumerator<KeyValuePair<Draw2DBase, HashSet<int>>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public void UnionWith(CPSelectionSet other)
        {
            foreach (var item in other)
            {
                foreach (int index in item.Value)
                {
                    Add(item.Key, index);
                }
            }
        }

        public SelectionSet ToSelectionSet()
        {
            SelectionSet ss = new SelectionSet();
            foreach (var pair in items)
            {
                ss.Add(pair.Key);
            }
            return ss;
        }
    }
}
