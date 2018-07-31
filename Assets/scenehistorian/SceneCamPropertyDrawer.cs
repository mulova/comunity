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
            Rect[] r1 = EditorGUIUtil.SplitRectHorizontally(rect, 0.7f);
            string name = EditorGUI.TextField(r1[0], item.id);
            GUI.Button(r1[1], "Apply");
            newItem = item;
            if (name != item.id)
            {
                item.id = name;
                return true;
            } else
            {
                return false;
            }
            return name != item.id;
        }
    }
}
//#endif