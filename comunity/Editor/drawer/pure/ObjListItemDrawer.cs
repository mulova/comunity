using UnityEngine;
using UnityEditor;

namespace mulova.comunity
{
    public class ObjListItemDrawer<T> : IItemDrawer<T> where T:Object
    {
        private bool allowSceneObjects;
        public bool editable = true;

        public ObjListItemDrawer(bool allowSceneObjects = true) {
            this.allowSceneObjects = allowSceneObjects;
        }

        public bool DrawItem(Rect rect, int index, T o1, out T changedObj)
        {
            var o2 = EditorGUI.ObjectField(rect, o1, typeof(T), allowSceneObjects);
            if (editable && o1 != o2)
            {
                changedObj = o2 as T;
                return true;
            }
            else
            {
                changedObj = o1;
                return false;
            }
        }

        public void DrawItemBackground(Rect position, int index, T obj)
        {
        }

        public float GetItemHeight(int index, T obj)
        {
            return 16;
        }
    }
}
