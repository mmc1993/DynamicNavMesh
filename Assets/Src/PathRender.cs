using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    public class PathRender : MonoBehaviour {
        public LineRenderer mLine;
        public GameObject mCube;

        void Start()
        {
            mPathBuild = new PathBuild();

            List<Vector2> poly = new List<Vector2>{
                new Vector2(-5, -5),
                new Vector2( 5, -5),
                new Vector2( 5,  5),
                new Vector2(-5,  5),
            };
            mPathBuild.Init(poly);
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
                        var pile = mPathBuild.Insert(point, 2);
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
                        mPathBuild.Remove(pile);
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
            
            foreach (var mesh in mPathBuild.GetMeshs())
            {
                DrawPiles(mesh.mPiles);
                DrawEdges(mesh.mEdges);
            }
        }

        void DrawPiles(List<PathBuild.Pile> list)
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

        void DrawEdges(List<PathBuild.Edge> list)
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

        Dictionary<Transform, PathBuild.Pile> mCubeMap = new Dictionary<Transform, PathBuild.Pile>();
        PathBuild mPathBuild;
    }
}