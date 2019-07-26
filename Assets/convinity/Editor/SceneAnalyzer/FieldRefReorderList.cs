using UnityEngine;
using System.Collections;
using comunity;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections.Generic;

namespace convinity
{
    public class FieldRefReorderList : ReorderList<FieldRef>
    {
		public FieldRefReorderList(List<FieldRef> list): base(null, list)
		{
			displayAdd = false;
			displayRemove = false;
        }

        protected override bool DrawItem(FieldRef item, Rect position, int index, bool isActive, bool isFocused)
        {
            // invalidate obj if scene is changed
            if (obj is SceneAsset && item.assetPath == SceneManager.GetActiveScene().path)
            {
                obj = null;
            }

            if (obj == null)
            {
                if (item.scenePath == null)
                {
                    obj = AssetDatabase.LoadAssetAtPath<Object>(item.assetPath);
                } else if (item.assetPath == SceneManager.GetActiveScene().path)
                {
                    Transform t = item.scenePath.Find();
                    if (t != null)
                    {
                        if (item.DeclaringType == typeof(GameObject))
                        {
                            obj = t.gameObject;
                        } else if (typeof(Component).IsAssignableFrom(item.DeclaringType))
                        {
                            var comps = t.GetComponents(item.DeclaringType);
                            if (item.compIndex < comps.Length)
                            {
                                obj = comps[item.compIndex];
                            }
                        }
                    } else
                    {
                        Debug.LogWarning("Can't find ref "+item.scenePath);
                    }
                } else
                {
                    obj = AssetDatabase.LoadAssetAtPath<Object>(item.assetPath);
                }
            }
            if (obj != null)
            {
                string displayName = item.scenePath == null? 
                    string.Format("{0} [{1}]", item.assetPath, item.GetSignature()):
                    string.Format("{0} [{1}]", item.scenePath, item.GetSignature());
                Rect[] rects = EditorGUIUtil.SplitRectHorizontally(position, 0.3f);
                EditorGUI.ObjectField(rects[0], obj, typeof(Object), true);
                EditorGUI.SelectableLabel(rects[1], displayName);
            } else
            {
                EditorGUI.SelectableLabel(position, ToString());
            }
            return false;
        }
    }
}


