using UnityEngine;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class ObjReorderList : ReorderList<Object>
    {
		private bool allowSceneObjects;

		public ObjReorderList(Object o, IList list, bool allowSceneObjects = true) : base(o, list) {
			this.allowSceneObjects = allowSceneObjects;
		}

        protected override Object CreateItem()
        {
            return null;
        }
        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var o = this[index];
            this[index] = EditorGUI.ObjectField(rect, o, typeof(Object), allowSceneObjects);
            return this[index] != o;
        }
    }
}


