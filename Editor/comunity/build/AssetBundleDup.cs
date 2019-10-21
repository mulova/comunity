using System.Collections.Generic;
using mulova.unicore;
using Object = UnityEngine.Object;

namespace mulova.build
{
    public class AssetBundleDup
    {
        public ObjRef dup;
        public List<ObjRef> refs = new List<ObjRef>();
		public bool duplicate;

        public AssetBundleDup(Object o)
        {
            dup = new ObjRef(o);
        }

        public void AddRef(Object o)
        {
            refs.Add(new ObjRef(o));
        }

		public override bool Equals(object obj)
		{
			if (obj is AssetBundleDup)
			{
				var that = obj as AssetBundleDup;
				return this.dup == that.dup;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return dup.id.GetHashCode();
		}

		public override string ToString()
		{
			return dup.id;
		}
    }
}

