using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System;
using Object = System.Object;
using mulova.unicore;

namespace mulova.comunity
{
    [CustomEditor(typeof(MethodCall))]
    public class MethodCallInspector : Editor {
        private GameObject refObj;
        private MethodCall method;
        private MethodInspector methodInspector;
        private SerializedInspector varInspector;
        
        void OnEnable() {
            method = (MethodCall)target;
            varInspector = new SerializedInspector(new SerializedObject(target), "forceActive");
            methodInspector = new MethodInspector();
            methodInspector.excludeMethodDeclaredType(typeof(MonoBehaviour), typeof(Component), typeof(Transform), typeof(Object));
            methodInspector.excludeMethod("ToString", "Awake", "Start", 
                "Update", "LateUpdate", "OnEnable", "OnDisable",
                "OnDrawGizmos");
            if (method.target != null) {
                refObj = method.target.gameObject;
            }
        }
        
        
        override public void OnInspectorGUI() {
            bool changed = methodInspector.DrawComponentPopup(ref refObj, ref method.target); 
            changed |= methodInspector.DrawMethodPopup(method.target, ref method.methodName);
            if (changed) {
                EditorUtil.SetDirty(method.gameObject);
            }
            varInspector.OnInspectorGUI();
        }
        
    }
}

