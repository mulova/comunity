using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace comunity
{
	public class ObjWrapperReorderList : ReorderList<ObjWrapper>
	{
		private bool allowSceneObjects = true;

		public ObjWrapperReorderList(Object o, IList list) : base(o, list)
		{
		}

		protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
		{
			var o1 = this[index];
			EditorGUI.ObjectField(rect, o1.Obj, typeof(Object), allowSceneObjects);
			return false;
		}
	}
}

