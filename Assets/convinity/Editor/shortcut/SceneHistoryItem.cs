using UnityEngine;
using Object = UnityEngine.Object;
using System;
using comunity;
using System.Collections.Generic;
using commons;

namespace convinity
{
    [Serializable]
    public class SceneHistoryItem
    {
        public List<UnityObjId> list;

        public SceneHistoryItem(Object o)
        {
            list = new List<UnityObjId>();
            list.Add(new UnityObjId(o));
        }

        public UnityObjId first
        {
            get
            {
                return list.Get(0);
            }
        }

        public Object firstRef
        {
            get
            {
                var o = first;
                if (o != null)
                {
                    return o.reference;
                } else
                {
                    return null;
                }
            }
        }
    }
}
