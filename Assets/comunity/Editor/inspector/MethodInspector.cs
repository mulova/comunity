using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Reflection;
using mulova.commons;

namespace comunity
{
    public class MethodInspector {
        private HashSet<Type> excludeCompTypes = new HashSet<Type>();
        private HashSet<string> excludeMethodNames = new HashSet<string>();
        
        /**
        * Add types to exclude whose methods are declared
        */
        public void excludeMethodDeclaredType(params Type[] types) {
            foreach (Type t in types) {
                excludeCompTypes.Add(t);
            }
        }
        
        public void excludeMethod(params string[] names) {
            foreach (string s in names) {
                excludeMethodNames.Add(s);
            }
        }
        
        private MethodInfo[] methodList;
        private MethodInfo method;
        private void ListMethods(Object o, string methodName) {
            if (o == null) {
                methodList = null;
            } else {
                List<MethodInfo> methods = ReflectionUtil.ListMethods(o.GetType(), MethodCall.FLAGS, excludeCompTypes);
                List<MethodInfo> list = new List<MethodInfo>();
                for (int i=0; i<methods.Count; i++) {
                    string name = methods[i].Name;
                    if (name == methodName) {
                        method = methods[i];
                    }
                    if (methods[i].ReturnType == typeof(void) && !excludeMethodNames.Contains(name)) {
                        ParameterInfo[] paramInfo = methods[i].GetParameters();
                        if (paramInfo == null || paramInfo.Length == 0) {
                            list.Add(methods[i]);
                        } else if (paramInfo.Length == 1) {
                            list.Add(methods[i]);
                        }
                    }
                }
                methodList = list.ToArray();
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
        
        public bool DrawMethodPopup(Object obj, ref string methodName) {
            ListMethods(obj, methodName);
            if (methodList != null) {
                if (EditorGUIUtil.PopupNullable<MethodInfo>("Method Name", ref method, methodList, MethodToString)) {
                    if (method == null) {
                        methodName = "";
                    } else {
                        methodName = method.Name;
                    }
                    return true;
                }
            }
            return false;
        }
        
        private string MethodToString(object o) {
            MethodInfo m = o as MethodInfo;
            string name = m.Name;
            ParameterInfo[] paramInfo = m.GetParameters();
            if (paramInfo == null || paramInfo.Length == 0) {
                return name;
            } else if (paramInfo.Length == 1) {
                return string.Format("{0}({1})", name, paramInfo[0].ParameterType.Name);
            }
            return name;
        }
        
        private string ToStringScript(object b) {
            return b.GetType().FullName;
        }
        
    }
}