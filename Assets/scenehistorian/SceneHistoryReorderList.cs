﻿using UnityEngine;
using comunity;
using UnityEditor;

namespace scenehistorian
{
    public class SceneHistoryReorderList : ReorderList<SceneHistoryItem>
    {
		public SceneHistoryReorderList(SceneHistory history) : base(null, history.items)
		{
			this.showAdd = false;
		}

        protected override SceneHistoryItem CreateItem()
        {
            return new SceneHistoryItem(Selection.activeObject);
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
			return UnityObjIdDrawer.DrawItem(this[index].first, rect, false);
        }
    }
}


