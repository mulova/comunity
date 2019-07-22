using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using comunity;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class StrReorderList : ReorderList<string>
    {
        public StrReorderList(Object o, IList list) : base(o, list) {
            onDrawItem = DrawItem;
        }

        public StrReorderList(Object o, string varName) : base(o, varName) {
            onDrawItem = DrawItem;
        }

        protected override string GetSerializedItem(SerializedProperty p, int i)
        {
            return p.GetArrayElementAtIndex(i).stringValue;
        }

        protected override void SetSerializedItem(SerializedProperty p, int i, string val)
        {
            if (p.arraySize < i)
            {
                p.InsertArrayElementAtIndex(i);
            }
            p.GetArrayElementAtIndex(i).stringValue = val;
        }

        private bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
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

