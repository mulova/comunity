using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using commons;
using comunity;

namespace etc
{
    public class ComponentCopy
    {
        [MenuItem("GameObject/Create Other/Copy/All Values No Add")]
        public static void CloneGameObject() {
            GameObject[] objs = Selection.gameObjects;
            GameObject src = objs[0];
            GameObject dst = objs[1];
            if (SelectSrcDst(ref src, ref dst)) {
                CopyRecursive(src, dst, IsAssignable, false);
            }
        }
        
        [MenuItem("GameObject/Create Other/Copy/All Values No Add", true)]
        public static bool IsCloneGameObject() {
            if (Selection.gameObjects == null || Selection.gameObjects.Length != 2) {
                return false;
            }
            string name1 = Selection.gameObjects[0].name;
            string name2 = Selection.gameObjects[1].name;
            return (name1.StartsWithIgnoreCase(name2) && (name1.EndsWithIgnoreCase("_src")||name1.EndsWithIgnoreCase("_dst")))
                || (name2.StartsWithIgnoreCase(name1) && (name2.EndsWithIgnoreCase("_src")||name2.EndsWithIgnoreCase("_dst")));
        }
        
        
        [MenuItem("GameObject/Create Other/Copy/Empty Values")]
        public static void CopyEmpty() {
            GameObject[] objs = Selection.gameObjects;
            GameObject src = objs[0];
            GameObject dst = objs[1];
            if (SelectSrcDst(ref src, ref dst)) {
                CopyRecursive(src, dst, IsAssignableForOnlyNullTarget, false);
            }
        }
        
        [MenuItem("GameObject/Create Other/Copy/Empty Values", true)]
        public static bool IsCopyEmpty() {
            if (Selection.gameObjects == null || Selection.gameObjects.Length != 2) {
                return false;
            }
            string name1 = Selection.gameObjects[0].name;
            string name2 = Selection.gameObjects[1].name;
            return (name1.StartsWith(name2) && (name1.EndsWithIgnoreCase("_src")||name1.EndsWithIgnoreCase("_dst")))
                || (name2.StartsWith(name1) && (name2.EndsWithIgnoreCase("_src")||name2.EndsWithIgnoreCase("_dst")));
        }
        
        
        [MenuItem("GameObject/Create Other/Copy/All Values Add Component")]
        public static void AddGameObject() {
            GameObject[] objs = Selection.gameObjects;
            GameObject src = objs[0];
            GameObject dst = objs[1];
            if (SelectSrcDst(ref src, ref dst)) {
                CopyRecursive(src, dst, IsAssignable, true);
            }
        }
        
        [MenuItem("GameObject/Create Other/Copy/All Values Add Component", true)]
        public static bool IsAddGameObject() {
            if (Selection.gameObjects == null || Selection.gameObjects.Length != 2) {
                return false;
            }
            string name1 = Selection.gameObjects[0].name;
            string name2 = Selection.gameObjects[1].name;
            return name1.EndsWith("_src") || name1.EndsWith("_dst")
                || name2.EndsWith("_src") || name2.EndsWith("_dst");
        }
        
        private static bool SelectSrcDst(ref GameObject src, ref GameObject dst) {
            if (dst.name.EndsWith("_src") || src.name.EndsWith("_dst")) {
                // swap
                GameObject temp = src;
                src = dst;
                dst = temp;
                return true;
            } else if (src.name.EndsWith("_src") || dst.name.EndsWith("_dst")) {
                return true;
            } else {
                Debug.LogError("One of the GameObject must ends with '_src' or '_dst'");
                return false;
            }
        }
        
        private static void CopyRecursive(GameObject src, GameObject dst, ObjCopy.IsAssignable assignable, bool addIfMissing) {
            CopyComponentValues(src, dst, assignable, addIfMissing);
            Dictionary<string, Transform> dstMap = new Dictionary<string, Transform>();
            foreach (Transform d in dst.transform) {
                dstMap[d.name] = d;
            }
            foreach (Transform s in src.transform) {
                Transform d = null;
                if (dstMap.TryGetValue(s.name, out d)) {
                    CopyRecursive(s.gameObject, d.gameObject, assignable, addIfMissing);
                } else {
                    Debug.LogWarning(string.Format("No GameObject '{0}'", s.name));
                }
            }
        }
        
        private static void CopyComponentValues(GameObject src, GameObject dst, ObjCopy.IsAssignable assignable, bool addIfMissing) {
            if (src.name != dst.name) {
                Debug.LogWarning(string.Format("Name is different. {0} <-> {1}", src.name, dst.name));
            }
            ObjCopy copy = new ObjCopy(true, assignable);
            copy.ExcludeType("UnityEngine.Component", "UnityEngine.Object");
            copy.ExcludeField("UnityEngine.Transform", "hasChanged");
            foreach (Component s in src.GetComponents<Component>()) {
                Component d = dst.GetComponent(s.GetType());
                if (addIfMissing && d == null) {
                    d = dst.AddComponent(s.GetType());
                }
                if (d != null) {
                    List<MemberInfo> changed = copy.SetValue(s, d);
                    CompatibilityEditor.SetDirty(d);
                    StringBuilder str = new StringBuilder();
                    foreach (MemberInfo m in changed) {
                        str.Append(dst.name).Append(".").Append(m.Name).Append("\n");
                    }
                    if (str.Length > 0) {
                        Debug.LogWarning(str.ToString());
                    }
                } else {
                    Debug.LogWarning(string.Format("Source has {0} but Missing in Dest", s.GetType().FullName));
                }
            }
            CompatibilityEditor.SetDirty(dst);
        }
        
        private static bool IsAssignableForOnlyNullTarget(MemberInfo m, object oldValue, object newValue) {
            if (ObjectUtil.Equals(oldValue, newValue)) {
                return false;
            }
            System.Type valType = null;
            if (m is PropertyInfo) {
                valType = (m as PropertyInfo).PropertyType;
            } else {
                valType = (m as FieldInfo).FieldType;
            }
            if (valType.IsClass && ((Object)oldValue) == null) {
                return true;
            }
            return false;
        }
        
        /**
        * value type and scene object refs are copied
        */
        private static bool IsAssignable(MemberInfo m, object oldValue, object newValue) {
            if (ObjectUtil.Equals(oldValue, newValue)) {
                return false;
            }
            System.Type valType = null;
            if (m is PropertyInfo) {
                valType = (m as PropertyInfo).PropertyType;
            } else {
                valType = (m as FieldInfo).FieldType;
            }
            if (valType.IsValueType || valType == typeof(string)) {
                return true;
            }
            return newValue is Object && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath((Object)newValue));
        }
        
        //  [MenuItem("Tools/unilova/Asset/Combine Mesh")]
        //  static void CombineNow() {
        //      GameObject obj = Selection.activeObject as GameObject;
        //      CombineChildrenExtended combiner = obj.AddComponent<CombineChildrenExtended>();
        //      combiner.Combine();
        //      Object.DestroyImmediate(combiner);
        //      
        //      for (int i=obj.transform.GetChildCount()-1; i>=0; i--) {
        //          GameObject child = obj.transform.GetChild(i).gameObject;    
        //          Object.DestroyImmediate(child);
        //      }
        //  }
    }
}

