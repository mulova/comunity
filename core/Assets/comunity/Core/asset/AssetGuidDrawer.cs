using System;
using UnityEngine;
using UnityEditor;
using editor.ex;
using Object = UnityEngine.Object;

namespace core
{
    [CustomPropertyDrawer(typeof(AssetGuid))]
    public class AssetGuidDrawer : PropertyDrawerBase
    {
        protected override int GetLineCount()
        {
            return 1;
        }

        protected override void DrawGUI(GUIContent label)
        {
            SerializedProperty guidProp = GetProperty("guid");
            string guid = guidProp.stringValue;
            if (EditorUI.GUIDField<Object>(GetLineRect(0), prop.name, ref guid))
            {
                guidProp.stringValue = guid;
                prop.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
