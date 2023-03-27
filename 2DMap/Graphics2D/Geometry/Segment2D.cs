using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Graphics2D.Geometry
{
    [TypeConverter(typeof(Segment2DConverter))]
    public struct Segment2D
    {
        public Point2D P1 { get; }
        public Point2D P2 { get; }

        public float X1 => P1.X;
        public float Y1 => P1.Y;
        public float X2 => P2.X;
        public float Y2 => P2.Y;

        public Vector2D Direction => (P2 - P1).Normal;
        public float Length => (P2 - P1).Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment2D(Point2D p1, Point2D p2)
        {
            P1 = p1;
            P2 = p2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment2D(float x1, float y1, float x2, float y2)
            : this(new Point2D(x1, y1), new Point2D(x2, y2))
        {
            ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Segment2D Transform(Matrix2D transformation)
        {
            Point2D p1 = P1.Transform(transformation);
            Point2D p2 = P2.Transform(transformation);
            return new Segment2D(p1, p2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Segment2D operator +(Segment2D s, Vector2D v)
        {
            return new Segment2D(s.P1 + v, s.P2 + v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Segment2D operator -(Segment2D s, Vector2D v)
        {
            return new Segment2D(s.P1 - v, s.P2 - v);
        }

        public bool Contains(Point2D pt, float tolerance, out float t)
        {
            Vector2D w = pt - P1;
            Vector2D vL = (P2 - P1);
            t = w.DotProduct(vL) / vL.DotProduct(vL);
            float dist = (w - t * vL).Length;

            if (dist < tolerance)
            {
                ////3个边的长
                //double ab = GetDistance(pt.X, pt.Y, P2.X, P2.Y);
                //double ac = GetDistance(pt.X, pt.Y, P1.X, P1.Y);
                //double bc = GetDistance(P1.X, P1.Y, P2.X, P2.Y);

                ////半周长
                //double p = (ab + ac + bc) / 2;

                ////面积(海仑公式)
                //double s = Math.Sqrt(p * (p - ab) * (p - ac) * (p - bc));

                ////高
                //double h = 2 * s / bc;

                float dx = P1.X - P2.X;
                float dy = P1.Y - P2.Y;

                float u = (pt.X - P1.X) * dx + (pt.Y - P1.Y) * dy;
                u = u / ((dx * dx) + (dy * dy));

                float newX = P1.X + u * dx;
                float newY = P1.Y + u * dy;

                Point2D pf = new Point2D(newX, newY);

                if (GetPointIsInLine(pf, P1, P2, 0.01))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            return false ;
        }

        /// <summary>
        /// 判断点是否在直线上
        /// </summary>
        /// <param name="pf"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="range"></param>
        /// <returns></returns>

        public static bool GetPointIsInLine(Point2D pf, Point2D p1, Point2D p2, double range)
        {
            //range 判断的的误差，不需要误差则赋值0
            //点在线段首尾两端之外则return false

            double cross = (p2.X - p1.X) * (pf.X - p1.X) + (p2.Y - p1.Y) * (pf.Y - p1.Y);
            if (cross <= 0) return false;
            double d2 = (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y);
            if (cross >= d2) return false;
            double r = cross / d2;
            double px = p1.X + (p2.X - p1.X) * r;
            double py = p1.Y + (p2.Y - p1.Y) * r;

            //判断距离是否小于误差
            return Math.Sqrt((pf.X - px) * (pf.X - px) + (py - pf.Y) * (py - pf.Y)) <= range;
        }

        private double GetDistance(double A_x, double A_y, double B_x, double B_y)
        {
            double x = System.Math.Abs(B_x - A_x);
            double y = System.Math.Abs(B_y - A_y);
            return Math.Sqrt(x * x + y * y);
        }


        public string ToString(IFormatProvider provider)
        {
            return ToString("{0:F}, {1:F} : {2:F}, {3:F}", provider);
        }

        public string ToString(string format = "{0:F}, {1:F} : {2:F}, {3:F}", IFormatProvider provider = null)
        {
            return (provider == null) ?
                string.Format(format, X1, Y1, X2, Y2) :
                string.Format(provider, format, X1, Y1, X2, Y2);
        }

        public static bool TryParse(string s, out Segment2D result)
        {
            Segment2DConverter conv = new Segment2DConverter();
            if (conv.IsValid(s))
            {
                result = (Segment2D)conv.ConvertFrom(s);
                return true;
            }
            else
            {
                result = new Segment2D();
                return false;
            }
        }
    }
}
