using UnityEngine;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class ObjReorderList<T> : ReorderList<T> where T: Object
    {
		private bool allowSceneObjects;
		public bool editable = true;

		public ObjReorderList(Object o, IList list, bool allowSceneObjects = true) : base(o, list) {
			this.allowSceneObjects = allowSceneObjects;
		}

		public ObjReorderList(Object o, string varName, bool allowSceneObjects = true) : base(o, varName) {
			this.allowSceneObjects = allowSceneObjects;
		}

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
			var o1 = this[index];
			var o2 = EditorGUI.ObjectField(rect, o1, typeof(T), allowSceneObjects);
			if (editable)
			{
				this[index] = o2 as T;
			}
            return o1 != o2;
        }
    }
}


