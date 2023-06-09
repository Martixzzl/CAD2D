﻿using Graphics2D.Geometry;
using Graphics2D.Graphics;
using System;
using System.IO;

namespace Graphics2D
{
    /// <summary>
    /// 文件读操作类
    /// </summary>
    public sealed class DocumentReader : IDisposable
    {
        BinaryReader reader;

        public Document Document { get; private set; }

        public DocumentReader(Document document, Stream stream)
        {
            Document = document;
            reader = new BinaryReader(stream);
        }

        public bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }

        public float ReadFloat()
        {
            return reader.ReadSingle();
        }

        public int ReadInt()
        {
            return reader.ReadInt32();
        }

        public uint ReadUInt()
        {
            return reader.ReadUInt32();
        }

        public string ReadString()
        {
            return reader.ReadString();
        }

        public Point2D ReadPoint2D()
        {
            return new Point2D(ReadFloat(), ReadFloat());
        }

        public Point2DCollection ReadPoint2DCollection()
        {
            Point2DCollection points = new Point2DCollection();
            int count = ReadInt();
            for (int i = 0; i < count; i++)
                points.Add(ReadPoint2D());
            return points;
        }

        public Vector2D ReadVector2D()
        {
            return new Vector2D(ReadFloat(), ReadFloat());
        }

        public Color ReadColor()
        {
            bool isByLayer = ReadBoolean();
            return new Color(ReadUInt(), isByLayer);
        }

        public Camera ReadCamera()
        {
            Point2D position = ReadPoint2D();
            float zoom = ReadFloat();
            return new Camera(position, zoom);
        }

        public T ReadPersistable<T>() where T : IPersistable
        {
            string typeName = ReadString();
            Type itemType = Type.GetType(typeName);
            T item = (T)Activator.CreateInstance(itemType);
            item.Load(this);
            return item;
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
