using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    public class PathRender : MonoBehaviour {
        public LineRenderer mLine;
        public GameObject mCube;

        void Start()
        {
            var values = new double[] {
//12, 0.7162937, 2.842388, 5, 5,
//5, 5, -5, 5,
//-5, 5, 0.7162937, 2.842388,
//12, 0.7162937, 2.842388, -5, 5,
//-5, 5, -5, -5,
//-5, -5, 0.7162937, 2.842388,
//12, 4.669822, 3.21595, 4.10948, 0.2897173,
//4.10948, 0.2897173, 5, -5,
//5, -5, 4.669822, 3.21595,
//12, 4.669822, 3.21595, 5, -5,
//5, -5, 5, 5,
//5, 5, 4.669822, 3.21595,
//12, 4.669822, 3.21595, 5, 5,
//5, 5, 4.10948, 0.2897173,
//4.10948, 0.2897173, 4.669822, 3.21595,
//16, 0.7162937, 2.842388, -5, -5,
//-5, -5, 4.10948, 0.2897173,
//4.10948, 0.2897173, 5, 5,
//5, 5, 0.7162937, 2.842388,
//12, 3.673656, -2.262955, -5, -5,
//-5, -5, 5, -5,
//5, -5, 3.673656, -2.262955,
//12, 3.673656, -2.262955, 5, -5,
//5, -5, 4.10948, 0.2897173,
//4.10948, 0.2897173, 3.673656, -2.262955,
//12, 3.673656, -2.262955, 4.10948, 0.2897173,
//4.10948, 0.2897173, -5, -5,
//-5, -5, 3.673656, -2.262955,
            };

            Vector3 a;
            Vector3 b;
            a.y = 0.1f;
            b.y = 0.1f;
            for (var i = 0; i != values.Length;)
            {
                var temp = Instantiate(mLine.gameObject,
                                mLine.transform.parent);
                var line = temp.GetComponent<LineRenderer>();
                temp.SetActive(true);

                var num = (int)values[i];
                line.positionCount = num / 2;++i;
                for (var j = 0; j != num; j += 2)
                {
                    a.x = (float)values[i + j    ];
                    a.z = (float)values[i + j + 1];
                    line.SetPosition(j / 2, a);
                }
                i += num;
            }


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
            if (Input.GetMouseButtonUp(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var result))
                {
                    if (result.transform == transform)
                    {
                        var cube = Instantiate(mCube, mCube.transform.parent);
                        cube.transform.position = result.point;
                        cube.SetActive(true);

                        Vector2 point;
                        point.x = result.point.x;
                        point.y = result.point.z;
                        var pile = mPathCore.Insert(point, 2);
                        mCubeMap.Add(cube.transform, pile);
                        OnRefresh();
                    }
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var result))
                {
                    if (mCubeMap.TryGetValue(result.transform, out var pile))
                    {
                        Destroy(result.transform.gameObject);
                        mPathCore.Remove(pile);
                        OnRefresh();
                    }
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var result))
                {
                    if (mCubeMap.TryGetValue(result.transform, out var pile))
                    {
                        mDragTarget = result.transform;
                    }
                }
            }

            if (Input.GetMouseButtonUp(2))
            {
                mDragTarget = null;
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
                        var pile = mPathCore.Insert(point, 0.1f);
                        mCubeMap.Add(mDragTarget.transform, pile);
                        OnRefresh();
                    }
                }
            }
        }

        public void OnRefresh()
        {
            //mPathBuild.Insert(new Vector2(0, 0), 1);

            //mPathBuild.Insert(new Vector2(2, 2), 1);

            //mPathBuild.Insert(new Vector2(3, 2), 1);

            //mPathBuild.Insert(new Vector2(3, -2), 1);

            OnRender();
        }

        void OnRender()
        {
            for (var i = 0; i != mLine.transform.parent.childCount; ++i)
            {
                var child = mLine.transform.parent.GetChild(i);
                if (child.name != "Line" && child.name.Contains("Line"))
                {
                    Destroy(child.gameObject);
                }
            }
            
            foreach (var mesh in mPathCore.GetMeshs())
            {
                DrawPiles(mesh.mPiles);
                DrawEdges(mesh.mEdges);
            }
        }

        void DrawPiles(List<PathCore.Pile> list)
        {
            var temp = Instantiate(mLine.gameObject,
                            mLine.transform.parent);
            var line = temp.GetComponent<LineRenderer>();
            line.positionCount = list.Count;
            for (var i = 0; i != list.Count; ++i)
            {
                Vector3 p;
                p.x = list[i].mOrigin.x;
                p.y = 0.1f;
                p.z = list[i].mOrigin.y;
                line.SetPosition(i, p);
            }
            temp.SetActive(true);
        }

        void DrawEdges(List<PathCore.Edge> list)
        {
            for (var i = 0; i != list.Count; ++i)
            {
                if (list[i].mLink != null)
                {
                    var temp = Instantiate(mLine.gameObject,
                                    mLine.transform.parent);
                    var line = temp.GetComponent<LineRenderer>();
                    line.positionCount = 0;
                    line.startColor = Color.red;
                    line.endColor   = Color.red;
                    line.loop = false;
                    temp.SetActive(true);

                    Vector3 p;

                    ++line.positionCount;
                    p.x = list[i].mSelf.mOrigin.x;
                    p.y = 0.1f;
                    p.z = list[i].mSelf.mOrigin.y;
                    line.SetPosition(line.positionCount - 1, p);

                    ++line.positionCount;
                    p = Vector2.Lerp(list[i].mA.mOrigin, 
                                     list[i].mB.mOrigin, 0.5f);
                    p.z = p.y; p.y = 0.1f;
                    line.SetPosition(line.positionCount - 1, p);

                    ++line.positionCount;
                    p.x = list[i].mLink.mSelf.mOrigin.x;
                    p.y = 0.1f;
                    p.z = list[i].mLink.mSelf.mOrigin.y;
                    line.SetPosition(line.positionCount - 1, p);
                }
            }
        }

        Dictionary<Transform, PathCore.Pile> mCubeMap = new Dictionary<Transform, PathCore.Pile>();
        Transform mDragTarget;
        PathCore  mPathCore;
    }
}