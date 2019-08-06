using UnityEngine;
using UnityEditor;

namespace comunity
{
    public class StrReorderProperty : ReorderProperty<string>
    {
        public bool editable;

        public StrReorderProperty(SerializedObject ser, string varName, bool editable = true) : base(ser, varName)
        {
            this.editable = editable;
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
            if (editable)
            {
                var o2 = EditorGUI.TextField(rect, o1);
                if (o1 != o2)
                {
                    this[index] = o2;
                    return true;
                }
                else
                {
                    return false;
                }
            } else
            {
                EditorGUI.LabelField(rect, o1);
                return false;
            }

        }
    }
}

