using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace comunity
{
    [CustomPropertyDrawer(typeof(ReorderableAttribute))]
    public class ReorderableDrawer : PropertyDrawer
    {
        private ReorderableList list;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.isArray)
            {
                list.DoLayoutList();
            } else
            {
                EditorGUI.PropertyField(position, property, label, true);
                base.OnGUI(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isArray)
            {
                if (list == null)
                {
                    list = new ReorderableList(property.serializedObject, property);
                }
                return list.GetHeight();
            } else
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
                //var height = base.GetPropertyHeight(property, label);
                //return height;
            }
        }
    }
}
