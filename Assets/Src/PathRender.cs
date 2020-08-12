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
12, 2.501167, 1.748363, 5, -5,
5, -5, 5, 5,
5, 5, 2.501167, 1.748363,
12, -1.47606, -1.327774, -5, -5,
-5, -5, 2.501167, 1.748363,
2.501167, 1.748363, -1.47606, -1.327774,
12, 2.035085, -4.279621, -5, -5,
-5, -5, 5, -5,
5, -5, 2.035085, -4.279621,
12, 2.936176, -1.265629, 2.035085, -4.279621,
2.035085, -4.279621, 5, -5,
5, -5, 2.936176, -1.265629,
12, 2.936176, -1.265629, 5, -5,
5, -5, 2.501167, 1.748363,
2.501167, 1.748363, 2.936176, -1.265629,
12, 2.936176, -1.265629, 2.501167, 1.748363,
2.501167, 1.748363, 2.035085, -4.279621,
2.035085, -4.279621, 2.936176, -1.265629,
16, -0.3263932, 1.593002, -1.47606, -1.327774,
-1.47606, -1.327774, 2.501167, 1.748363,
2.501167, 1.748363, 5, 5,
5, 5, -0.3263932, 1.593002,
12, -0.3263932, 1.593002, 5, 5,
5, 5, -5, 5,
-5, 5, -0.3263932, 1.593002,
12, -2.346078, 0.7851278, -0.3263932, 1.593002,
-0.3263932, 1.593002, -5, 5,
-5, 5, -2.346078, 0.7851278,
12, -2.346078, 0.7851278, -5, 5,
-5, 5, -1.47606, -1.327774,
-1.47606, -1.327774, -2.346078, 0.7851278,
12, -2.346078, 0.7851278, -1.47606, -1.327774,
-1.47606, -1.327774, -0.3263932, 1.593002,
-0.3263932, 1.593002, -2.346078, 0.7851278,
12, -2.936448, -1.265629, -1.47606, -1.327774,
-1.47606, -1.327774, -5, 5,
-5, 5, -2.936448, -1.265629,
12, -2.936448, -1.265629, -5, 5,
-5, 5, -5, -5,
-5, -5, -2.936448, -1.265629,
12, -2.936448, -1.265629, -5, -5,
-5, -5, -1.47606, -1.327774,
-1.47606, -1.327774, -2.936448, -1.265629,
12, -0.1399606, -3.533892, 2.035085, -4.279621,
2.035085, -4.279621, 2.501167, 1.748363,
2.501167, 1.748363, -0.1399606, -3.533892,
12, -0.1399606, -3.533892, 2.501167, 1.748363,
2.501167, 1.748363, -5, -5,
-5, -5, -0.1399606, -3.533892,
12, -0.1399606, -3.533892, -5, -5,
-5, -5, 2.035085, -4.279621,
2.035085, -4.279621, -0.1399606, -3.533892,

            };

            //Vector3 a;
            //Vector3 b;
            //a.y = 0.1f;
            //b.y = 0.1f;
            //for (var i = 0; i != values.Length;)
            //{
            //    var temp = Instantiate(mLine.gameObject,
            //                    mLine.transform.parent);
            //    var line = temp.GetComponent<LineRenderer>();
            //    temp.SetActive(true);

            //    var num = (int)values[i];
            //    line.positionCount = num / 2; ++i;
            //    for (var j = 0; j != num; j += 2)
            //    {
            //        a.x = (float)values[i + j] * 100;
            //        a.z = (float)values[i + j + 1] * 100;
            //        line.SetPosition(j / 2, a);
            //    }
            //    i += num;
            //}


            mPathCore = new PathCore();

            List<Vector2> poly = new List<Vector2>{
                new Vector2(-5, -5),
                new Vector2( 5, -5),
                new Vector2( 5,  5),
                new Vector2(-5,  5),
            };
            mPathCore.Init(poly);

            //var pile0 = mPathCore.Insert(new Vector2(-1.413916f, -0.2091784f), 1);
            //var pile1 = mPathCore.Insert(new Vector2(0.3882648f, -0.8927647f), 1);
            //var pile2 = mPathCore.Insert(new Vector2(3.43333f, -1.607423f), 1);
            //var pile3 = mPathCore.Insert(new Vector2(3.433331f, -0.08489053f), 1);
            //var pile4 = mPathCore.Insert(new Vector2(1.879725f, 2.680524f), 1);
            //var pile5 = mPathCore.Insert(new Vector2(-1.320699f, 2.214444f), 1);
            //mPathCore.Remove(pile4);
            //var pile6 = mPathCore.Insert(new Vector2(2.345805f, 3.395184f), 1);
            //mPathCore.Remove(pile6);
            //var pile7 = mPathCore.Insert(new Vector2(3.402257f, 3.79912f), 1);
            //mPathCore.Remove(pile5);
            //var pile8 = mPathCore.Insert(new Vector2(-0.8856905f, 1.624075f), 1);
            //mPathCore.Remove(pile8);
            //var pile9 = mPathCore.Insert(new Vector2(0.01540029f, 0.9715594f), 1);
            //mPathCore.Remove(pile9);
            //var pile10 = mPathCore.Insert(new Vector2(1.631148f, 0.2879747f), 1);
            //mPathCore.Remove(pile10);
            //var pile11 = mPathCore.Insert(new Vector2(4.147988f, 0.6297684f), 1);
            //mPathCore.Remove(pile11);
            //var pile12 = mPathCore.Insert(new Vector2(4.986933f, 1.344424f), 1);
            //mPathCore.Remove(pile2);
            //var pile13 = mPathCore.Insert(new Vector2(3.992626f, -2.197792f), 1);


            {
                //var temp = Instantiate(mLine.gameObject, mLine.transform.parent);
                //var line = temp.GetComponent<LineRenderer>();
                //temp.SetActive(true);
                //line.positionCount = 3;
                //line.SetPositions(new Vector3[] {
                //    new Vector3(-0.4506806f, 0.2f, 3.674831f),
                //    new Vector3(-2.346078f, 0.2f, 1.282281f),
                //    new Vector3(5f, 0.2f, 5f),
                //});

                //temp = Instantiate(mLine.gameObject, mLine.transform.parent);
                //line = temp.GetComponent<LineRenderer>();
                //temp.SetActive(true);
                //line.positionCount = 3;
                //line.SetPositions(new Vector3[] {
                //    new Vector3(-0.4506806f, 0.2f, 3.674831f),
                //    new Vector3(-5f, 0.2f, 5f),
                //    new Vector3(-2.346078f, 0.2f, 1.282281f),
                //});

                //temp = Instantiate(mLine.gameObject, mLine.transform.parent);
                //line = temp.GetComponent<LineRenderer>();
                //temp.SetActive(true);
                //line.positionCount = 3;
                //line.SetPositions(new Vector3[] {
                //    new Vector3(-2.346078f, 0.2f, 1.282281f),
                //    new Vector3(3.526545f, 0.2f, -3.036738f),
                //    new Vector3(3.122607f, 0.2f, 0.3811913f),
                //});

                //temp = Instantiate(mLine.gameObject, mLine.transform.parent);
                //line = temp.GetComponent<LineRenderer>();
                //temp.SetActive(true);
                //line.positionCount = 3;
                //line.SetPositions(new Vector3[] {
                //    new Vector3(-2.346078f, 0.2f, 1.282281f),
                //    new Vector3(3.122607f, 0.2f, 0.3811913f),
                //    new Vector3(5f, 0.2f, 5f),
                //});

                //temp = Instantiate(mLine.gameObject, mLine.transform.parent);
                //line = temp.GetComponent<LineRenderer>();
                //temp.SetActive(true);
                //line.positionCount = 3;
                //line.SetPositions(new Vector3[] {
                //    new Vector3(-2.346078f, 0.2f, 1.282281f),
                //    new Vector3(-5f, 0.2f, 5f),
                //    new Vector3(-5f, 0.2f, -5f),
                //});

                //temp = Instantiate(mLine.gameObject, mLine.transform.parent);
                //line = temp.GetComponent<LineRenderer>();
                //temp.SetActive(true);
                //line.positionCount = 3;
                //line.SetPositions(new Vector3[] {
                //    new Vector3(-0.9167625f, 0.2f, -3.720324f),
                //    new Vector3(-2.346078f, 0.2f, 1.282281f),
                //    new Vector3(-5f, 0.2f, -5f),
                //});

                //temp = Instantiate(mLine.gameObject, mLine.transform.parent);
                //line = temp.GetComponent<LineRenderer>();
                //temp.SetActive(true);
                //line.positionCount = 3;
                //line.SetPositions(new Vector3[] {
                //    new Vector3(-0.9167625f, 0.2f, -3.720324f),
                //    new Vector3(3.526545f, 0.2f, -3.036738f),
                //    new Vector3(-2.346078f, 0.2f, 1.282281f),
                //});
            }

            //OnRefresh();
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
                //DrawEdges(mesh.mEdges);
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