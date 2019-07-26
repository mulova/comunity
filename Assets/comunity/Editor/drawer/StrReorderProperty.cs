using UnityEngine;
using UnityEditor;

namespace comunity
{
    public class StrReorderProperty : ReorderProperty<string>
    {
        public StrReorderProperty(SerializedObject ser, string varName) : base(ser, varName) {
        }

        protected override string GetItem(SerializedProperty p)
        {
            return p.stringValue;
        }

        protected override void SetItem(SerializedProperty p, string value)
        {
            p.stringValue = value;
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

