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

