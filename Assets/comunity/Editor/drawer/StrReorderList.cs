using UnityEngine;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class StrReorderList : ReorderList<string>
    {
        public StrReorderList(IList list) : base(null, list) {
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

