using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    using Vec2 = Vector2;

    public class PathCore {
        public class Pile {
            public Vec2  mOrigin;   //  原点
            public float mRadius;   //  半径

            public static implicit operator Vec2(Pile val) => val.mOrigin;
        }

        public class Edge {
            public Pile mA;                 //  起点
            public Pile mB;                 //  终点
            public Mesh mSelf;              //  自己区域
            public Edge mLink;

            public Vec2 Origin {
                get {
                    var n = (mB.mOrigin - mA.mOrigin).normalized;
                    var a = mA.mOrigin + n * mA.mRadius;
                    var b = mB.mOrigin - n * mB.mRadius;
                    return Vec2.Lerp(a, b, 0.5f);
                } 
            }

            public bool IsEqual(Edge edge)
            {
                Math.Edge e0, e1;
                e0.P0 = mA.mOrigin;
                e0.P1 = mB.mOrigin;
                e1.P0 = edge.mA.mOrigin;
                e1.P1 = edge.mB.mOrigin;
                return e0.Equals(e1);
            }
        }

        public class Mesh {
            public Vec2 mOrigin;        //  中点
            public List<Pile> mPiles;   //  点集
            public List<Edge> mEdges;   //  边集
        }

        //  路径生成
        public class PathBuild {
            public List<Mesh> GetMeshs()
            {
                return mMeshs;
            }

            public void Init(List<Vec2> points)
            {
                mMeshs.Clear();

                var mesh = new Mesh {
                    mPiles = new List<Pile>(),
                    mEdges = new List<Edge>(),
                };
                mMeshs.Add(mesh);

                foreach (var p in points)
                {
                    var pile = new Pile { mOrigin = p, mRadius = 1, };

                    if (mesh.mEdges.Count != 0)
                    {
                        mesh.mEdges[mesh.mEdges.Count - 1].mB  = pile;
                    }

                    mesh.mEdges.Add(new Edge { mA = pile });

                    mesh.mPiles.Add(pile);
                }

                mesh.mEdges[mesh.mEdges.Count - 1].mB = mesh.mPiles[0];
            }

            public Pile Insert(Vec2 p, float r)
            {
                var meshs = FindMeshs(p);

                mEdges.Clear();
                for (var i = 0; i != meshs.Count; ++i)
                {
                    var mesh = meshs[i];
                    mMeshs.Remove(mesh);
                    var edges = mesh.mEdges;
                    for (var j = 0; j != edges.Count; ++j)
                    {
                        if (edges[j].mLink == null)
                        {
                            continue;
                        }
                        edges[j].mLink.mLink =null;
                        mEdges.Add(edges[j].mLink);
                    }
                }

                var pile = new Pile {
                    mOrigin = p,
                    mRadius = r,
                };
                return Insert(meshs, pile);
            }

            public void Remove(Pile pile)
            {
                var meshs = FindMeshs(pile);

                mLinks.Clear();
                mEdges.Clear();
                for (var i = 0; i != meshs.Count; ++i)
                {
                    var mesh = meshs[i];
                    mMeshs.Remove(mesh);
                    var edges = mesh.mEdges;
                    for (var j = 0; j != mesh.mEdges.Count; ++j)
                    {
                        if (edges[j].mA == pile || edges[j].mB == pile)
                        {
                            continue;
                        }
                        if (edges[j].mLink != null)
                        {
                            edges[j].mLink.mLink =null;
                            mEdges.Add(edges[j].mLink);
                        }
                        mLinks.AddLast(edges[j]);
                    }
                }

                var merge = new List<Edge>();
                MergeEdges(merge);
                StripEdges(merge);
            }

            void MergeEdges(List<Edge> merge)
            {
                merge.Add(mLinks.First.Value);

                mLinks.RemoveFirst();

                while (mLinks.Count != 0)
                {
                    for (var it = mLinks.First; it != null; it = it.Next)
                    {
                        if (it.Value.mA == merge[merge.Count - 1].mB)
                        {
                            merge.Add(it.Value); mLinks.Remove(it); break;
                        }
                    }
                }
            }

            void StripEdges(List<Edge> merge)
            {
                var edge = new Edge();
                var next = false;
                for (var i = 0; i != merge.Count;)
                {
                    var front = merge[i];
                    var links = new List<Edge>{ merge[i] };
                    for (var j = 1; j != merge.Count; ++j)
                    {
                        var e0 = links[Math.Index(   -1, links.Count)];
                        var e1 = merge[Math.Index(i + j, merge.Count)];
                        var ab = e0.mB.mOrigin - e0.mA.mOrigin;
                        var cd = e1.mB.mOrigin - e1.mA.mOrigin;
                        next = Math.V2Cross(ab, cd) >= 0;
                        if (next)
                        {
                            var v0 = front.mA.mOrigin - e1.mB.mOrigin;
                            var v1 = front.mB.mOrigin - front.mA.mOrigin;
                            next = Math.V2Cross(v0, v1) >= 0;
                        }
                        if (next)
                        {
                            links.Add(e1); edge.mA = e1.mB; edge.mB = front.mA; links.Add(edge);
                            next = merge.Find(m => {
                                return links.Find(l => m.mA == l.mA || m.mA == l.mB) == null
                                    && Math.IsContainsConvex(links, m.mA, e => e.mA.mOrigin);
                            }) == null;
                            if (next)
                            {
                                links.RemoveAt(links.Count - 1);
                            }
                            else
                            {
                                links.RemoveAt(links.Count - 1);
                                links.RemoveAt(links.Count - 1);
                            }
                        }

                        if (next) { continue; }

                        if (links.Count == 1)
                        {
                            Debug.Assert(j == 1);links.Clear();
                            i = Math.Index(i + j, merge.Count);
                        }
                        else
                        {
                            links.Add(new Edge { mA = e1.mA, mB = front.mA, });

                            merge.Insert(Math.Index(i + j, merge.Count), 
                                new Edge { mA = front.mA, mB = e1.mA });

                            var mesh = new Mesh {
                                mEdges = links, mPiles = new List<Pile>()
                            };

                            foreach (var e in links)
                            {
                                mesh.mPiles.Add(e.mA); merge.Remove(e);
                            }

                            LinkMesh(mesh); i = Math.Index(i, merge.Count);
                        }
                        break;
                    }

                    if (next)
                    {
                        var mesh = new Mesh {
                            mEdges = links, mPiles = new List<Pile>(),
                        };

                        links.ForEach(e => mesh.mPiles.Add(e.mA));

                        LinkMesh(mesh); break;
                    }
                }
            }

            Pile Insert(List<Mesh> meshs, Pile pile)
            {
                if (meshs.Count == 1)
                {
                    AppendPile(meshs[0], pile);
                }
                else if (meshs.Count > 1)
                {
                    meshs.ForEach(v => InsertPile(v, pile));
                }
                return pile;
            }

            void InsertPile(Mesh mesh, Pile p)
            {
                Math.Edge edge;
                for (var i = 0; i != mesh.mEdges.Count; ++i)
                {
                    edge.P0 = mesh.mEdges[i].mA.mOrigin;
                    edge.P1 = mesh.mEdges[i].mB.mOrigin;
                    if (Math.IsOnEdge(edge, p.mOrigin))
                    {
                        mesh.mPiles.Insert(i + 1, p);
                        mesh.mEdges.Insert(i + 1, new Edge { mA = mesh.mEdges[i].mA, mB = p });
                        mesh.mEdges.Insert(i + 2, new Edge { mB = mesh.mEdges[i].mB, mA = p });
                        mesh.mEdges.RemoveAt(i);
                        break;
                    }
                }
                Debug.Assert(mesh.mEdges.Count > 2);

                LinkMesh(mesh);
            }

            void AppendPile(Mesh mesh, Pile p)
            {
                for (var i = 0; i != mesh.mPiles.Count;)
                {
                    var val = new Mesh {
                        mPiles = new List<Pile>(),
                        mEdges = new List<Edge>(),
                    };

                    //  第一条边
                    val.mEdges.Add(new Edge {
                        mA = p, mB = mesh.mPiles[i]
                    });
                    val.mPiles.Add(val.mEdges[0].mA);
                    val.mPiles.Add(val.mEdges[0].mB);

                    //  中间边
                    var ab = val.mPiles[1].mOrigin
                           - val.mPiles[0].mOrigin;
                    for (; i != mesh.mPiles.Count; ++i)
                    {
                        var k = (i + 1) % mesh.mPiles.Count;
                        var cd = p - mesh.mPiles[k].mOrigin;
                        if (0 > Math.V2Cross(cd, ab)) break;
                    
                        val.mPiles.Add(mesh.mPiles[k]);
                        val.mEdges.Add(new Edge {
                            mA = val.mPiles[val.mPiles.Count - 2],
                            mB = val.mPiles[val.mPiles.Count - 1],
                        });
                    }

                    //  最后一条边
                    val.mEdges.Add(new Edge {
                        mA = val.mPiles[Math.Index(-1, val.mPiles.Count)],
                        mB = val.mPiles[Math.Index( 0, val.mPiles.Count)],
                    });

                    //  连接边
                    LinkMesh(val);
                }
            }

            void LinkMesh(Mesh mesh)
            {
                foreach (var edge in mesh.mEdges)
                {
                    var link = mEdges.Find(v => edge.IsEqual(v));
                    if (link != null && link != edge)
                    {
                        edge.mLink = link;
                        link.mLink = edge;
                    }
                    else
                    {
                        edge.mLink = null;
                    }
                    edge.mSelf= mesh;
                    mEdges.Add(edge);
                }

                mesh.mOrigin = Math.CalcCenterCoord(mesh.mPiles, v => v.mOrigin);

                Debug.Assert(mesh.mPiles.Count > 2);
                mMeshs.Add(mesh);
            }

            List<Mesh> FindMeshs(Pile pile)
            {
                return mMeshs.FindAll(mesh => mesh.mPiles.Contains(pile));
            }

            List<Mesh> FindMeshs(Vec2 point)
            {
                return mMeshs.FindAll(mesh => Math.IsContainsConvex(mesh.mPiles, point, v => v.mOrigin));
            }

            private readonly LinkedList<Edge> mLinks = new LinkedList<Edge>();
            private readonly List<Edge> mEdges = new List<Edge>();
            private readonly List<Mesh> mMeshs = new List<Mesh>();
        }

        //  路径搜索
        public class PathQuery {
            class WayPoint : System.IComparable {
                public WayPoint mParent;
                public Vec2 mOrigin;
                public Edge mEdge;
                public float F;
                public float T;
                public float S { get => F * 0.5f + T; }

                public int CompareTo(object obj)
                {
                    var v = obj as WayPoint;
                    return S  < v.S ? -1
                         : S == v.S ?  0 : 1;
                }
            }

            public bool Find(List<Mesh> meshs, Vec2 fCoord, Vec2 tCoord, float radius, List<Vec2> path)
            {
                var fMesh = Find(meshs, fCoord);
                var tMesh = Find(meshs, tCoord);
                if (fMesh == null || tMesh == null)
                {
                    return false;
                }

                mEdges.Clear();
                mFCoord = fCoord;
                mTCoord = tCoord;
                mRadius = radius;
                if (fMesh == tMesh)
                {
                    if (IsCanLink(tMesh, fCoord, tCoord))
                    {
                        path.Add(tCoord);
                    }
                }
                else
                {
                    mClosed      .Clear();
                    mOpened.Get().Clear();
                    foreach (var edge in fMesh.mEdges)
                    {
                        var eCoord  = edge.Origin;
                        if (IsCanGoto(edge) && IsCanLink(fMesh, fCoord, eCoord))
                        {
                            mOpened.Push(new WayPoint {
                                T = CalcT(eCoord),
                                F = CalcF(eCoord, fCoord),
                                mOrigin = eCoord, mEdge = edge,
                            });
                        }
                    }

                    Find(tMesh);
                    GenNav();
                    Result(radius, path);
                }
                return path.Count != 0;
            }

            //  判断Mesh中的两点是否可连通
            bool IsCanLink(Mesh mesh, Vec2 fCoord, Vec2 tCoord)
            {
                Math.Edge e;
                Math.Edge s;
                s.P0 = fCoord;
                s.P1 = tCoord;
                foreach (var pile in mesh.mPiles)
                {
                    foreach (var edge in mesh.mEdges)
                    {
                        if (pile == edge.mA || pile == edge.mB) { continue; }
                        e.P0 = edge.mA.mOrigin;
                        e.P1 = edge.mB.mOrigin;
                        var d = Math.GetDistance(pile.mOrigin, e);

                        var n = d.normalized;
                        e.P0 = pile.mOrigin + d;
                        e.P1 = pile.mOrigin + n * pile.mRadius;
                        if (!Math.IsCrossSeg(s, e)) { continue; }

                        var r = pile.mRadius 
                              * pile.mRadius 
                              + mRadius * mRadius;
                        if (d.sqrMagnitude < r) { return false; }
                    }
                }
                return true;
            }

            //  判断是否可通过Edge
            bool IsCanGoto(Edge edge)
            {
                if (edge.mLink == null) { return false; }
                var d = edge.mA.mOrigin - edge.mB.mOrigin;
                var r = edge.mA.mRadius
                      + edge.mB.mRadius
                      + mRadius;
                return d.sqrMagnitude >= r * r;
            }

            bool Find(Mesh tMesh)
            {
                while (!mOpened.Empty())
                {
                    var top = mOpened.Pop();
                    mClosed.Add(top);

                    if (top.mEdge.mLink.mSelf == tMesh)
                    {
                        if (IsCanLink(tMesh, top.mOrigin, mTCoord)) { return true; }
                    }

                    Link(top);
                }
                return false;
            }

            void Link(WayPoint wp)
            {
                if (wp.mEdge.mLink == null) { return; }

                foreach (var edge in wp.mEdge.mLink.mSelf.mEdges)
                {
                    if (!IsCanGoto(edge) || !IsCanLink(wp.mEdge.mLink.mSelf, wp.mEdge.Origin, edge.Origin) ||
                        mOpened.Get().Find(v => v.mEdge == edge) != null ||
                        mClosed      .Find(v => v.mEdge == edge) != null)
                    {
                        continue;
                    }

                    var tCoord = edge.Origin;
                    mOpened.Push(new WayPoint { F = CalcF(tCoord, wp.mOrigin),
                                                T = CalcT(tCoord),
                                                mOrigin = tCoord,
                                                mParent = wp,
                                                mEdge = edge,
                    });
                }
            }

            void GenNav()
            {
                var wp = mClosed[mClosed.Count - 1];
                while (wp != null)
                {
                    mEdges.Add(wp.mEdge); wp = wp.mParent;
                }
            }

            void Result(float raduls, List<Vec2> path)
            {
                //path.Add(mFCoord);
                //for (var i = mEdges.Count - 1; i != 0; --i)
                //{
                //    var mesh = mEdges[i    ];
                //    var next = mEdges[i - 1];
                //    var edge = mesh.mEdges.Find(e => e.mLink?.mSelf == next);
                //    var fCoord = path[path.Count - 1];
                //    var tCoord = i > 1 ? mEdges[i - 2].mOrigin : mTCoord;

                //    var len = (edge.mB.mOrigin - edge.mA.mOrigin).magnitude;
                //    var e0 = Vec2.Lerp(edge.mA.mOrigin, edge.mB.mOrigin, (raduls + edge.mA.mRadius) / len);
                //    var e1 = Vec2.Lerp(edge.mB.mOrigin, edge.mA.mOrigin, (raduls + edge.mB.mRadius) / len);
                //    var v0 = tCoord - fCoord;
                //    var v1 = e0 - fCoord;
                //    var v2 = e1 - fCoord;
                //    if      (Math.V2Cross(v0, v1) > 0)
                //    {
                //        path.Add(e0);
                //        //  优化路径
                //    }
                //    else if (Math.V2Cross(v0, v2) < 0)
                //    {
                //        path.Add(e1);
                //        //  优化路径
                //    }
                //}
                //path.Add(mTCoord);
            }

            float CalcF(Vec2 tCoord, Vec2 fCoord)
            {
                return (fCoord - tCoord).magnitude;
            }

            float CalcT(Vec2 tCoord)
            {
                return (mTCoord - tCoord).magnitude;
            }

            Mesh Find(List<Mesh> meshs, Vec2 coord)
            {
                return meshs.Find(mesh => Math.IsContainsConvex(mesh.mPiles, coord, p => p.mOrigin));
            }

            public List<Edge> GetFindResultEdges()
            {
                return mEdges;
            }

            Queue<WayPoint> mOpened = new Queue<WayPoint>();
            List<WayPoint>  mClosed = new List<WayPoint>();
            List<Edge>      mEdges  = new List<Edge>();
            Vec2 mTCoord;
            Vec2 mFCoord;
            float mRadius;
        }

        public void Init(List<Vec2> list)
        {
            mBuild.Init(list);
        }

        public Pile Insert(Vec2 p, float r)
        {
            return mBuild.Insert(p, r);
        }

        public void Remove(Pile pile)
        {
            mBuild.Remove(pile);
        }

        public List<Mesh> GetMeshs()
        {
            return mBuild.GetMeshs();
        }

        public List<Edge> GetFindResultEdges()
        {
            return mQuery.GetFindResultEdges();
        }

        public bool GetFindResultPoint(Vec2 fCoord, Vec2 tCoord, float radius, List<Vec2> path)
        {
            return mQuery.Find(mBuild.GetMeshs(), fCoord, tCoord, radius, path);
        }

        PathBuild mBuild = new PathBuild();
        PathQuery mQuery = new PathQuery();
    }
}