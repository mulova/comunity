using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace scenehistorian
{
    public class SceneHistoryReorderList : ReorderList<SceneHistoryItem>
    {
		public SceneHistoryReorderList(SceneHistory history) : base(null, history.items)
		{
			this.displayAdd = false;
            onCreateItem = CreateItem;
            onDrawItem = DrawItem;
        }

        private SceneHistoryItem CreateItem()
        {
            return new SceneHistoryItem(Selection.activeObject);
        }

        private bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
			return UnityObjIdDrawer.DrawItem(this[index].first, rect, false);
        }
    }
}


