using UnityEngine;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class NamedObjDrawer : IItemDrawer<NamedObj>
	{
		private bool allowSceneObjects = true;

        public bool DrawItem(Rect rect, int index, NamedObj obj, out NamedObj changedObj)
        {
            EditorGUI.ObjectField(rect, obj.Obj, typeof(Object), allowSceneObjects);
            changedObj = obj;
            return false;
        }

        public void DrawItemBackground(Rect position, int index, NamedObj obj)
        {
            throw new System.NotImplementedException();
        }

        public float GetItemHeight(int index, NamedObj obj)
        {
            return 16;
        }

	}
}

