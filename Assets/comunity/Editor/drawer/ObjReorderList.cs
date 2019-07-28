using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

namespace comunity
{
    public class ObjReorderList<T> : ReorderList<T> where T : Object
    {
        private bool allowSceneObjects;
        public bool editable = true;

        public ObjReorderList(IList list, bool allowSceneObjects = true) : base(list) {
            this.allowSceneObjects = allowSceneObjects;
        }

        protected override bool DrawItem(T item, Rect rect, int index, bool isActive, bool isFocused)
        {
            var item2 = EditorGUI.ObjectField(rect, item, typeof(T), allowSceneObjects);
            if (editable && item != item2)
            {
                this[index] = item2 as T;
                return true;
            } else
            {
                return false;
            }
        }
    }
}

