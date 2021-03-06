﻿using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace convinity
{
    public class ShaderSearchItemDrawer : ItemDrawer<ShaderSearchItem>
	{
		public override bool DrawItem(Rect rect, int index, ShaderSearchItem item, out ShaderSearchItem newItem)
		{
            Rect[] area1 = EditorGUILayoutEx.SplitRectHorizontally(rect, 0.5f);
            Rect[] area2 = EditorGUILayoutEx.SplitRectHorizontally(area1[1], 0.5f);
			EditorGUI.SelectableLabel(area1[0], item.name);
			EditorGUI.ObjectField(area2[0], item.rend, typeof(Object), true);
			EditorGUI.ObjectField(area2[1], item.material, typeof(Material), true);
			newItem = item;
			return false;
		}
	}
}

