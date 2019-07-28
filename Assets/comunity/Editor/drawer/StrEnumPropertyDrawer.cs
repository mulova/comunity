using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using commons;


namespace comunity
{
    [CustomPropertyDrawer(typeof(StrEnumDrawerAttribute))]
    public class StrEnumPropertyDrawer : PropertyDrawerBase
    {
        protected override void DrawGUI(GUIContent label)
        {
            var attr = attribute as StrEnumDrawerAttribute;
            Rect[] r = HorizontalSplit(0, 0.7f);
            var strProp = GetProperty(attr.strVar);
            var enumProp = GetProperty(attr.enumVar);

            string str = strProp.stringValue;
            if (str == null)
            {
                str = string.Empty;
            }
            EditorGUI.PropertyField(r[0], strProp);
            EditorGUI.PropertyField(r[1], enumProp);
        }

        protected override int GetLineCount()
        {
            return 1;
        }
    }
}

