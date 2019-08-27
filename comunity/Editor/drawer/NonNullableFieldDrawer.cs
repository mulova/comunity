using UnityEditor;
using UnityEngine;

namespace comunity
{
    [CustomPropertyDrawer(typeof(NonNullableFieldAttribute))]
    public class NonNullableFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Color old = GUI.color;
            if (property.objectReferenceValue == null)
            {
                GUI.color = Color.red;
            }
            EditorGUI.PropertyField(rect, property);
            if (property.objectReferenceValue == null)
            {
                GUI.color = old;
            }
        }
    }
}
