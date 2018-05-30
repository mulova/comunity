using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using commons;

namespace comunity
{
    public delegate string ConvToString(object o);
    public delegate Object ConvToObject(object o);

    public class ItemDrawer<T> : IItemDrawer<T> where T:class
    {
        public const float LINE_HEIGHT = 16f;
        public ConvToString _toStr = ScenePathToString;

        public ConvToString toStr
        {
            get { return _toStr; }
            set
            {
                if (value != null)
                {
                    _toStr = value;
                } else
                {
                    _toStr = ScenePathToString;
                }
            }
        }

        public ConvToObject _toObj = DefaultToObject;

        public ConvToObject toObj
        {
            get { return _toObj; }
            set
            {
                if (value != null)
                {
                    _toObj = value;
                } else
                {
                    _toObj = DefaultToObject;
                }
            }
        }

        public virtual void DrawItemBackground(Rect position, int index, T obj)
        {
        }

        public virtual bool DrawItem(Rect position, int index, T obj, out T newObj)
        {
            try
            {
                Object o = toObj(obj);
                newObj = EditorGUI.ObjectField(position, o, typeof(T), true) as T;
                return obj != newObj;
            } catch (Exception ex)
            {
                newObj = obj;
                if (!(ex is ExitGUIException))
                {
                    throw ex;
                }
                return false;
            }
        }

        public virtual float GetItemHeight(int index, T obj)
        {
            return LINE_HEIGHT;
        }

        public static string DefaultToString(object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            if (o is Object)
            {
                Object obj = o as Object;
                if (obj != null)
                {
                    if (o is GameObject)
                    {
                        return (o as GameObject).name;
                    } else if (o is Component)
                    {
                        Component c = o as Component;
                        if (c == null||c.gameObject == null)
                        {
                            return string.Empty;
                        }
                        return c.name;
                    } else
                    {
                        return (o as Object).name;
                    }
                } else
                {
                    return string.Empty;
                }
            } else if (o is ObjWrapper)
            {
                return (o as ObjWrapper).Name;
            }
            return o.ToString();
        }

        public static Object DefaultToObject(object o)
        {
            if (o is ObjWrapper)
            {
                return (o as ObjWrapper).Obj;
            } else if (o is Object)
            {
                return o as Object;
            }
            return null;
        }

        /// <summary>
        /// Get asset path or scene path from the object
        /// </summary>
        /// <returns>convert object to asset path / scene path. if none, return o.ToString() </returns>
        /// <param name="o">O.</param>
        public static string ScenePathToString(object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            try
            {
                if (o is AnimationClip)
                {
                    return (o as AnimationClip).name;
                } else if (o is Object)
                {
                    string str = AssetDatabase.GetAssetPath(o as Object);
                    if (!str.IsEmpty())
                    {
                        return str;
                    } else
                    {
                        if (o is GameObject)
                        {
                            return (o as GameObject).transform.GetScenePath();
                        } else if (o is Component)
                        {
                            Component c = o as Component;
                            if (c == null||c.gameObject == null)
                            {
                                return string.Empty;
                            }
                            return c.transform.GetScenePath();
                        }
                    }
                } else if (o is ObjWrapper)
                {
                    return (o as ObjWrapper).Name;
                }
                return DefaultToString(o);
            } catch (Exception ex)
            {
                Debug.LogException(ex);
                return string.Empty;
            }
        }
    }
}

