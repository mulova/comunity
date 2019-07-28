using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace comunity
{
    public class UnityObjIdReorderList : ReorderList<UnityObjId> 
    {
        public bool allowSceneObject = true;

        public UnityObjIdReorderList(IList src) : base(src)
        {
        }

        protected override UnityObjId CreateItem()
        {
            return new UnityObjId(Selection.activeObject);
        }

        protected override bool DrawItem(UnityObjId item, Rect rect, int index, bool isActive, bool isFocused)
        {
			return UnityObjIdDrawer.DrawItem(item, rect, allowSceneObject);
        }
    }
}
