using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.comunity
{
    [CustomPropertyDrawer(typeof(AssetGuid))]
    public class AssetGuidDrawer : PropertyDrawerBase
    {
        protected override int GetLineCount(SerializedProperty p)
        {
            return 1;
        }

        protected override void OnGUI(SerializedProperty p, Rect bound)
        {
            SerializedProperty guidProp = p.FindPropertyRelative("guid");
            string guid = guidProp.stringValue;
            if (EditorUI.GUIDField<Object>(bound, p.name, ref guid))
            {
                guidProp.stringValue = guid;
                p.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
