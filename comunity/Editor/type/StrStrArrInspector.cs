//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using Object = UnityEngine.Object;
using mulova.commons;

namespace comunity
{
    public class StrStrArrInspector<T> : ArrInspector<T> {
        private string keyVar;
        private string valVar;
        private string[] keyPreset;
        private string[] valPreset;
        
        public StrStrArrInspector(Object obj, string varName, string keyVarName, string valVarName) : base(obj, varName) { 
            SetTitle(null);
            this.keyVar = keyVarName;
            this.valVar = valVarName;
        }
        
        public void SetKeyPreset(string[] keyPreset) {
            this.keyPreset = keyPreset;
        }
        
        public void SetValuePreset(string[] valPreset) {
            this.valPreset = valPreset;
        }
        
        protected override bool OnInspectorGUI (T t, int i)
        {
            float width = GetWidth();
            bool changed = false;
            string key = ReflectionUtil.GetFieldValue<string>(t, keyVar);
            if (keyPreset != null && keyPreset.Length > 0) {
                if (EditorGUIUtil.PopupNullable<string>(null, ref key, keyPreset, GUILayout.MinWidth(width/2))) {
                    ReflectionUtil.SetFieldValue<string>(t, keyVar, key);
                    changed = true;
                }
            } else {
                if (EditorGUIUtil.TextField(null, ref key, GUILayout.MinWidth(width/2))) {
                    ReflectionUtil.SetFieldValue<string>(t, keyVar, key);
                    changed = true;
                }
            }
            string val = ReflectionUtil.GetFieldValue<string>(t, valVar);
            if (valPreset != null && valPreset.Length > 0) {
                if (EditorGUIUtil.PopupNullable<string>(null, ref val, valPreset, GUILayout.MinWidth(width/2))) {
                    ReflectionUtil.SetFieldValue(t, valVar, val);
                    changed = true;
                }
            } else {
                if (EditorGUIUtil.TextField(null, ref val, GUILayout.MinWidth(width/2))) {
                    ReflectionUtil.SetFieldValue(t, valVar, val);
                    changed = true;
                }
            }
            return changed;
        }
        
        protected override bool DrawFooter() {
            return false;
        }
    }
}

