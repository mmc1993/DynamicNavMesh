using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    using Vec2 = Vector2;
    //  路径生成
    public class PathBuild {
        public class Pile {
            public Area  mArea;     //  所属区域
            public Vec2  mOrigin;   //  原点
            public float mRadius;   //  半径
        }

        public class Area {
            public Math.Polygon mVerts;     //  顶点集
            public List<Pile> mPiles;       //  桩集
            public List<Mesh> mMeshs;       //  网格
            public Area mPrev;  //  上级
            public Area mNext;  //  下级
        }

        public class Edge {
            public Vec2 mA;                 //  起点
            public Vec2 mB;                 //  终点
            public Area mLink;              //  链接区域
            public Area mSelf;              //  自己区域
        }

        public class Mesh {
            public Vec2         mOrigin;    //  中点
            public Math.Polygon mVerts;     //  点集
            public List<Edge>   mEdges;     //  边集
        }

        public Area Root()
        {
            return mRoot;
        }

        public void Init(Math.Polygon points)
        {
            mRoot = new Area {
                mVerts = points,
                mPiles = new List<Pile>(),
                mMeshs = new List<Mesh>(),
            };
            foreach (var point in points.Ps)
            {
                mRoot.mPiles.Add(new Pile {
                    mArea = mRoot,
                    mOrigin = point,
                    mRadius = 0.00f,
                });
            }
        }

        public void Insert(Vec2 p, float r)
        {
            var area = FindArea(mRoot, p);

            var point = new Pile {
                mOrigin = p,
                mRadius = r,
            };

            Insert(area, point);
        }

        void Insert(Area area, Pile pile)
        {
            List<Pile> piles = new List<Pile>();

            //  构造桩集
            while (area.mPiles.Count != area.mVerts.Count)
            {
                var i=area.mPiles.Count-1;
                piles.Add(area.mPiles[i]);
                area.mPiles.RemoveAt(i);
            }
            piles.Add(pile);

            //  清空子项
            Clear(area.mNext, piles);
            area.mNext = null;

            var list = new List<Pile>();
            while (piles.Count != 0)
            {
                list.Clear();
                InitPiles(piles, list);
                LinkPiles(piles, list, ref area);
            }
        }

        void FillMesh(Area area, Area prev)
        {

        }

        void FillMesh(Area area, Pile p)
        {
            for (var i = 0; i != area.mVerts.Count;)
            {
                var mesh = new Mesh();
                mesh.mVerts.Ps = new List<Vec2> {
                    p.mOrigin, area.mVerts[i],
                };

                var ab = mesh.mVerts.Ps[1] - mesh.mVerts.Ps[0];
                for (var j = i; j != area.mVerts.Count;++j,++i)
                {
                    var k = j % area.mVerts.Count;
                    var cd = area.mVerts[k] - p.mOrigin;
                    if (0 > Math.V2Cross(cd,ab)){break;}
                    mesh.mVerts.Ps.Add(area.mVerts[k]);
                }

                mesh.mOrigin = Math.CalcCenterCoord(mesh.mVerts);

                area.mMeshs.Add(mesh);
            }
        }

        void FillMesh(Area area, Pile p0, Pile p1)
        {
            var offset = Math.CalcFirstIndex(area.mVerts.Ps,
                                             p0.mOrigin,
                                             p1.mOrigin);
            var lMesh = new Mesh();
            var rMesh = new Mesh();
            var tMesh = new Mesh();
            var bMesh = new Mesh();
            lMesh.mVerts.Ps = new List<Vec2>();
            rMesh.mVerts.Ps = new List<Vec2>();
            tMesh.mVerts.Ps = new List<Vec2>();
            bMesh.mVerts.Ps = new List<Vec2>();
            var p0p1 = p1.mOrigin - p0.mOrigin;
            for (var i = 0; i != area.mVerts.Count; ++i)
            {
                var index = (offset + i) % area.mVerts.Count;
                var point = area.mVerts[index] - p0.mOrigin;
                var cross = Math.V2Cross(p0p1, point);
                if (cross < 0)
                {
                    lMesh.mVerts.Ps.Add(point);
                }
                else
                {
                    rMesh.mVerts.Ps.Add(point);
                }
            }
            lMesh.mVerts.Ps.Add(p0.mOrigin);
            lMesh.mVerts.Ps.Add(p1.mOrigin);
            rMesh.mVerts.Ps.Add(p1.mOrigin);
            rMesh.mVerts.Ps.Add(p0.mOrigin);

            tMesh.mVerts.Ps.Add(p0.mOrigin);
            bMesh.mVerts.Ps.Add(p0.mOrigin);
            tMesh.mVerts.Ps.Add(rMesh.mVerts.Ps[rMesh.mVerts.Ps.Count - 3]);
            bMesh.mVerts.Ps.Add(lMesh.mVerts.Ps[lMesh.mVerts.Ps.Count - 3]);
            tMesh.mVerts.Ps.Add(lMesh.mVerts.Ps[0]);
            bMesh.mVerts.Ps.Add(rMesh.mVerts.Ps[0]);

            lMesh.mOrigin = Math.CalcCenterCoord(lMesh.mVerts);
            rMesh.mOrigin = Math.CalcCenterCoord(rMesh.mVerts);
            tMesh.mOrigin = Math.CalcCenterCoord(tMesh.mVerts);
            bMesh.mOrigin = Math.CalcCenterCoord(bMesh.mVerts);

            area.mMeshs.Add(lMesh);
            area.mMeshs.Add(rMesh);
            area.mMeshs.Add(tMesh);
            area.mMeshs.Add(bMesh);
        }

        void LinkPiles(List<Pile> listNo, List<Pile> listOk, ref Area prev)
        {
            var area = new Area {
                mPrev   = prev,
                mPiles  = listOk,
            };

            //  填充轮廓
            area.mVerts.Ps = new List<Vec2>();
            listOk.ForEach(v => area.mVerts.Ps.Add(v.mOrigin));

            //  填充网格
            area.mMeshs = new List<Mesh>();
            if (prev != null)
            {
                FillMesh(area, prev);
                prev.mNext = area;
            }

            //  填充剩余顶点
            if      (listNo.Count == 1)
            {
                FillMesh(area, listNo[0]);
                listNo.Clear();
            }
            else if (listNo.Count == 2)
            {
                FillMesh(area, listNo[0], listNo[1]);
                listNo.Clear();
            }

            prev = area;
        }

        void InitPiles(List<Pile> piles, List<Pile> list)
        {
            //  极坐标原点
            var o = 0;
            for (var i = 1; i != piles.Count; ++i)
            {
                if (piles[i].mOrigin.y < piles[o].mOrigin.y) { o = i; }
            }

            //  极坐标排序
            var point = piles[o];
            piles.Sort((a, b) => {
                var ap = a.mOrigin - point.mOrigin;
                var bp = b.mOrigin - point.mOrigin;
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

            list = new List<Pile>() { piles[0], piles[1] };
            for (var i = 1; i != piles.Count; ++i)
            {
                var p = piles[i];
                while (piles.Count > 1)
                {
                    var a = piles[piles.Count - 2];
                    var b = piles[piles.Count - 1];
                    if (0 <= Math.V2Cross(b.mOrigin - a.mOrigin,
                                          p.mOrigin - b.mOrigin)) { break; }
                    list.RemoveAt(list.Count - 1);
                }
                list.Add(p);
            }

            list.ForEach(v => piles.Remove(v));
        }

        void Clear(Area area, List<Pile> piles)
        {
            if (area != null)
            {
                piles.AddRange(area.mPiles);
                //area.mMeshs.Clear();
                //area.mPiles.Clear();
                //area.mVerts.Ps.Clear();
                Clear(area.mNext, piles);
            }
        }

        Area FindArea(Area area, Vec2 point)
        {
            if (area.mNext == null)
            {
                return area;
            }

            if (!Math.IsContainsConvex(area.mNext.mVerts, point))
            {
                return area;
            }

            return FindArea(area.mNext, point);
        }

        Area mRoot;
    }

    //  路径搜索
    public class PathQuery {
    }
}