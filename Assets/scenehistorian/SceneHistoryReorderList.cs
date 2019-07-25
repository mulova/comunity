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
        }

        protected override void FillNewItem(object o)
        {
            var i = o as SceneHistoryItem;
            i.list.Add(new UnityObjId(Selection.activeObject));
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
			return UnityObjIdDrawer.DrawItem(this[index].first, rect, false);
        }
    }
}


