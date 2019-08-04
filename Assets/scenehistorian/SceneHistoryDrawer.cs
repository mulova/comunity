﻿using UnityEngine;
using UnityEditor;
using comunity;

namespace scenehistorian
{
	public class SceneHistoryDrawer : ListDrawer<SceneHistoryItem>
	{
        public SceneHistoryDrawer(SceneHistory list) : base(list.items, new SceneHistoryItemDrawer(list.useCam))
		{
			this.createDefaultValue = () => new SceneHistoryItem(Selection.activeObject);
			this.createItem = CreateItem;
		}
		
		private SceneHistoryItem CreateItem(Object o)
		{
			return new SceneHistoryItem(o);
		}
	}
}
