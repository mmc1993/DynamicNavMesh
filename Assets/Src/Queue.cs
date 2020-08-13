using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mmc {
    public class Queue<T> where T : System.IComparable {

        public void Push(T val)
        {
            mValues.Add(val); Push(mValues.Count - 1, val);
        }

        public T Pop()
        {
            var top = Top();
            var idx = mValues.Count -1;
            Tools.Swap(mValues, 0,idx);
            Sort(0, idx).RemoveAt(idx);
            return top;
        }

        public T Top()
        {
            return mValues[0];
        }

        public bool Empty()
        {
            return mValues.Count == 0;
        }

        public int Length()
        {
            return mValues.Count;
        }

        public List<T> Get()
        {
            return mValues;
        }

        List<T> Sort(int idx, int len)
        {
            if (idx < len / 2)
            {
                var min = idx;
                var l = idx * 2 + 1;
                var r = idx * 2 + 2;
                if (l < len && mValues[l].CompareTo(mValues[min]) == -1)
                {
                    min = l;
                }
                if (r < len && mValues[r].CompareTo(mValues[min]) == -1)
                {
                    min = r;
                }
                if (min != idx)
                {
                    Tools.Swap(mValues, idx, min); Sort(min, len);
                }
            }
            return mValues;
        }

        void Push(int idx, T val)
        {
            while (idx > 0)
            {
                var pre = (idx - 1) / 2;
                if (val.CompareTo(mValues[pre]) != -1)
                {
                    break;
                }
                mValues[idx] = mValues[pre]; idx = pre;
            }
            mValues[idx] = val;
        }

        readonly List<T> mValues = new List<T>();
    }
}