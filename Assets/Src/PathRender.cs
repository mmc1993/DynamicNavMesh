using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    public class PathRender : MonoBehaviour {
        public LineRenderer mLine;

        void Start()
        {
            mPathBuild = new PathBuild();
        }

        void Update()
        {

        }

        public void OnRefresh()
        {
            List<Vector2> poly = new List<Vector2>{
                new Vector2(-5, -5),
                new Vector2( 5, -5),
                new Vector2( 5,  5),
                new Vector2(-5,  5),
            };
            mPathBuild.Init(poly);

            mPathBuild.Insert(new Vector2(0, 0), 1);

            mPathBuild.Insert(new Vector2(2, 2), 1);

            OnRender();
        }

        void OnRender()
        {
            for (var i = 0; i != mLine.transform.parent.childCount; ++i)
            {
                var child = mLine.transform.parent.GetChild(i);
                if (child.name != "Line")
                {
                    Destroy(child.gameObject);
                }
            }
            
            foreach (var mesh in mPathBuild.GetMeshs())
            {
                DrawPoints(mesh.mPiles);
            }
        }

        void DrawPoints(List<PathBuild.Pile> list)
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

        PathBuild mPathBuild;
    }
}