using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Text.Ex;

namespace comunity
{
	public class ObjRefListDrawer : ListDrawer<ObjRef>
	{
        public bool allowSceneObject = true;

        public ObjRefListDrawer(List<ObjRef> list) : base(list, new ObjRefDrawer())
		{
			this.createDefaultValue = () => CreateItem(Selection.activeObject);
			this.createItem = CreateItem;
		}
		
		private ObjRef CreateItem(Object o)
		{
            if (allowSceneObject || !AssetDatabase.GetAssetPath(o).IsEmpty())
            {
                return new ObjRef(o);
            } else
            {
               return new ObjRef(null);
            }
        }
	}
}
