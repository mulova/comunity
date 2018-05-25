using comunity;
using System.Collections;
using Object = UnityEngine.Object;
using UnityEditor;
using UnityEngine;

namespace comunity
{
    public class UnityObjIdReorderList : ReorderList<UnityObjId> 
    {
        public bool allowSceneObject = true;

        public UnityObjIdReorderList(Object obj, IList src) : base(obj, src)
        {
        }
        
        protected override UnityObjId createItem()
        {
            return new UnityObjId(Selection.activeGameObject);
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            UnityObjId item = this[index];
            Rect[] area = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-15);
            Object obj = item.reference;
            item.reference = EditorGUI.ObjectField(area[0], obj, typeof(Object), allowSceneObject);
            bool starred = item.starred;
            item.starred = EditorGUI.Toggle(area[1], item.starred);
            return starred != item.starred || obj != item.reference;
        }
    }
}