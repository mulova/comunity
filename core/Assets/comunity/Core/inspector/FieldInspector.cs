using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Reflection;
using commons;

namespace core
{
    public class FieldInspector {
        private HashSet<Type> excludeDeclaringTypes = new HashSet<Type>();
        private HashSet<string> excludeFieldNames = new HashSet<string>();
        private static readonly BindingFlags FIELD_BINDING = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        private static readonly BindingFlags PROPERTY_BINING = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        
        /**
        * Add types to exclude whose methods are declared
        */
        public void ExcludeFieldDeclaredType(params Type[] types) {
            foreach (Type t in types) {
                excludeDeclaringTypes.Add(t);
            }
            fieldArr = null;
        }
        
        public void ExcludeField(params string[] names) {
            foreach (string s in names) {
                excludeFieldNames.Add(s);
            }
            fieldArr = null;
        }
        
        private FieldInfo[] fieldArr;
        private PropertyInfo[] propArr;
        private Type type;
        private void ListFields(Type type) {
            if (type == null) {
                fieldArr = null;
                propArr = null;
            } else if (this.type != type || fieldArr == null) {
                this.type = type;
                List<FieldInfo> fields = ReflectionUtil.ListFields(type, FIELD_BINDING, excludeDeclaringTypes);
                List<FieldInfo> fList = new List<FieldInfo>();
                foreach (FieldInfo f in fields) {
                    if (!excludeFieldNames.Contains(f.Name)) {
                        fList.Add(f);
                    }
                }
                fieldArr = fList.ToArray();
                List<PropertyInfo> props = ReflectionUtil.ListProperty(type, PROPERTY_BINING, excludeDeclaringTypes);
                List<PropertyInfo> pList = new List<PropertyInfo>();
                foreach (PropertyInfo p in props) {
                    if (!excludeFieldNames.Contains(p.Name) && p.CanRead && p.CanWrite) {
                        pList.Add(p);
                    }
                }
                propArr = pList.ToArray();
            }
        }
        
        
        public bool DrawComponentPopup(ref GameObject obj, ref MonoBehaviour comp) {
            bool changed = EditorGUIUtil.ObjectField<GameObject>("GameObject", ref obj, true);
            if (obj != null) {
                MonoBehaviour[] comps = obj.GetComponents<MonoBehaviour>();
                changed |= EditorGUIUtil.PopupNullable<MonoBehaviour>(null, ref comp, comps, ToStringScript);
            }
            return changed;
        }
        
        public bool DrawFieldPopup(Type type, ref FieldInfo field) {
            ListFields(type);
            if (fieldArr != null) {
                if (EditorGUIUtil.PopupNullable<FieldInfo>("Field Name", ref field, fieldArr, FieldToString)) {
                    return true;
                }
            }
            return false;
        }
        
        public bool DrawPropertyPopup(Type type, ref PropertyInfo prop) {
            ListFields(type);
            if (propArr != null) {
                if (EditorGUIUtil.PopupNullable<PropertyInfo>("Property Name", ref prop, propArr, PropertyToString)) {
                    return true;
                }
            }
            return false;
        }
        
        private string PropertyToString(object o) {
            PropertyInfo p = o as PropertyInfo;
            return string.Format("{0} ({1})", p.Name, p.PropertyType.Name);
        }
        
        private string FieldToString(object o) {
            FieldInfo f = o as FieldInfo;
            return string.Format("{0} ({1})", f.Name, f.FieldType.Name);
        }
        
        private string ToStringScript(object b) {
            return b.GetType().FullName;
        }
        
    }
}