using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    public class Math {
        public struct Quad {
            public float X;
            public float Y;
            public float W;
            public float H;

            public Quad(List<Vector2> list)
            {
                X = list[0].x;
                Y = list[0].y;
                W = list[2].x - list[0].x;
                H = list[2].y - list[0].y;
            }

            public Quad(float x, float y, float w, float h)
            {
                X = x; Y = y; W = w; H = h;
            }
        }

        public struct Edge {
            public Vector2 P0;
            public Vector2 P1;

            public Edge(Vector2 p0, Vector2 p1)
            {
                P0 = p0; P1 = p1;
            }
            public override bool Equals(object obj)
            {
                var other = (Edge)obj;
                return P0 == other.P0 && P1 == other.P1
                    || P0 == other.P1 && P1 == other.P0;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public struct Polygon {
            public List<Vector2> Ps;

            public int Count { get => Ps.Count; }

            public Vector2 this[int index]
            {
                get => Ps[index];
                set => Ps[index] = value;
            }

            public bool IsVaild()
            {
                return Ps.Count >= 3;
            }

            public Polygon(List<Vector2> ps)
            {
                Ps = ps;
            }
        }

        public struct Triangle {
            public Vector2 P0;
            public Vector2 P1;
            public Vector2 P2;

            public Triangle(List<Vector2> list)
            {
                P0 = list[0]; P1 = list[1]; P2 = list[2];
            }

            public Triangle(Vector2 p0, Vector2 p1, Vector2 p2)
            {
                P0 = p0; P1 = p1; P2 = p2;
            }

            public float Area()
            {
                return V2Cross(P1 - P0, P2 - P1) * 0.5f;
            }

            public override bool Equals(object obj)
            {
                var other = (Triangle)obj;
                return (P0 == other.P0 || P0 == other.P1 || P0 == other.P2)
                    && (P1 == other.P0 || P1 == other.P1 || P1 == other.P2)
                    && (P2 == other.P0 || P2 == other.P1 || P2 == other.P2);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public struct Circular {
            public Vector2 mOrigin;
            public float mRadius;

            public Circular(Vector2 origin, float radius)
            {
                mOrigin = origin;
                mRadius = radius;
            }
        }



        //
        //  2D向量计算
        //
        public static float V2Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        public static float Lerp(float a, float b, float t)
        {
            return (b - a) * t + a;
        }

        //  下一个索引
        public static int IndexNext(int idx, int add, int num)
        {
            return (idx + add) % num;
        }

        //  上一个索引
        public static int IndexPrev(int idx, int add, int num)
        {
            return IndexNext(idx, num - add % num, num);
        }

        //  相交
        //  直线 直线
        public static bool IsCrossSeg(Edge a, Edge b)
        {
            var ab = a.P1 - a.P0;
            var cd = b.P1 - b.P0;
            if (Mathf.Max(a.P0.x, a.P1.x) < Mathf.Min(b.P0.x, b.P1.x)) { return false; }
            if (Mathf.Max(a.P0.y, a.P1.y) < Mathf.Min(b.P0.y, b.P1.y)) { return false; }
            if (Mathf.Min(a.P0.x, a.P1.x) > Mathf.Max(b.P0.x, b.P1.x)) { return false; }
            if (Mathf.Min(a.P0.y, a.P1.y) > Mathf.Max(b.P0.y, b.P1.y)) { return false; }
            return V2Cross(ab, b.P0 - a.P0) * V2Cross(ab, b.P1 - a.P0) <= 0
                && V2Cross(cd, a.P0 - b.P0) * V2Cross(cd, a.P1 - b.P0) <= 0;
        }

        //  线段 线段
        public static bool IsCrossSeg(Edge a, Edge b, ref float u, ref float v)
        {
            if (IsCross(a, b, ref u, ref v))
            {
                return u >= 0.0f && u <= 1.0f
                    && v >= 0.0f && v <= 1.0f;
            }
            return false;
        }

        //  直线 直线
        public static bool IsCross(Edge a, Edge b, ref float u, ref float v)
        {
            var cross = V2Cross(a.P1 - a.P0, b.P1 - b.P0);
            if (!cross.Equals(0.0f))
            {
                u = V2Cross(b.P1 - b.P0, a.P0 - b.P0) / cross;
                v = V2Cross(a.P1 - a.P0, a.P0 - b.P0) / cross;
                return true;
            }
            return false;
        }

        //  线段 矩形
        public static bool IsCross(Edge edge, Quad quad)
        {
            Vector2 p0; p0.x = quad.X; p0.y = quad.Y;
            Vector2 p1; p1.x = quad.X + quad.W; p1.y = quad.Y;
            Vector2 p2; p2.x = quad.X + quad.W; p2.y = quad.Y + quad.H;
            Vector2 p3; p3.x = quad.X; p3.y = quad.Y + quad.H;

            Edge e0; e0.P0 = p0; e0.P1 = p1;
            Edge e1; e1.P0 = p1; e1.P1 = p2;
            Edge e2; e2.P0 = p2; e2.P1 = p3;
            Edge e3; e3.P0 = p3; e3.P1 = p0;
            return IsCrossSeg(edge, e0) || IsCrossSeg(edge, e1)
                || IsCrossSeg(edge, e2) || IsCrossSeg(edge, e3);
        }

        //  矩形 线段
        public static bool IsCross(Quad quad, Edge edge)
        {
            return IsCross(edge, quad);
        }

        //  矩形 矩形
        public static bool IsCross(Quad quad0, Quad quad1)
        {
            var x0 = (quad0.X - quad1.X) * (quad0.X + quad0.W - quad1.X - quad1.W);
            var x1 = (quad1.X - quad0.X) * (quad1.X + quad1.W - quad0.X - quad0.W);
            var y0 = (quad0.Y - quad1.Y) * (quad0.Y + quad0.H - quad1.Y - quad1.H);
            var y1 = (quad1.Y - quad0.Y) * (quad1.Y - quad1.H - quad0.Y - quad0.H);
            return (x0 <= 0 || y0 <= 0)
                && (x1 <= 0 || y1 <= 0);
        }

        //  矩形 圆形
        public static bool IsCross(Quad quad, Circular cir)
        {
            Vector2 v;
            v.x = cir.mOrigin.x - quad.X;
            v.y = cir.mOrigin.y - quad.Y;
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            v.x -= quad.W * 0.5f;
            v.y -= quad.H * 0.5f;
            v.x = Mathf.Max(0, v.x);
            v.y = Mathf.Max(0, v.y);
            return Vector2.Dot(v, v) <= cir.mRadius * cir.mRadius;
        }

        //  矩形 三角形
        public static bool IsCross(Quad quad, Triangle tri)
        {
            Vector2 p0; p0.x = quad.X; p0.y = quad.Y;
            Vector2 p1; p1.x = quad.X + quad.W; p1.y = quad.Y;
            Vector2 p2; p2.x = quad.X + quad.W; p2.y = quad.Y + quad.H;
            Vector2 p3; p3.x = quad.X; p3.y = quad.Y + quad.H;

            Edge qe0; qe0.P0 = p0; qe0.P1 = p1;
            Edge qe1; qe1.P0 = p1; qe1.P1 = p2;
            Edge qe2; qe2.P0 = p2; qe2.P1 = p3;
            Edge qe3; qe3.P0 = p3; qe3.P1 = p0;

            Edge te0; te0.P0 = tri.P0; te0.P1 = tri.P1;
            Edge te1; te1.P0 = tri.P1; te1.P1 = tri.P2;
            Edge te2; te2.P0 = tri.P2; te2.P1 = tri.P0;

            return IsCrossSeg(qe0, te0) || IsCrossSeg(qe0, te1) || IsCrossSeg(qe0, te2)
                || IsCrossSeg(qe1, te0) || IsCrossSeg(qe1, te1) || IsCrossSeg(qe1, te2)
                || IsCrossSeg(qe2, te0) || IsCrossSeg(qe2, te1) || IsCrossSeg(qe2, te2)
                || IsCrossSeg(qe3, te0) || IsCrossSeg(qe3, te1) || IsCrossSeg(qe3, te2);
        }

        //  包含
        //  三角形 点
        public static bool IsContains(Triangle triangle, Vector2 point)
        {
            var p0p1 = triangle.P1 - triangle.P0;
            var p1p2 = triangle.P2 - triangle.P1;
            var p2p0 = triangle.P0 - triangle.P2;
            var c0 = V2Cross(p0p1, p1p2);
            if (c0 * V2Cross(p0p1, point - triangle.P0) < 0) { return false; }
            var c1 = V2Cross(p1p2, p2p0);
            if (c1 * V2Cross(p1p2, point - triangle.P1) < 0) { return false; }
            var c2 = V2Cross(p2p0, p0p1);
            if (c2 * V2Cross(p2p0, point - triangle.P2) < 0) { return false; }
            return true;
        }

        //  三角形 矩形
        public static bool IsContains(Triangle triangle, Quad quad)
        {
            Vector2 p0; p0.x = quad.X; p0.y = quad.Y;
            Vector2 p1; p1.x = quad.X + quad.W; p1.y = quad.Y;
            Vector2 p2; p2.x = quad.X + quad.W; p2.y = quad.Y + quad.H;
            Vector2 p3; p3.x = quad.X; p3.y = quad.Y + quad.H;
            return IsContains(triangle, p0)
                && IsContains(triangle, p1)
                && IsContains(triangle, p2)
                && IsContains(triangle, p3);
        }

        //  矩形 矩形
        public static bool IsContains(Quad quad0, Quad quad1)
        {
            return quad0.X <= quad1.X && quad0.Y <= quad1.Y
                && quad0.W >= quad1.W && quad0.H >= quad1.H;
        }

        //  矩形 圆
        public static bool IsContains(Quad quad, Circular circular)
        {
            var dx = circular.mOrigin.x - quad.X;
            var dy = circular.mOrigin.y - quad.Y;
            return dx >= circular.mRadius && dy >= circular.mRadius;
        }

        //  矩形 点
        public static bool IsContains(Quad quad, Vector2 point)
        {
            return point.x >= quad.X
                && point.y >= quad.Y
                && point.x <= quad.X + quad.W
                && point.y <= quad.Y + quad.H;
        }

        //  矩形 三角形
        public static bool IsContains(Quad quad, Triangle tri)
        {
            return IsContains(quad, tri.P0)
                && IsContains(quad, tri.P1)
                && IsContains(quad, tri.P2);
        }

        //  矩形 线段
        public static bool IsContains(Quad quad, Edge edge)
        {
            return IsContains(quad, edge.P0)
                && IsContains(quad, edge.P1);
        }

        //  凸包 点
        public static bool IsContainsConvex(Polygon polygon, Vector2 p)
        {
            for (var i = 0; i != polygon.Count; ++i)
            {
                var j = IndexNext(i, 1, polygon.Count);
                var k = IndexNext(i, 2, polygon.Count);
                var a = polygon[i];
                var b = polygon[j];
                var c = polygon[k];
                var cross0 = V2Cross(b - a, c - b);
                var cross1 = V2Cross(b - a, p - a);
                if (cross0 * cross1 < 0)
                {
                    return false;
                }
            }
            return true;
        }

        //  极坐标排序
        public static void SortPointByAxis<T>(List<T> list, System.Func<T, Vector2> func)
        {
            var o = 0;
            for (var i = 1; i != list.Count; ++i)
            {
                if (func(list[i]).y < func(list[o]).y) { o = i; }
            }

            var point = list[o];
            list.Sort((a, b) => {
                var ap = func(a) - func(point);
                var bp = func(b) - func(point);
                var a0 = Mathf.Atan2(ap.y, ap.x);
                var a1 = Mathf.Atan2(ap.y, ap.x);
                if (a0 < a1) { return -1; }
                if (a0 > a1) { return  1; }
                if (a0 == a1)
                {
                    if (ap.x > bp.x) { return -1; }
                    if (ap.x < bp.x) { return  1; }
                }
                return 0;
            });
        }

        //  生成凸包
        public static void GenConvex<T>(List<T> list, System.Func<T, Vector2> func, List<T> output)
        {
            output.Add(list[0]);
            output.Add(list[1]);
            for (var i = 1; i != list.Count; ++i)
            {
                var p = list[i];
                while (list.Count > 1)
                {
                    var a = list[list.Count - 2];
                    var b = list[list.Count - 1];
                    if (0 <= V2Cross(func(b) - func(a), func(p) - func(b)))
                    {
                        break; 
                    }
                    output.RemoveAt(list.Count - 1);
                }
                output.Add(p);
            }
        }

        //  计算中点
        public static Vector2 CalcCenterCoord(Polygon polygon)
        {
            var sum = Vector2.zero;
            for (var i = 0; i != polygon.Count; ++i)
            {
                sum += polygon[i];
            }
            return sum / polygon.Count;
        }

        //  计算起点
        //  根据p0, p1给定的方向, 返回合适的起点索引
        public static int CalcFirstIndex(List<Vector2> list, Vector2 p0, Vector2 p1)
        {
            var p0p1  = p1 - p0;
            var index = 0;
            var cross = V2Cross(p0p1, list[index] - p0);
            for (index = 1; index != list.Count; ++index)
            {
                var c = V2Cross(p0p1, list[index] - p0);
                if (c * cross < 0) { break; }
            }
            return index % list.Count;
        }

        //  计算多边形顺序
        public static float CalePointsOrder(List<Vector2> points)
        {
            Debug.Assert(points.Count >= 3);
            var min = 0;
            var ret = 0.0f;
            for (var i = 0; i != points.Count; ++i)
            {
                if (points[i].x <= points[min].x)
                {
                    var prev = (i + points.Count - 1) % points.Count;
                    var next = (i + 1) % points.Count;
                    var a = points[i] - points[prev];
                    var b = points[next] - points[i];
                    var z = V2Cross(a, b);
                    if (z != 0) { ret = z; min = i; }
                }
            }
            return ret;
        }
    }
}