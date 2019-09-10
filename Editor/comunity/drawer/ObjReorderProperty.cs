using UnityEngine;
using UnityEditor;

namespace mulova.comunity
{
    public class ObjReorderProperty<T> : PropertyReorder<T> where T:UnityEngine.Object
    {
        public bool allowSceneObjects = true;
        public bool editable = true;

        public ObjReorderProperty(SerializedObject ser, string varName, bool allowSceneObjects = true) : base(ser, varName) {
            this.allowSceneObjects = allowSceneObjects;
            this.setItem = SetItem;
            this.getItem = GetItem;
        }

        private T GetItem(SerializedProperty p)
        {
            return p.objectReferenceValue as T;
        }

        private void SetItem(SerializedProperty p, T value)
        {
            p.objectReferenceValue = value;
        }

        protected override void DrawItem(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
        {
            var o1 = this[index];
            var o2 = EditorGUI.ObjectField(rect, o1, typeof(T), allowSceneObjects);
            if (editable && o1 != o2)
            {
                this[index] = o2 as T;
            }
        }
    }
}

