using UnityEngine;
using System;
using Object = UnityEngine.Object;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class ObjReorderList<T> : ReorderList<T> where T:UnityEngine.Object
    {
        private bool allowSceneObjects;
        public bool editable = true;

        public ObjReorderList(IList list, bool allowSceneObjects = true) : base(null, list) {
            this.allowSceneObjects = allowSceneObjects;
        }

        protected override T GetSerializedItem(SerializedProperty p, int i)
        {
            return (T)p.GetArrayElementAtIndex(i).objectReferenceValue;
        }

        protected override void SetSerializedItem(SerializedProperty p, int i, T val)
        {
            if (p.arraySize < i)
            {
                p.InsertArrayElementAtIndex(i);
            }
            p.GetArrayElementAtIndex(i).objectReferenceValue = val;
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

