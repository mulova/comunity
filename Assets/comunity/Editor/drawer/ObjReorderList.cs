using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using comunity;
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
            drawItem = DrawItem;
        }

        public ObjReorderList(Object o, string varName, bool allowSceneObjects = true) : base(o, varName) {
            this.allowSceneObjects = allowSceneObjects;
            drawItem = DrawItem;
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

        private bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
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

