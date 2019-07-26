using UnityEngine;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class ObjWrapperReorderList : ReorderList<ObjWrapper>
	{
		private bool allowSceneObjects = true;

		public ObjWrapperReorderList(Object o, IList list) : base(o, list)
		{
        }

        protected override bool DrawItem(ObjWrapper item, Rect rect, int index, bool isActive, bool isFocused)
		{
			EditorGUI.ObjectField(rect, item.Obj, typeof(Object), allowSceneObjects);
			return false;
		}
	}
}

