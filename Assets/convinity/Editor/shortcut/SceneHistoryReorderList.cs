using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace convinity
{
    public class SceneHistoryReorderList : ReorderList<SceneHistoryItem>
    {
		public SceneHistoryReorderList(SceneHistory history) : base(null, history.items)
		{
			this.showAdd = false;
		}

        protected override SceneHistoryItem createItem()
        {
            return new SceneHistoryItem(Selection.activeObject);
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
			return UnityObjIdDrawer.DrawItem(this[index].first, rect, false);
        }
    }
}


