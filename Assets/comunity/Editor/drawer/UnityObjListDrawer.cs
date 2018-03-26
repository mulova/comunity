using UnityEngine;
using System.Collections;
using comunity;
using convinity;
using System.Collections.Generic;
using UnityEditor;

namespace comunity
{
}

public class UnityObjListDrawer : ListDrawer<UnityObjId>
{
    public UnityObjListDrawer(List<UnityObjId> list) : base(list, new UnityObjIdDrawer())
    {
        this.createDefaultValue = () => new UnityObjId(Selection.activeObject);
        this.createItem = CreateItem;
    }

    private UnityObjId CreateItem(Object o)
    {
        return new UnityObjId(o);
    }
}

