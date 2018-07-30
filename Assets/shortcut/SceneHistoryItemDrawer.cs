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
            var right = item.camProperty.valid? 50 : 10;
            newItem = item;
            Rect[] area1 = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-right);
            Object obj = item.first.reference as Object;
            item.first.reference = EditorGUI.ObjectField(area1[0], obj, typeof(Object), false);
            Rect starredRect = area1[1];
            if (item.camProperty.valid)
            {
                Rect[] area2 = EditorGUIUtil.SplitRectHorizontally(area1[1], 40);
                if (item.camProperty.valid && GUI.Button(area2[0], "cam", EditorStyles.miniButton))
                {
                    item.camProperty.Apply();
                }
                starredRect = area2[1];
            }
            bool starred = item.starred;
            item.starred = EditorGUI.Toggle(starredRect, starred);
//			if (GUI.Button(area2[1], starred ? texOn: texOff))
//			{
//				item.starred = !item.starred;
//			}
            return starred != item.starred || obj != item.first.reference;
        }
    }
}
#endif

