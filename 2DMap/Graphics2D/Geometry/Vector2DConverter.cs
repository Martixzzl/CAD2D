﻿using System;
using System.ComponentModel;

namespace Graphics2D.Geometry
{
    public class Vector2DConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            string str = value as string;

            if (str == null) return false;

            string[] parts = str.Replace(" ", "").Split(';', ',');
            if (parts.Length != 2) return false;
            foreach (string part in parts)
            {
                if (!float.TryParse(part, out _))
                    return false;
            }

            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string str = value as string;

            if (str != null)
            {
                string[] parts = str.Replace(" ", "").Split(';', ',');
                if (parts.Length == 2)
                {
                    float x = float.Parse(parts[0]);
                    float y = float.Parse(parts[1]);
                    return new Vector2D(x, y);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Vector2D)
            {
                Vector2D vec = (Vector2D)value;
                return vec.X.ToString("F2") + "; " + vec.Y.ToString("F2");
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
