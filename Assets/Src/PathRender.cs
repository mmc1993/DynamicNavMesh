using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    public class PathRender : MonoBehaviour {
        public GameObject mCube;
        public Transform mPathA;
        public Transform mPathB;

        void Start()
        {
            mPathCore = new PathCore();

            List<Vector2> poly = new List<Vector2>{
                new Vector2(-5, -5),
                new Vector2( 5, -5),
                new Vector2( 5,  5),
                new Vector2(-5,  5),
            };
            mPathCore.Init(poly);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMoved();
            }
            else if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl))
            {
                OnInsert();
            }
            else if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftShift))
            {
                OnRemove();
            }

            if (Input.GetMouseButtonUp(0))
            {
                mDragTarget = null;
            }

            if (mFCoord != mPathA.position || mTCoord != mPathB.position)
            {
                var list = new List<Vector2>();
                mPath.Clear();
                mFCoord.x = mPathA.position.x;
                mFCoord.y = 0.1f;
                mFCoord.z = mPathA.position.z;

                mTCoord.x = mPathB.position.x;
                mTCoord.y = 0.1f;
                mTCoord.z = mPathB.position.z;

                Vector2 a; a.x = mFCoord.x; a.y = mFCoord.z;
                Vector2 b; b.x = mTCoord.x; b.y = mTCoord.z;
                mPathCore.GetFindResultPoint(a, b, 1, list);
                list.ForEach(v => {
                    Vector3 p;
                    p.x = v.x;
                    p.z = v.y;
                    p.y = 0.1f;
                    mPath.Add(p);
                });
            }

            if (mDragTarget != null)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var result))
                {
                    if (result.transform == transform)
                    {
                        mDragTarget.position = result.point;
                        mPathCore.Remove(mCubeMap[mDragTarget]);

                        Vector2 point;
                        point.x = result.point.x;
                        point.y = result.point.z;
                        mCubeMap.Remove(mDragTarget);
                        var pile = mPathCore.Insert(point, 1);
                        mCubeMap.Add(mDragTarget.transform, pile);
                    }
                }
            }
        }

        void OnInsert()
        {
            //  添加
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var result))
            {
                if (result.transform == transform)
                {
                    var cube = Tools.Instantiate(mCube);
                    cube.transform.position = result.point;

                    Vector2 point;
                    point.x = result.point.x;
                    point.y = result.point.z;
                    var pile = mPathCore.Insert(point, 1);
                    mCubeMap.Add(cube.transform, pile);
                }
            }
        }

        void OnRemove()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var result))
            {
                if (mCubeMap.TryGetValue(result.transform, out var pile))
                {
                    Destroy(result.transform.gameObject);
                    mPathCore.Remove(pile);
                }
            }
        }

        void OnMoved()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var result))
            {
                if (mCubeMap.ContainsKey(result.transform))
                {
                    mDragTarget = result.transform;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                foreach (var mesh in mPathCore.GetMeshs())
                {
                    DrawMesh(mesh, 0.3f);
                }

                foreach (var edge in mPathCore.GetFindResultEdges())
                {
                    DrawEdge(edge, 1.0f);
                }

                if (mPath.Count != 0)
                {
                    Gizmos.color = Color.blue;

                    Gizmos.DrawLine(mFCoord, mPath[0]);

                    for (var i = 0; i != mPath.Count - 1; ++i)
                    {
                        var a = mPath[i    ];
                        var b = mPath[i + 1];
                        Gizmos.DrawLine(a, b);
                    }
                }
            }
        }

        void DrawMesh(PathCore.Mesh mesh, float colorAlpha = 1)
        {
            for (var i = 0; i != mesh.mPiles.Count; ++i)
            {
                var a = mesh.mPiles[Math.Index(i,     mesh.mPiles.Count)];
                var b = mesh.mPiles[Math.Index(i + 1, mesh.mPiles.Count)];
                Vector3 p0;
                p0.x = a.mOrigin.x;
                p0.y = 0.1f;
                p0.z = a.mOrigin.y;
                Vector3 p1;
                p1.x = b.mOrigin.x;
                p1.y = 0.1f;
                p1.z = b.mOrigin.y;
                Gizmos.color = Color.black * colorAlpha;
                Gizmos.DrawLine(p0, p1);

                Vector3 o;
                o.x = mesh.mPiles[i].mOrigin.x;
                o.z = mesh.mPiles[i].mOrigin.y;
                o.y = 0.1f;
                Gizmos.color = Color.red * colorAlpha;
                Gizmos.DrawSphere(o, mesh.mPiles[i].mRadius);
            }

            Vector3 p;
            p.x = mesh.mOrigin.x;
            p.z = mesh.mOrigin.y;
            p.y = 0.1f;
            Gizmos.color = Color.white * colorAlpha;
            Gizmos.DrawSphere(p, 0.1f);
        }

        void DrawEdge(PathCore.Edge edge, float colorAlpha = 1)
        {
            Vector3 a;
            Vector3 b;
            a.x = edge.mA.mOrigin.x;
            a.z = edge.mA.mOrigin.y;
            a.y = 0.1f;

            b.x = edge.mB.mOrigin.x;
            b.z = edge.mB.mOrigin.y;
            b.y = 0.1f;

            Gizmos.color = Color.red * colorAlpha;
            Gizmos.DrawLine(a, b);
        }

        Dictionary<Transform, PathCore.Pile> mCubeMap = new Dictionary<Transform, PathCore.Pile>();
        Transform mDragTarget;
        PathCore  mPathCore;
        List<Vector3> mPath = new List<Vector3>();
        Vector3 mFCoord;
        Vector3 mTCoord;
    }
}