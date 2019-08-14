using UnityEngine;
using UnityEditor;

namespace comunity
{
	public class UnityObjIdDrawer : ItemDrawer<UnityObjId>
	{
		public bool allowSceneObject = true;

		public override bool DrawItem(Rect rect, int index, UnityObjId item, out UnityObjId newItem)
		{
			Rect[] area = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-15);
			Object obj = item?.reference;
			item.reference = EditorGUI.ObjectField(area[0], obj, typeof(Object), allowSceneObject);
			newItem = item;
			return obj != item?.reference;
		}
	}
	
}


