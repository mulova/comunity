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
		private Texture texOn;
		private Texture texOff;

		public SceneHistoryItemDrawer()
		{
			this.drawer = new UnityObjIdDrawer();
			this.drawer.allowSceneObject = false;
			texOn = EditorGUIUtility.Load("Assets/Editor Default Resources/starred_on.png") as Texture;
			texOff = EditorGUIUtility.Load("starred_off") as Texture;
		}

        public override bool DrawItem(Rect rect, int index, SceneHistoryItem item, out SceneHistoryItem newItem)
        {
            newItem = item;
            Rect[] area1 = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-50);
            Object obj = item.first.reference as Object;
            item.first.reference = EditorGUI.ObjectField(area1[0], obj, typeof(Object), false);
            Rect starRect = new Rect();
            Rect[] area2 = EditorGUIUtil.SplitRectHorizontally(area1[1], 40);
            if (item.camProperty.valid && GUI.Button(area2[0], "cam", EditorStyles.miniButton))
            {
                item.camProperty.Apply();
            }
            bool starred = item.starred;
			starred = EditorGUI.Toggle(area2[1], starred);
//			if (GUI.Button(area2[1], starred ? texOn: texOff))
//			{
//				item.starred = !item.starred;
//			}
            return starred != item.starred || obj != item.first.reference;
        }
    }
}
#endif

