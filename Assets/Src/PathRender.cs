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
            Math.Polygon poly;
            poly.Ps = new List<Vector2>{
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
            
            Queue<PathBuild.Area> queue = 
                new Queue<PathBuild.Area>();
            queue.Enqueue(mPathBuild.Root());
            while (queue.Count != 0)
            {
                var front = queue.Dequeue();
                DrawPoints(front.mVerts.Ps);
                foreach (var mesh in front.mMeshs)
                {
                    DrawPoints(mesh.mVerts.Ps);
                }
            }
        }

        void DrawPoints(List<Vector2> list)
        {
            var temp = Instantiate(mLine.gameObject,
                            mLine.transform.parent);
            var line = temp.GetComponent<LineRenderer>();
            line.positionCount = list.Count;
            for (var i = 0; i != list.Count; ++i)
            {
                Vector3 p;
                p.x = list[i].x;
                p.y = 0.1f;
                p.z = list[i].y;
                line.SetPosition(i, p);
            }
            temp.SetActive(true);
        }

        PathBuild mPathBuild;
    }
}