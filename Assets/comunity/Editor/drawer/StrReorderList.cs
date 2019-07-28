using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

namespace comunity
{
    public class StrReorderList : ReorderList<string>
    {
        public StrReorderList(IList list) : base(list) {
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

