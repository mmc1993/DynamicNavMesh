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

            public override bool Equals(object obj)
            {
                var other = obj as Edge;
                Math.Edge e0, e1;
                e0.P0 = mA.mOrigin;
                e0.P1 = mB.mOrigin;
                e1.P0 = other.mA.mOrigin;
                e1.P1 = other.mB.mOrigin;
                return e0.Equals(e1);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
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

                    mesh.mEdges.Add(new Edge { mA = pile, mSelf = mesh });

                    mesh.mPiles.Add(pile);
                }

                mesh.mEdges[mesh.mEdges.Count - 1].mB = mesh.mPiles[0];
            }

            public Pile Insert(Vec2 p, float r)
            {
                mInsert = p;
                Debug.LogFormat("插入: {0}, {1}", p.x, p.y);
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
                Insert(meshs, pile);
                Dump();
                return pile;
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

            void MergeEdges(List<Edge> output)
            {
                output.Add(mLinks.First.Value);

                mLinks.RemoveFirst();

                while (mLinks.Count != 0)
                {
                    for (var it = mLinks.First; it != null; it = it.Next)
                    {
                        if (it.Value.mA == output[output.Count - 1].mB)
                        {
                            output.Add(it.Value); mLinks.Remove(it); break;
                        }
                    }
                }
            }

            void StripEdges(List<Edge> merge)
            {
                for (var i = 0; i != merge.Count; ++i)
                {
                    var e0 = merge[Math.Index(i,     merge.Count)];
                    var e1 = merge[Math.Index(i + 1, merge.Count)];
                    var e2 = merge[Math.Index(i + 2, merge.Count)];
                    var a = e1.mB.mOrigin - e1.mA.mOrigin;
                    var b = e2.mB.mOrigin - e2.mA.mOrigin;
                    if (Math.V2Cross(a, b) < 0)
                    {
                        var pos = Math.Index(i, merge.Count);
                        merge.Insert(pos, new Edge {
                            mA = e0.mA,
                            mB = e2.mA,
                        });
                        merge.Remove(e0);
                        merge.Remove(e1);

                        var mesh = new Mesh {
                            mEdges = new List<Edge>(),
                            mPiles = new List<Pile>(),
                        };
                        e0.mSelf = mesh;
                        e1.mSelf = mesh;
                        mesh.mEdges.Add(e0);
                        mesh.mEdges.Add(e1);
                        mesh.mEdges.Add(new Edge {
                            mA = e2.mA,
                            mB = e0.mA,
                            mSelf = mesh,
                        });

                        mesh.mPiles.Add(e0.mA);
                        mesh.mPiles.Add(e1.mA);
                        mesh.mPiles.Add(e2.mA);

                        mesh.mOrigin = Math.CalcCenterCoord(mesh.mPiles, v => v.mOrigin);

                        LinkMesh(mesh); 

                        i -= 2;
                    }
                }

                {
                    var mesh = new Mesh {
                        mEdges = new List<Edge>(),
                        mPiles = new List<Pile>(),
                    };

                    for (var i = 0; i != merge.Count; ++i)
                    {
                        merge[i].mSelf = mesh;
                        mesh.mEdges.Add(merge[i]);
                        mesh.mPiles.Add(merge[i].mA);
                    }

                    mesh.mOrigin = Math.CalcCenterCoord(mesh.mPiles, v => v.mOrigin);

                    LinkMesh(mesh);
                }
            }

            void Insert(List<Mesh> meshs, Pile pile)
            {
                if (meshs.Count == 1)
                {
                    AppendPile(meshs[0], pile);
                }
                else if (meshs.Count > 1)
                {
                    meshs.ForEach(v => InsertPile(v, pile));
                }
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
                        mesh.mEdges.Insert(i + 1, new Edge { mA = mesh.mEdges[i].mA, mB = p, mSelf = mesh, });
                        mesh.mEdges.Insert(i + 2, new Edge { mB = mesh.mEdges[i].mB, mA = p, mSelf = mesh, });
                        mesh.mEdges.RemoveAt(i);
                        break;
                    }
                }

                Debug.AssertFormat(mesh.mEdges.Count > 2, "{0}, {1}", mInsert.x, mInsert.y);

                mesh.mOrigin = Math.CalcCenterCoord(mesh.mPiles, v => v);

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
                        mA = p, mB = mesh.mPiles[i], mSelf = val,
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
                            mSelf = val,
                        });
                    }

                    //  最后一条边
                    val.mEdges.Add(new Edge {
                        mA = val.mPiles[val.mPiles.Count - 1],
                        mB = val.mPiles[0],
                        mSelf = val,
                    });

                    //  计算中点
                    val.mOrigin = Math.CalcCenterCoord(val.mPiles, v => v);

                    //  连接边
                    LinkMesh(val);
                }
            }

            void LinkMesh(Mesh mesh)
            {
                foreach (var edge in mesh.mEdges)
                {
                    var link = mEdges.Find(v => edge.Equals(v));
                    if (link != null)
                    {
                        edge.mLink = link;
                        link.mLink = edge;
                    }
                    else
                    {
                        edge.mLink = null;
                    }
                    mEdges.Add(edge);
                }
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

            void Dump()
            {
                mBuffer += string.Format("Insert {0}, {1}\n", mInsert.x, mInsert.y);
                for (var i = 0; i != mMeshs.Count; ++i)
                {
                    mBuffer += string.Format("Mesh{0}\n", i);
                    for (var j = 0; j != mMeshs[i].mPiles.Count; ++j)
                    {
                        mBuffer += string.Format("Pile{0} {1}, {2}\n",
                            j,
                            mMeshs[i].mPiles[j].mOrigin.x,
                            mMeshs[i].mPiles[j].mOrigin.y);
                    }

                    mBuffer += string.Format("{0}, ", mMeshs[i].mEdges.Count * 4);
                    for (var j = 0; j != mMeshs[i].mEdges.Count; ++j)
                    {
                        mBuffer += string.Format("Edge{0} {1}, {2}, {3}, {4}\n",
                            j,
                            mMeshs[i].mEdges[j].mA.mOrigin.x,
                            mMeshs[i].mEdges[j].mA.mOrigin.y,
                            mMeshs[i].mEdges[j].mB.mOrigin.x,
                            mMeshs[i].mEdges[j].mB.mOrigin.y);
                    }
                }
                mBuffer += "\n\n";
                System.IO.File.WriteAllText("dump.txt", mBuffer);
            }

            Vec2 mInsert;
            string mBuffer = "";
            private readonly LinkedList<Edge> mLinks = new LinkedList<Edge>();  //  用于删除节点时
            private readonly List<Edge> mEdges = new List<Edge>();
            private readonly List<Mesh> mMeshs = new List<Mesh>();
        }

        //  路径搜索
        public class PathQuery {

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

        PathBuild mBuild = new PathBuild();
        PathQuery mQuery = new PathQuery();
    }
}