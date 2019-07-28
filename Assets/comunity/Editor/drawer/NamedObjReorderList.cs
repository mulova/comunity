using UnityEngine;
using UnityEditor;
using System.Collections;

namespace comunity
{
    public class NamedObjReorderList : ReorderList<NamedObj>
	{
		private bool allowSceneObjects = true;

		public NamedObjReorderList(IList list) : base(list)
		{
        }

        protected override bool DrawItem(NamedObj item, Rect rect, int index, bool isActive, bool isFocused)
		{
			EditorGUI.ObjectField(rect, item.Obj, typeof(Object), allowSceneObjects);
			return false;
		}
	}
}

