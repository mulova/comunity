using UnityEngine;
using System.Collections;
using comunity;
using convinity;
using System.Collections.Generic;
using UnityEditor;

namespace comunity
{
}

public class SceneHistoryDrawer : ListDrawer<SceneHistoryItem>
{
    public SceneHistoryDrawer(SceneHistory list) : base(list, new SceneHistoryItemDrawer())
    {
        this.createDefaultValue = () => new SceneHistoryItem(Selection.activeObject);
        this.createItem = CreateItem;
    }

    private SceneHistoryItem CreateItem(Object o)
    {
        return new SceneHistoryItem(o);
    }
}
