using UnityEngine;
using UnityEditor;

namespace comunity
{
    public class StrReorderProperty : ReorderSerialized<string>
    {
        public bool editable;

        public StrReorderProperty(SerializedObject ser, string varName, bool editable = true) : base(ser, varName)
        {
            this.editable = editable;
            this.setItem = SetItem;
            this.getItem = GetItem;
        }

        private string GetItem(SerializedProperty p)
        {
            return p.stringValue;
        }

        private void SetItem(SerializedProperty p, string value)
        {
            p.stringValue = value;
        }

        protected override void DrawItem(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
        {
            var o1 = this[index];
            if (editable)
            {
                var o2 = EditorGUI.TextField(rect, o1);
                if (o1 != o2)
                {
                    this[index] = o2;
                }
            } else
            {
                EditorGUI.LabelField(rect, o1);
            }

        }
    }
}

