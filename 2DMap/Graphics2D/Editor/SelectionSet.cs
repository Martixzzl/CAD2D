using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Graphics2D
{
    public sealed class SelectionSet : ISet<Draw2DBase>, INotifyCollectionChanged
    {
        /// <summary>
        /// 上一个对象
        /// </summary>
        private Draw2DBase lastItem;

        /// <summary>
        /// 
        /// </summary>
        HashSet<Draw2DBase> items = new HashSet<Draw2DBase>();

        /// <summary>
        /// 集合变化事件
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// 静态对象
        /// </summary>
        public static SelectionSet Empty => new SelectionSet();

        public SelectionSet()
        {
            ;
        }

        /// <summary>
        /// 添加，用于多选
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(Draw2DBase item)
        {           
            if (items.Count == 1)
            {

                items.Clear();
                bool check = items.Add(item);
                if (check)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                }
                lastItem = item;
                return check;
            }
            else
            {
                bool check = items.Add(item);
                if (check)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                }
                lastItem = item;
                return check;
            }
        }

        /// <summary>
        /// 清除重置
        /// </summary>
        public void Clear()
        {
            items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// 数组是否包含项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Draw2DBase item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Draw2DBase[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 选中个数
        /// </summary>
        public int Count { get { return items.Count; } }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// 移除选中项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Draw2DBase item)
        {
            bool check = items.Remove(item);
            if (check)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
            return check;
        }

        /// <summary>
        /// 迭代获取
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Draw2DBase> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 集合改变触发
        /// </summary>
        /// <param name="e"></param>
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 实现UnionWith
        /// </summary>
        /// <param name="other"></param>
        public void UnionWith(IEnumerable<Draw2DBase> other)
        {
            foreach(var item in other)
            {
                Add(item);
            }
        }

        #region 未实现接口
        void ISet<Draw2DBase>.IntersectWith(IEnumerable<Draw2DBase> other)
        {
            throw new NotImplementedException();
        }

        void ISet<Draw2DBase>.ExceptWith(IEnumerable<Draw2DBase> other)
        {
            throw new NotImplementedException();
        }

        void ISet<Draw2DBase>.SymmetricExceptWith(IEnumerable<Draw2DBase> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Draw2DBase>.IsSubsetOf(IEnumerable<Draw2DBase> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Draw2DBase>.IsSupersetOf(IEnumerable<Draw2DBase> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Draw2DBase>.IsProperSupersetOf(IEnumerable<Draw2DBase> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Draw2DBase>.IsProperSubsetOf(IEnumerable<Draw2DBase> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Draw2DBase>.Overlaps(IEnumerable<Draw2DBase> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Draw2DBase>.SetEquals(IEnumerable<Draw2DBase> other)
        {
            throw new NotImplementedException();
        }

        void ICollection<Draw2DBase>.Add(Draw2DBase item)
        {
            ((ISet<Draw2DBase>)this).Add(item);
        }
        #endregion
    }
}
