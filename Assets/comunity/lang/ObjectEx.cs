//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using commons;

using comunity;

namespace UnityEngine.Ex
{
    public static class ObjectEx {
        
        private static WeakHashSet<Object> prefabPool = new WeakHashSet<Object>();
        
        
        public static void DestroyEx(this Object o) {
            if (o == null) {
                return;
            }
            if (o is GameObject) {
                (o as GameObject).SetActive(false);
            }
            if (Application.isPlaying) {
                Object.Destroy(o);
            } else {
                Object.DestroyImmediate(o);
            }
        }
        
        public static bool IsNull<T>(this T o) where T:class
        {
            if (ReferenceEquals(o, null)) {
                return true;
            }
            if (Platform.isEditor) {
                return (!(o is string) && o.ToString() == "null");
            } else {
                return false;
            }
        }
        
        public static T InstantiateEx<T>(this T prefab) where T:Object {
            return prefab.InstantiateEx(null, false);
        }
        
        public static T InstantiateEx<T>(this T prefab, Transform parent) where T:Object {
            return prefab.InstantiateEx(parent, false);
        }
        
        public static T InstantiateEx<T>(this T prefab, Transform parent, bool worldPositionStays) where T:Object {
            if (Platform.isDebug) {
                if (!prefabPool.Contains(prefab)) {
                    Debug.LogFormat("{0} is instantiated for the first time", prefab);
                    prefabPool.Add(prefab);
                }
            }
            T inst = Object.Instantiate(prefab) as T;
            inst.name = prefab.name;
            Transform trans = null;
            if (typeof(GameObject).IsAssignableFrom(typeof(T))) {
                if (parent == null)
                {
                    parent = (prefab as GameObject).transform.parent;
                }
                trans = (inst as GameObject).transform;
            } else if (typeof(Component).IsAssignableFrom(typeof(T))) {
                if (parent == null)
                {
                    parent = (prefab as Component).transform.parent;
                }
                trans = (inst as Component).transform;
            }
            if (trans != null)
            {
                trans.SetParent(parent, worldPositionStays);
            }
            return inst;
        }
    }
}

