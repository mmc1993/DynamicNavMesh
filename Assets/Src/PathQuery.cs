using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    using Vec2 = Vector2;
    //  路径生成
    public class PathBuild {
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

        public void Insert(Vec2 p, float r)
        {
            var meshs = FindArea(p);

            mEdges.Clear();
            for (var i = 0; i != meshs.Count; ++i)
            {
                var mesh = meshs[i];
                mMeshs.Remove(mesh);
                mEdges.AddRange(mesh.mEdges);
            }

            Insert(meshs, new Pile { mOrigin = p, mRadius = r, });
        }

        void Insert(List<Mesh> meshs, Pile pile)
        {
            if      (meshs.Count == 1)
            {
                Insert(meshs[0], pile);
            }
            else if (meshs.Count > 1)
            {
                meshs.ForEach(v => Insert(v, pile));
            }
        }

        void Insert(Mesh mesh, Pile p)
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
                var ab = val.mPiles[1].mOrigin - val.mPiles[0].mOrigin;
                for (var j = i; j != mesh.mPiles.Count; ++j, ++i)
                {
                    var k = (j + 1) % mesh.mPiles.Count;
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
                mEdges.Add(edge);
            }
            mMeshs.Add(mesh);
        }

        List<Mesh> FindArea(Vec2 point)
        {
            return mMeshs.FindAll(mesh => Math.IsContainsConvex(mesh.mPiles, point, v => v.mOrigin));
        }

        private readonly List<Edge> mEdges = new List<Edge>();
        private readonly List<Mesh> mMeshs = new List<Mesh>();
    }

    //  路径搜索
    public class PathQuery {
    }
}