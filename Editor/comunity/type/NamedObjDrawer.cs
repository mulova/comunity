using UnityEngine;
using System.Collections;
using UnityEditor;

namespace mulova.comunity
{
    public class NamedObjDrawer<T> : IItemDrawer<T> where T: NamedObj
    {
		private bool allowSceneObjects = true;

        public bool DrawItem(Rect rect, int index, T obj, out T changedObj)
        {
            EditorGUI.ObjectField(rect, obj.Obj, typeof(Object), allowSceneObjects);
            changedObj = obj;
            return false;
        }

        public void DrawItemBackground(Rect position, int index, T obj)
        {
            throw new System.NotImplementedException();
        }

        public float GetItemHeight(int index, T obj)
        {
            return 16;
        }

	}
}

