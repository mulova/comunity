using UnityEditor;
using UnityEngine;

namespace mulova.comunity
{
    [CustomPropertyDrawer(typeof(IfAttribute))]
    public class IfDrawer : PropertyDrawer
    {
        private IfAction GetAction(SerializedProperty property)
        {
            IfAttribute attr = attribute as IfAttribute;
            var refProp = property.serializedObject.FindProperty(attr.refPropertyPath);
            if (refProp != null)
            {
                var match = object.Equals(refProp.GetValue(), attr.value);
                if (match)
                {
                    return attr.action;
                }
                else
                {
                    switch (attr.action)
                    {
                        case IfAction.Hide:
                            return IfAction.Show;
                        case IfAction.Show:
                            return IfAction.Hide;
                        case IfAction.Disable:
                            return IfAction.Enable;
                        case IfAction.Enable:
                            return IfAction.Disable;
                        case IfAction.None:
                            return IfAction.None;
                        default:
                            throw new System.Exception("Unreachable");
                    }

                }
            }
            else
            {
               return IfAction.None;
            }
            //return refProp != null &&  == attr.value;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            IfAttribute attr = attribute as IfAttribute;
            var action = GetAction(property);

            using (new EditorGUI.DisabledScope(action == IfAction.Disable))
            {
                if (action != IfAction.Hide)
                {
                    EditorGUI.PropertyField(position, property);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            IfAttribute attr = attribute as IfAttribute;
            var action = GetAction(property);

            if (action == IfAction.Hide)
            {
                return 0;
            } else
            {
                return base.GetPropertyHeight(property, label);
            }
        }
    }
}
