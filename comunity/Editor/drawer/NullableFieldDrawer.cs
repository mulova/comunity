using UnityEditor;
using UnityEngine;

namespace mulova.comunity
{
    [CustomPropertyDrawer(typeof(NullableFieldAttribute))]
    public class NullableFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Color old = GUI.contentColor;
            if (property.objectReferenceValue == null)
            {
                GUI.contentColor = new Color32(211, 211, 211, 255);
            }
            EditorGUI.PropertyField(rect, property);
            if (property.objectReferenceValue == null)
            {
                GUI.contentColor = old;
            }
        }
    }
}
