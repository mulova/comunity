using UnityEditor;
using Object = UnityEngine.Object;

namespace comunity
{
    [CustomPropertyDrawer(typeof(AssetGuid))]
    public class AssetGuidDrawer : PropertyDrawerBase
    {
        protected override int GetLineCount(SerializedProperty p)
        {
            return 1;
        }

        protected override void DrawGUI(SerializedProperty p)
        {
            SerializedProperty guidProp = p.FindPropertyRelative("guid");
            string guid = guidProp.stringValue;
            if (EditorUI.GUIDField<Object>(GetLineRect(0), p.name, ref guid))
            {
                guidProp.stringValue = guid;
                p.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
