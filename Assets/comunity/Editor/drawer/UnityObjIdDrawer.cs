using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;

namespace convinity
{
    [CustomPropertyDrawer(typeof(UnityObjId))]
    public class UnityObjIdDrawer : PropertyDrawerBase
    {
        protected override int GetLineCount()
        {
            return 1;
        }

        protected override void DrawGUI(GUIContent label)
        {
            EditorGUIUtil.ObjectField();
        }

        public static bool DrawItem(UnityObjId item, Rect rect, bool allowSceneObject)
        {
            Rect[] area = EditorGUIUtil.SplitRectHorizontally(rect, (int)rect.width-15);
            Object obj = item.reference;
            item.reference = EditorGUI.ObjectField(area[0], obj, typeof(Object), allowSceneObject);
            bool starred = item.starred;
            item.starred = EditorGUI.Toggle(area[1], item.starred);
            return starred != item.starred || obj != item.reference;
        }
    }
}


