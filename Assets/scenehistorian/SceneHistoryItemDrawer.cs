#if !INTERNAL_REORDER
using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace scenehistorian
{
    public class SceneHistoryItemDrawer : ItemDrawer<SceneHistoryItem>
    {
        private UnityObjIdDrawer drawer;
        private GUIContent favoriate;

		public SceneHistoryItemDrawer()
		{
			this.drawer = new UnityObjIdDrawer();
			this.drawer.allowSceneObject = false;
            favoriate = new GUIContent(EditorGUIUtility.FindTexture("Favorite"), "Save search");
		}

        public override bool DrawItem(Rect rect, int index, SceneHistoryItem item, out SceneHistoryItem newItem)
        {
            bool camValid = item.camProperty != null && item.camProperty.valid;
            var right = camValid? 60 : 20;
            newItem = item;
            Rect[] area1 = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-right);
            Object obj = item.first.reference as Object;
            item.first.reference = EditorGUI.ObjectField(area1[0], obj, typeof(Object), false);
            Rect starredRect = area1[1];
            if (camValid)
            {
                Rect[] area2 = EditorGUIUtil.SplitRectHorizontally(area1[1], 40);
                if (camValid && GUI.Button(area2[0], "cam", EditorStyles.toolbarButton))
                {
                    item.camProperty.Apply();
                }
                starredRect = area2[1];
            }
            bool starred = item.starred;
            Color cc = GUI.contentColor;
            GUI.contentColor = starred? Color.cyan: Color.black;
                
            if (GUI.Button(starredRect, favoriate, EditorStyles.toolbarButton))
            {
                item.starred = !item.starred;
            }
            GUI.contentColor = cc;
            return starred != item.starred || obj != item.first.reference;
        }
    }
}
#endif

