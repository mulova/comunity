using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace comunity
{
    [System.Serializable]
    public class UnityObjList : List<UnityObjId>
    {
        public void Add(Object obj)
        {
            Add(new UnityObjId(obj));
        }

        public void Insert(int index, Object obj)
        {
            Insert(index, new UnityObjId(obj));
        }

        public void Remove(Object obj)
        {
            int index = FindIndex(o => o.reference == obj);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        public void Remove(string guid)
        {
            int index = FindIndex(o => o.id == guid);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }
    }
}
