﻿using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class StrReorderList : ReorderList<string>
    {
        public StrReorderList(Object o, IList list) : base(o, list) {
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var o1 = this[index];
            var o2 = EditorGUI.TextField(rect, o1);
            if (o1 != o2)
            {
                this[index] = o2;
                return true;
            } else
            {
                return false;
            }
        }
    }
}

