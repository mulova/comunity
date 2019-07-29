using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace scenehistorian
{
    public class SceneHistoryItemDrawer : ItemDrawer<SceneHistoryItem>
    {
        private UnityObjIdDrawer drawer;
        private bool useCam;
        private GUIContent favoriate;

		public SceneHistoryItemDrawer(bool useCam)
		{
			this.drawer = new UnityObjIdDrawer();
			this.drawer.allowSceneObject = false;
            this.useCam = useCam;
            favoriate = new GUIContent(EditorGUIUtility.FindTexture("Favorite"), "Save search");
		}

        public override bool DrawItem(Rect rect, int index, SceneHistoryItem item, out SceneHistoryItem newItem)
        {
            var showCam = useCam && item.camProperty.valid;
            var rightWidth = showCam? 60 : 20;
            newItem = item;
            Rect[] area1 = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-rightWidth);
            Object obj = item.first.reference as Object;
            item.first.reference = EditorGUI.ObjectField(area1[0], obj, typeof(Object), false);
            Rect starredRect = area1[1];
            if (showCam)
            {
                Rect[] area2 = EditorGUIUtil.SplitRectHorizontally(area1[1], 40);
                if (showCam && GUI.Button(area2[0], "cam", EditorStyles.toolbarButton))
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

