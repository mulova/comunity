using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace convinity
{
    public class SceneHistoryReorderList : ReorderList<SceneHistoryItem>
    {
        protected override SceneHistoryItem createItem()
        {
            return new SceneHistoryItem(Selection.activeObject);
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            UnityObjIdDrawer.DrawItem(this[index], rect, this.allowSceneObject);
        }
    }
}


