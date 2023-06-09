﻿using Graphics2D.Draw2DBases;
using System;
using System.IO;
using System.Linq;

namespace Graphics2D
{
    /// <summary>
    /// 场景文件类
    /// </summary>
    public class Document
    {
        public delegate void DocumentChangedEventHandler(object sender, EventArgs e);
        public delegate void TransientsChangedEventHandler(object sender, EventArgs e);
        public delegate void SelectionChangedEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Model 存放所有点线元素
        /// </summary>
        public Model Model { get; private set; }
        public Composite Jigged { get; private set; }
        public Composite Transients { get; private set; }
        public Editor Editor { get; private set; }
        public Settings Settings { get; private set; }
        public View2D ActiveView { get; set; }

        public LayerDictionary Layers { get; private set; }
        public TextStyleDictionary TextStyles { get; private set; }
        public CompositeDictionary Composites { get; private set; }

        public string FileName { get; private set; }
        public bool IsModified { get; private set; } = false;

        public event DocumentChangedEventHandler DocumentChanged;
        public event TransientsChangedEventHandler TransientsChanged;
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// 构造
        /// </summary>
        public Document()
        {
            Editor = new Editor(this);

            Settings = new Settings();
            Layers = new LayerDictionary();
            TextStyles = new TextStyleDictionary();
            Composites = new CompositeDictionary();
            Model = new Model(this);
            Jigged = new Composite();
            Transients = new Composite();

            ActiveView = null;

            Editor.PickedSelection.CollectionChanged += Selection_CollectionChanged;
            Model.CollectionChanged += Model_CollectionChanged;
            Jigged.CollectionChanged += Jigged_CollectionChanged;
        }

        /// <summary>
        /// 文件新建
        /// </summary>
        public void New()
        {
            Settings.Reset();
            Layers.Clear();
            TextStyles.Clear();
            Composites.Clear();
            Model.Clear();
            Jigged.Clear();
            Transients.Clear();

            FileName = "";
            IsModified = false;
        }

        /// <summary>
        /// 文件打开
        /// </summary>
        /// <param name="stream"></param>
        public void Open(Stream stream)
        {
            using (var reader = new DocumentReader(this, stream))
            {
                Editor.PickedSelection.Clear();
                Jigged.Clear();
                Transients.Clear();

                Settings.Load(reader);
                Layers.Load(reader);
                TextStyles.Load(reader);
                Composites.Load(reader);
                Model.Load(reader);

                FileName = "";
                IsModified = false;
            }
        }

        /// <summary>
        /// 文件打开
        /// </summary>
        /// <param name="filename"></param>
        public void Open(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Open(stream);
                FileName = filename;
            }
        }

        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            using (var writer = new DocumentWriter(this, stream))
            {
                Settings.Save(writer);
                Layers.Save(writer);
                TextStyles.Save(writer);
                Composites.Save(writer);
                Model.Save(writer);

                FileName = "";
                IsModified = false;
            }
        }

        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(stream);
                FileName = filename;
            }
        }

        /// <summary>
        /// 场景元素集合改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    OnDocumentChanged(new EventArgs());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (Draw2DBase item in e.OldItems.Cast<Draw2DBase>())
                    {
                        Editor.PickedSelection.Remove(item);
                    }
                    OnDocumentChanged(new EventArgs());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Editor.PickedSelection.Clear();
                    OnDocumentChanged(new EventArgs());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    OnDocumentChanged(new EventArgs());
                    break;
            }
        }

        private void Jigged_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnTransientsChanged(new EventArgs());
        }

        /// <summary>
        /// 文件改变
        /// </summary>
        /// <param name="e"></param>
        protected void OnDocumentChanged(EventArgs e)
        {
            IsModified = true;
            DocumentChanged?.Invoke(this, e);
        }

        protected void OnTransientsChanged(EventArgs e)
        {
            TransientsChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 选中改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnSelectionChanged(new EventArgs());
        }

        /// <summary>
        /// 文件改变触发
        /// </summary>
        /// <param name="e"></param>
        protected void OnSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
