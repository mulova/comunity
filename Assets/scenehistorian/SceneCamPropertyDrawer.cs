//#if !INTERNAL_REORDER
using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace scenehistorian
{
    public class SceneCamPropertyDrawer : ItemDrawer<SceneCamProperty>
    {
		public SceneCamPropertyDrawer()
		{
		}

        public override bool DrawItem(Rect rect, int index, SceneCamProperty item, out SceneCamProperty newItem)
        {
            Rect[] r1 = EditorGUIUtil.SplitRectHorizontally(rect, -100);
            Rect[] r2 = EditorGUIUtil.SplitRectHorizontally(r1[1], 50);
            if (item == null)
            {
                item = new SceneCamProperty();
                item.Collect();
            }
            string name = EditorGUI.TextField(r1[0], item.id);
            if (GUI.Button(r2[0], "Save"))
            {
                item.Collect();
            }
            if (GUI.Button(r2[1], "Apply"))
            {
                item.Apply();
            }
            newItem = item;
            if (name != item.id)
            {
                item.id = name;
                return true;
            } else
            {
                return false;
            }
        }
    }
}
//#endif