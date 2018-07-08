using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace comunity
{
	#if !INTERNAL_REORDER
	public class UnityObjIdDrawer : ItemDrawer<UnityObjId>
	{
		public bool allowSceneObject = true;

		public override bool DrawItem(Rect rect, int index, UnityObjId item, out UnityObjId newItem)
		{
			Rect[] area = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-15);
			Object obj = item.reference;
			item.reference = EditorGUI.ObjectField(area[0], obj, typeof(Object), allowSceneObject);
			newItem = item;
			return obj != item.reference;
		}

		public static bool DrawItem(UnityObjId item, Rect rect, bool allowSceneObject)
		{
			Rect[] area = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-15);
			Object obj = item.reference;
			item.reference = EditorGUI.ObjectField(area[0], obj, typeof(Object), allowSceneObject);
			return obj != item.reference;
		}
	}
	#else
    [CustomPropertyDrawer(typeof(UnityObjId))]
    public class UnityObjIdDrawer : PropertyDrawerBase
    {
        protected override int GetLineCount()
        {
            return 1;
        }

        protected override void DrawGUI(GUIContent label)
        {
			// TODOM
//            EditorGUIUtil.ObjectField();
        }

        public static bool DrawItem(UnityObjId item, Rect rect, bool allowSceneObject)
        {
            Rect[] area = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-15);
            Object obj = item.reference;
			item.reference = EditorGUI.ObjectField(area[0], obj, typeof(Object), allowSceneObject);
			return obj != item.reference;
        }
    }
	#endif
}


