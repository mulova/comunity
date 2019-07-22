using UnityEngine;
using System.Collections;
using comunity;
using System.Collections.Generic;
using UnityEditor;

namespace convinity
{
	public class ShaderSearchReorderList : ReorderList<ShaderSearchItem>
	{
		public ShaderSearchReorderList(List<ShaderSearchItem> list) : base(null, list)
		{
			this.displayAdd = false;
			this.displayRemove = false;
            onDrawItem = DrawItem;
        }

		private bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
		{
			var item = this[index];
			Rect[] area1 = EditorGUIUtil.SplitRectHorizontally(rect, 0.5f);
			Rect[] area2 = EditorGUIUtil.SplitRectHorizontally(area1[1], 0.5f);
			EditorGUI.SelectableLabel(area1[0], item.name);
			EditorGUI.ObjectField(area2[0], item.rend, typeof(Object), true);
			EditorGUI.ObjectField(area2[1], item.material, typeof(Material), true);
			return false;
		}
	}
}


