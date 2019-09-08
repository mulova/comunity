using System;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace mulova.comunity
{
    [CustomPropertyDrawer(typeof(EnumArrayAttribute))]
    public class EnumArrayDrawer : PropertyDrawer
    {
        private Regex regex = new Regex(@"\[([0-9]+)\]");
        private Array GetEnumValues()
        {
            var attr = attribute as EnumArrayAttribute;
            var values = Enum.GetValues(attr.enumType);
            return values;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enums = GetEnumValues();

            var matches = regex.Matches(property.propertyPath);
            if (matches.Count > 0)
            {
                var m = matches[matches.Count - 1];
                var g = m.Groups[m.Groups.Count - 1];
                int i = int.Parse(g.Value);
                Color c = GUI.color;
                if (i >= enums.Length)
                {
                    GUI.color = Color.red;
                    EditorGUI.PropertyField(position, property);
                    GUI.color = c;
                } else
                {
                    var e = enums.GetValue(i);
                    EditorGUI.PropertyField(position, property, new GUIContent(e.ToString()));
                }

            }
        }
    }
}
