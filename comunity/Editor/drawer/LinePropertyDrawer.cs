﻿using UnityEngine;
using UnityEditor;

namespace comunity
{
    [CustomPropertyDrawer(typeof(LineDrawerAttribute))]
    public class LinePropertyDrawer : PropertyDrawerBase
    {
        protected override void DrawProperty(SerializedProperty p, Rect bound)
        {
            var attr = attribute as LineDrawerAttribute;
            Rect[] r = bound.SplitHorizontally(attr.names.Length);
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

