//#if !INTERNAL_REORDER
using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;
using System;

namespace scenehistorian
{
    public class SceneCamPropertyDrawer : ItemDrawer<SceneCamProperty>
    {
        public const int CONFIRM_PERIOD = 2;

		public SceneCamPropertyDrawer()
		{
		}

        private static DateTime time;
        private static SceneCamProperty toSave;
        public override bool DrawItem(Rect rect, int index, SceneCamProperty item, out SceneCamProperty newItem)
        {
            Rect[] r1 = EditorGUIUtil.SplitRectHorizontally(rect, -100);
            Rect[] r2 = EditorGUIUtil.SplitRectHorizontally(r1[1], 60);
            if (item == null)
            {
                item = new SceneCamProperty();
                item.Collect();
            }
            string name = EditorGUI.TextField(r1[0], item.id);
            if (GUI.Button(r2[0], "Apply"))
            {
                item.Apply();
            }
            TimeSpan diff = System.DateTime.Now-time;
            Color bg = GUI.backgroundColor;
            if (diff.TotalSeconds < CONFIRM_PERIOD)
            {
                if (toSave == item)
                {
                    GUI.backgroundColor = Color.red;
                }
            } else
            {
                toSave = null;
            }
            if (GUI.Button(r2[1], "Save"))
            {
                if (toSave == item)
                {
                    item.Collect();
                    toSave = null;
                } else
                {
                    toSave = item;
                }
                time = System.DateTime.Now;
            }
            GUI.backgroundColor = bg;
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