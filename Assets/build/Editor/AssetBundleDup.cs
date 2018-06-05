using comunity;
using System.Collections.Generic;
using System;

namespace build
{
    public class AssetBundleDup
    {
        public UnityObjId dup;
        public List<UnityObjId> refs = new List<UnityObjId>();

        public AssetBundleDup(Object o)
        {
            dup = new UnityObjId(o);
        }

        public void AddRef(Object o)
        {
            refs.Add(new UnityObjId(o));
        }
    }
}

