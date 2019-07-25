using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class ObjReorderList<T> : ReorderList<T> where T:UnityEngine.Object
    {
        private bool allowSceneObjects;
        public bool editable = true;

        public ObjReorderList(Object o, IList list, bool allowSceneObjects = true) : base(o, list) {
            this.allowSceneObjects = allowSceneObjects;
        }

        public ObjReorderList(SerializedObject ser, string varName, bool allowSceneObjects = true) : base(ser, varName) {
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

