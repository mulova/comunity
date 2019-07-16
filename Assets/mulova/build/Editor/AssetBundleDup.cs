using System.Collections.Generic;
using comunity;
using Object = UnityEngine.Object;

namespace build
{
    public class AssetBundleDup
    {
        public UnityObjId dup;
        public List<UnityObjId> refs = new List<UnityObjId>();
		public bool duplicate;

        public AssetBundleDup(Object o)
        {
            dup = new UnityObjId(o);
        }

        public void AddRef(Object o)
        {
            refs.Add(new UnityObjId(o));
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

