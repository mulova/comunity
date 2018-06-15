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
        }

        public ObjReorderList(Object o, string varName, bool allowSceneObjects = true) : base(o, varName) {
            this.allowSceneObjects = allowSceneObjects;
        }

        protected override T GetItem(int i)
        {
            return (T)drawer.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue;
        }

        protected override void SetItem(int i, T val)
        {
            if (drawer.serializedProperty.arraySize < i)
            {
                drawer.serializedProperty.InsertArrayElementAtIndex(i);
            }
            drawer.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue = val;
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var o1 = this[index];
            var o2 = EditorGUI.ObjectField(rect, o1, typeof(T), allowSceneObjects);
            if (editable)
            {
                this[index] = o2 as T;
            }
            return editable && o1 != o2;
        }
    }
}

