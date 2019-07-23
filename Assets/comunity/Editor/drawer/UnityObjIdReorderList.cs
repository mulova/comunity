using comunity;
using System.Collections;
using Object = UnityEngine.Object;
using UnityEditor;
using UnityEngine;

namespace comunity
{
    public class UnityObjIdReorderList : ReorderList<UnityObjId> 
    {
        public bool allowSceneObject = true;

        public UnityObjIdReorderList(Object obj, IList src) : base(obj, src)
        {
            createItem = CreateItem;
            drawItem = DrawItem;
        }

        public UnityObjIdReorderList(Object obj, string varName) : base(obj, varName)
        {
        }

        private UnityObjId CreateItem()
        {
            return new UnityObjId(Selection.activeObject);
        }

        private bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
			return UnityObjIdDrawer.DrawItem(this[index], rect, allowSceneObject);
        }
    }
}
