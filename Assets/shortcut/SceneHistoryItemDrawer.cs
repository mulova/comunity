#if !INTERNAL_REORDER
using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace convinity
{
    public class SceneHistoryItemDrawer : ItemDrawer<SceneHistoryItem>
    {
        private UnityObjIdDrawer drawer;

		public SceneHistoryItemDrawer()
		{
			this.drawer = new UnityObjIdDrawer();
			this.drawer.allowSceneObject = false;
		}

        public override bool DrawItem(Rect rect, int index, SceneHistoryItem item, out SceneHistoryItem newItem)
        {
            newItem = item;
            Rect[] area1 = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-45);
            Object obj = item.first.reference as Object;
            item.first.reference = EditorGUI.ObjectField(area1[0], obj, typeof(Object), false);
            Rect starRect = new Rect();
            Rect[] area2 = EditorGUIUtil.SplitRectHorizontally(area1[1], 30);
            if (item.camProperty.valid && GUI.Button(area2[0], "cam"))
            {
                item.camProperty.Apply();
            }
            bool starred = item.first.starred;
            item.first.starred = EditorGUI.Toggle(area2[1], item.first.starred);
            return starred != item.first.starred || obj != item.first.reference;
        }
    }
}
#endif

