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
            drawItem = DrawItem;
        }

        private bool DrawItem(Rect position, int index, bool isActive, bool isFocused)
        {
            FieldRef fr = this[index];
            // invalidate obj if scene is changed
            if (obj is SceneAsset && fr.assetPath == SceneManager.GetActiveScene().path)
            {
                obj = null;
            }

            if (obj == null)
            {
                if (fr.scenePath == null)
                {
                    obj = AssetDatabase.LoadAssetAtPath<Object>(fr.assetPath);
                } else if (fr.assetPath == SceneManager.GetActiveScene().path)
                {
                    Transform t = fr.scenePath.Find();
                    if (t != null)
                    {
                        if (fr.DeclaringType == typeof(GameObject))
                        {
                            obj = t.gameObject;
                        } else if (typeof(Component).IsAssignableFrom(fr.DeclaringType))
                        {
                            var comps = t.GetComponents(fr.DeclaringType);
                            if (fr.compIndex < comps.Length)
                            {
                                obj = comps[fr.compIndex];
                            }
                        }
                    } else
                    {
                        Debug.LogWarning("Can't find ref "+fr.scenePath);
                    }
                } else
                {
                    obj = AssetDatabase.LoadAssetAtPath<Object>(fr.assetPath);
                }
            }
            if (obj != null)
            {
                string displayName = fr.scenePath == null? 
                    string.Format("{0} [{1}]", fr.assetPath, fr.GetSignature()):
                    string.Format("{0} [{1}]", fr.scenePath, fr.GetSignature());
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


