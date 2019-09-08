using UnityEngine;
using mulova.comunity;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace convinity
{
    public class FieldRefDrawer : IItemDrawer<FieldRef>
    {
        public bool DrawItem(Rect position, int index, FieldRef item, out FieldRef changedObj)
        {
            // invalidate obj if scene is changed
            if (item.obj is SceneAsset && item.assetPath == SceneManager.GetActiveScene().path)
            {
                item.obj = null;
            }

            if (item.obj == null)
            {
                if (item.scenePath == null)
                {
                    item.obj = AssetDatabase.LoadAssetAtPath<Object>(item.assetPath);
                }
                else if (item.assetPath == SceneManager.GetActiveScene().path)
                {
                    Transform t = item.scenePath.Find();
                    if (t != null)
                    {
                        if (item.DeclaringType == typeof(GameObject))
                        {
                            item.obj = t.gameObject;
                        }
                        else if (typeof(Component).IsAssignableFrom(item.DeclaringType))
                        {
                            var comps = t.GetComponents(item.DeclaringType);
                            if (item.compIndex < comps.Length)
                            {
                                item.obj = comps[item.compIndex];
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Can't find ref " + item.scenePath);
                    }
                }
                else
                {
                    item.obj = AssetDatabase.LoadAssetAtPath<Object>(item.assetPath);
                }
            }
            if (item.obj != null)
            {
                string displayName = item.scenePath == null ?
                    string.Format("{0} [{1}]", item.assetPath, item.GetSignature()) :
                    string.Format("{0} [{1}]", item.scenePath, item.GetSignature());
                Rect[] rects = EditorGUILayoutUtil.SplitRectHorizontally(position, 0.3f);
                EditorGUI.ObjectField(rects[0], item.obj, typeof(Object), true);
                EditorGUI.SelectableLabel(rects[1], displayName);
            }
            else
            {
                EditorGUI.SelectableLabel(position, ToString());
            }
            changedObj = item;
            return false;
        }

        public void DrawItemBackground(Rect position, int index, FieldRef obj)
        {
            throw new System.NotImplementedException();
        }

        public float GetItemHeight(int index, FieldRef obj)
        {
            throw new System.NotImplementedException();
        }

    }
}


