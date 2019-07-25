using System.Collections;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace comunity
{
    public class UnityObjIdReorderList : ReorderList<UnityObjId> 
    {
        public bool allowSceneObject = true;

        public UnityObjIdReorderList(Object obj, IList src) : base(obj, src)
        {
        }

        protected override UnityObjId CreateItem()
        {
            return new UnityObjId(Selection.activeObject);
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
			return UnityObjIdDrawer.DrawItem(this[index], rect, allowSceneObject);
        }
    }
}
