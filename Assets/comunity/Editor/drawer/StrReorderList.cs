using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class StrReorderList : ReorderList<string>
    {
        public StrReorderList(Object o, IList list) : base(o, list) {
        }

        protected override bool DrawItem(string item, Rect rect, int index, bool isActive, bool isFocused)
        {
            var item2 = EditorGUI.TextField(rect, item);
            if (item != item2)
            {
                this[index] = item2;
                return true;
            } else
            {
                return false;
            }
        }
    }
}

