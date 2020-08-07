using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    using Vec2 = Vector2;
    //  路径生成
    public class PathBuild {
        class Pile {
            public Area  mArea;     //  所属区域
            public Vec2  mOrigin;   //  原点
            public float mRadius;   //  半径
        }

        class Area {
            public Math.Polygon mVerts;     //  顶点集
            public List<Pile> mPiles;       //  桩集
            public List<Mesh> mMeshs;       //  网格
            public Area mPrev;  //  上级
            public Area mNext;  //  下级
        }

        class Edge {
            public Vec2 mA;                 //  起点
            public Vec2 mB;                 //  终点
            public Area mLink;              //  链接区域
            public Area mSelf;              //  自己区域
        }

        class Mesh {
            public Vec2         mOrigin;    //  中点
            public Math.Polygon mVerts;     //  点集
            public List<Edge>   mEdges;     //  边集
        }

        public void Init(Math.Polygon points)
        {
            mRoot = new Area {
                mVerts = points,
                mPiles = new List<Pile>(),
                mMeshs = new List<Mesh>(),
            };
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
            Clear(area, piles); piles.Add(pile);

            var prev = area.mPrev;
            var list = new List<Pile>();
            while (piles.Count != 0)
            {
                list.Clear();
                InitPiles(piles, list);
                LinkPiles(piles, list, ref prev);
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
            piles.Sort((a, b) => {
                if (a.mOrigin.y < b.mOrigin.y) { return -1; }
                if (b.mOrigin.y < a.mOrigin.y) { return  1; }
                if (a.mOrigin.x < b.mOrigin.x) { return -1; }
                if (b.mOrigin.x < a.mOrigin.x) { return  1; }
                return 0;
            });

            list = new List<Pile>() {
                piles[0],
                piles[1] 
            };
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
            piles.AddRange(area.mPiles);
            area.mMeshs.Clear();
            area.mPiles.Clear();
            area.mVerts.Ps.Clear();
            if (area.mNext != null)
            {
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