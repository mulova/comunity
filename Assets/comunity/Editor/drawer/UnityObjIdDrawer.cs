using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace convinity
{
    public class UnityObjIdDrawer : ItemDrawer<UnityObjId>
    {
        public override bool DrawItem(Rect rect, int index, UnityObjId item, out UnityObjId newItem)
        {
            Rect[] area = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-15);
            Object obj = item.reference;
            item.reference = EditorGUI.ObjectField(area[0], obj, typeof(Object), true);
            bool starred = item.starred;
            item.starred = EditorGUI.Toggle(area[1], item.starred);
            newItem = item;
            return starred != item.starred;
        }
    }
}


