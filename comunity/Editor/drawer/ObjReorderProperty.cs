using UnityEngine;
using UnityEditor;

namespace comunity
{
    public class ObjReorderProperty<T> : ReorderProperty<T> where T:UnityEngine.Object
    {
        public bool allowSceneObjects;
        public bool editable = true;

        public ObjReorderProperty(SerializedObject ser, string varName, bool allowSceneObjects = true) : base(ser, varName) {
            this.allowSceneObjects = allowSceneObjects;
        }

        protected override T GetItem(SerializedProperty p)
        {
            return p.objectReferenceValue as T;
        }

        protected override void SetItem(SerializedProperty p, T value)
        {
            p.objectReferenceValue = value;
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var o1 = this[index];
            var o2 = EditorGUI.ObjectField(rect, o1, typeof(T), allowSceneObjects);
            if (editable && o1 != o2)
            {
                this[index] = o2 as T;
                return true;
            } else
            {
                return false;
            }
        }
    }
}

