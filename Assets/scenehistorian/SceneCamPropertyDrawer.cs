//#if !INTERNAL_REORDER
//#define CONFIRM
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
            bool changed = false;
            Rect[] r1 = EditorGUIUtil.SplitRectHorizontally(rect, -100);
            Rect[] r2 = EditorGUIUtil.SplitRectHorizontally(r1[1], 70);
            if (item == null)
            {
                item = new SceneCamProperty();
                item.Collect();
            }
            string name = EditorGUI.TextField(r1[0], item.id);
            if (GUI.Button(r2[0], "Load", EditorStyles.toolbarButton))
            {
                item.Apply();
            }
            Color bg = GUI.backgroundColor;
            #if CONFIRM
            TimeSpan diff = System.DateTime.Now-time;
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
            #endif
            if (GUI.Button(r2[1], "Save", EditorStyles.toolbarButton))
            {
                #if CONFIRM
                if (toSave == item)
                {
                    item.Collect();
                    toSave = null;
                } else
                {
                    toSave = item;
                }
                time = System.DateTime.Now;
                #else
                item.Collect();
                changed = true;
                #endif
            }
            GUI.backgroundColor = bg;
            newItem = item;
            if (name != item.id)
            {
                item.id = name;
                return true;
            } else
            {
                return changed;
            }
        }
    }
}
//#endif