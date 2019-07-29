using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using commons;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using comunity;
using System.Ex;
using System.Text.Ex;

namespace convinity
{
    public class FieldRef : ItemDrawer<FieldRef>
    {
        public string assetGuid;
        public ScenePath scenePath;
        public int compIndex;
        public FieldInfo field;
        public PropertyInfo prop;
        public Object obj;
        public string assetPath { get; private set; }

        internal Type DeclaringType
        {
            get
            {
                if (field != null)
                {
                    return field.DeclaringType;
                } else
                {
                    return prop.DeclaringType;
                }
            }
        }

        private FieldRef(Object o)
        {
            this.obj = o;
            assetPath = AssetDatabase.GetAssetPath(o);
            if (o is Component)
            {
                compIndex = (o as Component).gameObject.GetComponents(o.GetType()).FindIndex(c => c == o);
            }
            if (assetPath.IsEmpty())
            {
                assetPath = EditorSceneManager.GetActiveScene().path;
                scenePath = new ScenePath(o);
            }
            assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
        }

        public FieldRef(Object o, FieldInfo f) : this(o)
        {
            this.field = f;
        }

        public FieldRef(Object o, PropertyInfo p) : this(o)
        {
            this.prop = p;
        }

        public void SetValue(Object val)
        {
            if (field != null)
            {
                field.SetValue(obj, val);
            } else
            {
                prop.SetValue(obj, val, null);
            }
            EditorUtil.SetDirty(obj);
        }

        public override bool DrawItem(Rect position, int index, FieldRef o1, out FieldRef o2)
        {
            // invalidate obj if scene is changed
            if (obj is SceneAsset && assetPath == SceneManager.GetActiveScene().path)
            {
                obj = null;
            }

            if (obj == null)
            {
                if (scenePath == null)
                {
                    obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                } else if (assetPath == SceneManager.GetActiveScene().path)
                {
                    Transform t = scenePath.Find();
                    if (t != null)
                    {
                        if (DeclaringType == typeof(GameObject))
                        {
                            obj = t.gameObject;
                        } else if (typeof(Component).IsAssignableFrom(DeclaringType))
                        {
                            var comps = t.GetComponents(DeclaringType);
                            if (compIndex < comps.Length)
                            {
                                obj = comps[compIndex];
                            }
                        }
                    } else
                    {
                        Debug.LogWarning("Can't find ref "+scenePath);
                    }
                } else
                {
                    obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                }
            }
            if (obj != null)
            {
                string displayName = scenePath == null? 
                        string.Format("{0} [{1}]", assetPath, GetSignature()):
                        string.Format("{0} [{1}]", scenePath, GetSignature());
                Rect[] rects = EditorGUIUtil.SplitRectHorizontally(position, 0.3f);
                EditorGUI.ObjectField(rects[0], obj, typeof(Object), true);
                EditorGUI.SelectableLabel(rects[1], displayName);
            } else
            {
                EditorGUI.SelectableLabel(position, ToString());
            }
            o2 = o1;
            return false;
        }

        public override bool Equals(object thatObj)
        {
            FieldRef that = thatObj as FieldRef;
            if (that == null)
            {
                return false;
            }
            return this.obj == that.obj;
        }

        public override int GetHashCode()
        {
            return obj.GetHashCode();
        }

        public string GetSignature()
        {
            if (field != null)
            {
                return string.Concat(field.DeclaringType.Name, ".", field.Name);
            } else
            {
                return string.Concat(prop.DeclaringType.Name, ".", prop.Name);
            }
        }

        public override string ToString()
        {
            string path = scenePath == null? assetPath : string.Format("'{0}'{1}", assetPath, scenePath);
            return string.Format("{0} [{1}]", path, GetSignature());
        }
    }
}