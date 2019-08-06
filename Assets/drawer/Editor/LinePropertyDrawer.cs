using UnityEngine;
using UnityEditor;
using System;

namespace comunity
{
    [CustomPropertyDrawer(typeof(LineDrawerAttribute))]
    public class LinePropertyDrawer : PropertyDrawerBase
    {
        protected override void DrawGUI(SerializedProperty p)
        {
            var attr = attribute as LineDrawerAttribute;
            Rect[] r = SplitLineMulti(0, attr.names.Length);
            for (int i=0; i<r.Length; ++i)
            {
                var n = attr.names[i];
                var child = p.FindPropertyRelative(n);
                var changed = EditorGUI.PropertyField(r[i], child, new GUIContent(""), true);
            }
        }

        protected override int GetLineCount(SerializedProperty p)
        {
            return 1;
        }
    }
}

