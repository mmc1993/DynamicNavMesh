using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System.Net;
using System.IO;
using UnityEngine.EventSystems;

namespace mmc {
    public class Tools {
        //  ---------------------------------------------------------------------------------------------------
        //  通用
        //  ---------------------------------------------------------------------------------------------------
        public static void Swap<T>(ref T v0, ref T v1)
        {
            var t = v0;
            v0 = v1;
            v1 = t;
        }

        public static void Swap(IList list, int i0, int i1)
        {
            var t = list[i0];
            list[i0] = list[i1];
            list[i1] = t;
        }

        //  ---------------------------------------------------------------------------------------------------
        //  组件相关
        //  ---------------------------------------------------------------------------------------------------
        //  查找Component
        public static T FindComp<T>(Transform target, string path = null) where T : Object
        {
            var result = path != null
                ? target.transform.Find(path) : target;
            Debug.Assert(result != null, path);
            return result.GetComponent<T>();
        }

        public static T FindComp<T>(Transform target, int path) where T : Object
        {
            return FindComp<T>(target, path.ToString());
        }

        //  查找Component
        public static T FindComp<T>(GameObject target, string path = null) where T: Object
        {
            return FindComp<T>(target.transform, path);
        }

        public static T FindComp<T>(GameObject target, int path) where T : Object
        {
            return FindComp<T>(target, path.ToString());
        }

        //  查找Transform
        public static Transform Find(Transform target, string path = null)
        {
            var result = path != null
                ? target.transform.Find(path) : target;
            Debug.Assert(result != null, path);
            return result;
        }

        public static Transform Find(Transform target, int path)
        {
            return Find(target, path.ToString());
        }

        //  查找GameObject
        public static GameObject Find(GameObject target, string path = null)
        {
            return Find(target.transform, path).gameObject;
        }

        public static GameObject Find(GameObject target, int path)
        {
            return Find(target, path.ToString());
        }


        public static GameObject Instantiate(GameObject target)
        {
            var result = Object.Instantiate(target, target.transform.parent);
            result.SetActive(true);
            return result;
        }

        public static GameObject Instantiate(GameObject target, string name)
        {
            var result = Instantiate(target);
            result.name = name;return result;
        }

        public static GameObject Instantiate(GameObject target, int name)
        {
            return Instantiate(target, name.ToString());
        }

        public static GameObject Instantiate(Transform target)
        {
            return Object.Instantiate(target.gameObject, target.parent);
        }

        public static GameObject Instantiate(Transform target, string name)
        {
            var result = Instantiate(target);
            result.SetActive(true);
            result.name = name; return result;
        }

        public static GameObject Instantiate(Transform target, int name)
        {
            return Instantiate(target, name.ToString());
        }

        public static T InstantiateComp<T>(T comp) where T : Component
        {
            var result = Instantiate(comp.gameObject);
            return FindComp<T>(result);
        }
    }
}