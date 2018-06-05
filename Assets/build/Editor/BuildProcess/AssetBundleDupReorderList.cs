using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace build
{
    public class AssetBundleDupReorderList : ReorderList<AssetBundleDup>
    {
        public AssetBundleDupReorderList(List<AssetBundleDup> list) : base(list) {
            showAdd = false;
            showRemove = false;
            base.drawer.elementHeightCallback = GetHeight;
        }

        private float GetHeight(int index)
        {
            return base.drawer.elementHeight * (this[index].refs.Count+1);
        }

        protected override bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            int count = this[index].refs.Count+1;
//            for (int i=0; 
            return false;
        }
    }
}
