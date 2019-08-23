using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(HideIfNullAttribute))]
public class HideIfNullDrawer : PropertyDrawer
{
    private bool GetDisabled(SerializedProperty property)
    {
        HideIfNullAttribute attr = attribute as HideIfNullAttribute;
        var nullProp = property.serializedObject.FindProperty(attr.nullPropertyName);
        return nullProp != null && nullProp.objectReferenceValue != null;
    }
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        HideIfNullAttribute attr = attribute as HideIfNullAttribute;
        bool disabled = GetDisabled(property);

        if (disabled)
        {
            GUI.enabled = false;
        }
        if (!attr.hide || !disabled)
        {
            EditorGUI.PropertyField(rect, property);
        }
        if (disabled)
        {
            GUI.enabled = true;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        HideIfNullAttribute attr = attribute as HideIfNullAttribute;
        bool disabled = GetDisabled(property);

        if (attr.hide && disabled)
        {
            return 0;
        } else
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
