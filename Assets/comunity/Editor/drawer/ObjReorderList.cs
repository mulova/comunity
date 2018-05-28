using UnityEngine;
using System.Collections;
using UnityEditor;

namespace comunity
{
    public class ObjReorderList : ReorderList<Object>
    {

        public ObjReorderList(Object o, IList list) : base(o, list) { }

        protected override Object createItem()
        {
            return null;
        }
        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var o = this[index];
            this[index] = EditorGUI.ObjectField(rect, o, typeof(Object));
            return this[index] != o;
        }
    }
}


