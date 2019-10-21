using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace mulova.build
{
    public class AssetBundleDupDrawer : IItemDrawer<AssetBundleDup>
    {
        public bool DrawItem(Rect lineRect, int index, AssetBundleDup item, out AssetBundleDup changedObj)
        {
            float lineHeight = 16;
            lineRect.height = lineHeight;
            var color = GUI.backgroundColor;
            GUI.contentColor = Color.yellow;
            EditorGUI.ObjectField(lineRect, item.dup.reference, typeof(Object), false);
            GUI.contentColor = color;
            lineRect.x += 10;
            lineRect.width -= 10;
            for (int i = 0; i < item.refs.Count; ++i)
            {
                lineRect.y += lineHeight + 2;
                EditorGUI.ObjectField(lineRect, item.refs[i].reference, typeof(Object), false);
            }
            changedObj = item;
            return false;
        }

        public void DrawItemBackground(Rect position, int index, AssetBundleDup obj)
        {
        }

        public float GetItemHeight(int index, AssetBundleDup obj)
        {
            return 16;
        }
    }
}
