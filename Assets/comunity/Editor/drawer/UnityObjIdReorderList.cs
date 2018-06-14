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
        }

        public UnityObjIdReorderList(Object obj, string varName) : base(obj, varName)
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

        protected override UnityObjId GetItem(int i)
        {
            return null;
        }

        protected override void SetItem(int i, UnityObjId val)
        {
        }
    }
}
