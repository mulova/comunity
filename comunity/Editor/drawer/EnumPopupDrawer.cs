using System;
using UnityEditor;
using UnityEngine;
using mulova.commons;
using comunity;

[CustomPropertyDrawer(typeof(EnumPopupAttribute))]
public class EnumPopupDrawer : PropertyDrawer
{
    private TypeSelector typeSelector;
    private string GetTypeString(SerializedProperty property)
    {
        EnumPopupAttribute attr = attribute as EnumPopupAttribute;
        var variable = property.serializedObject.FindProperty(attr.enumVar);
        if (variable == null)
        {
            return null;
        }
        return variable.stringValue;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (typeSelector == null)
        {
            typeSelector = new TypeSelector(typeof(Enum));
        }
        typeSelector.DrawSelector(position, property);
        //var enumStr = property.stringValue;
        //var enumType = ReflectionUtil.GetType(GetTypeString(property));
        //if (enumType != null && enumType.IsEnum)
        //{
        //    Enum old = null;
        //    try
        //    {
        //        old = (Enum)Enum.Parse(enumType, enumStr, true);
        //    } catch (Exception ex)
        //    {
        //        old = (Enum)Enum.GetValues(enumType).GetValue(0);
        //    }
        //    Enum val = EditorGUI.EnumPopup(position, old);
        //    if (val != old)
        //    {
        //        property.stringValue = val.ToString();
        //        property.serializedObject.ApplyModifiedProperties();
        //    }
        //} else
        //{
        //    var val = EditorGUI.TextField(position, enumStr);
        //    if (val != enumStr)
        //    {
        //        property.stringValue = val.ToString();
        //        property.serializedObject.ApplyModifiedProperties();
        //    }
        //}
    }
}
