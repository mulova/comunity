using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Text.Ex;

namespace comunity
{
	public class UnityObjListDrawer : ListDrawer<UnityObjId>
	{
        public bool allowSceneObject = true;

        public UnityObjListDrawer(List<UnityObjId> list) : base(list, new UnityObjIdDrawer())
		{
			this.createDefaultValue = () => CreateItem(Selection.activeObject);
			this.createItem = CreateItem;
		}
		
		private UnityObjId CreateItem(Object o)
		{
            if (allowSceneObject || !AssetDatabase.GetAssetPath(o).IsEmpty())
            {
                return new UnityObjId(o);
            } else
            {
               return new UnityObjId(null);
            }
        }
	}
}
