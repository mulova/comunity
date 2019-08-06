//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using Object = UnityEngine.Object;
using mulova.commons;
using System.Text.Ex;
using System.Collections.Generic.Ex;

namespace comunity
{
    public class SerializedInspector {
        private SerializedObject obj;
        private List<SerializedProperty> properties;
        private Dictionary<string, SerializedProperty> map = new Dictionary<string, SerializedProperty>();
        
        public SerializedInspector(SerializedObject obj, params string[] varNames) {
            this.obj = obj;
            Init(varNames);
        }
        
        private string error;
        public SerializedInspector(SerializedObject obj) {
            this.obj = obj;
            BindingFlags flags = (BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy) & ~BindingFlags.SetProperty & ~BindingFlags.GetProperty;
            List<FieldInfo> vars = ReflectionUtil.ListFields(obj.targetObject.GetType(), flags, null);
            List<string> names = new List<string>();
            foreach (FieldInfo f in vars) {
                names.Add(f.Name);
            }
            Init(names.ToArray());
        }
        
        public void Exclude(params string[] varNames) {
            foreach (string name in varNames) {
                SerializedProperty p = map.Get(name);
                if (p != null) {
                    properties.Remove(p);
                    map.Remove(name);
                }
            }
        }
        
        public void Exclude(Type type) {
            foreach (PropertyInfo p in type.GetProperties()) {
                Exclude(p.Name);
            }
            foreach (FieldInfo f in type.GetFields()) {
                Exclude(f.Name);
            }
        }
        
        private void Init(string[] varNames) {
            this.properties = new List<SerializedProperty>();
            foreach (string m in varNames) {
                SerializedProperty p = obj.FindProperty(m);
                if (p != null) {
                    properties.Add(p);
                    map[m] = p;
                } else {
                    error = string.Format("No {0}.{1}", obj.targetObject.GetType().FullName, m);
                }
            }
        }
        
        public void Begin() {
            obj.Update();
        }
        
        public void End() {
            obj.ApplyModifiedProperties();
        }
        
        private HashSet<string> change = new HashSet<string>();
        public bool OnInspectorGUI() {
            if (error.IsNotEmpty()) {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                return false;
            }
            change.Clear();
            Begin();
            foreach (SerializedProperty p in properties) {
                bool b = false;
                Color c = Color.blue;
                int e = 0;
                int i = 0;
                float f = 0;
                Object o = null;
                Vector2 v2 = Vector2.zero;
                Vector3 v3 = Vector3.zero;
                string s = string.Empty;
                Rect r = new Rect();
                if (p.propertyType == SerializedPropertyType.Boolean) {
                    b = p.boolValue;
                } else if (p.propertyType == SerializedPropertyType.Color) {
                    c = p.colorValue;
                } else if (p.propertyType == SerializedPropertyType.Enum) {
                    e = p.enumValueIndex;
                } else if (p.propertyType == SerializedPropertyType.Float) {
                    f = p.floatValue;
                } else if (p.propertyType == SerializedPropertyType.Integer) {
                    i = p.intValue;
                } else if (p.propertyType == SerializedPropertyType.ObjectReference) {
                    o = p.objectReferenceValue;
                } else if (p.propertyType == SerializedPropertyType.Rect) {
                    r = p.rectValue;
                } else if (p.propertyType == SerializedPropertyType.String) {
                    s = p.stringValue;
                } else if (p.propertyType == SerializedPropertyType.Vector2) {
                    v2 = p.vector2Value;
                } else if (p.propertyType == SerializedPropertyType.Vector3) {
                    v3 = p.vector3Value;
                } 
                EditorGUILayout.PropertyField(p, true);
                if ((p.propertyType == SerializedPropertyType.Boolean && b != p.boolValue)
                    || (p.propertyType == SerializedPropertyType.Color && c != p.colorValue) 
                    || (p.propertyType == SerializedPropertyType.Enum && e != p.enumValueIndex) 
                    || (p.propertyType == SerializedPropertyType.Float && f != p.floatValue) 
                    || (p.propertyType == SerializedPropertyType.Integer && i != p.intValue) 
                    || (p.propertyType == SerializedPropertyType.ObjectReference && o != p.objectReferenceValue) 
                    || (p.propertyType == SerializedPropertyType.Rect && r != p.rectValue)
                    || (p.propertyType == SerializedPropertyType.String && s != p.stringValue)
                    || (p.propertyType == SerializedPropertyType.Vector2 && v2 != p.vector2Value)
                    || (p.propertyType == SerializedPropertyType.Vector3 && v3 != p.vector3Value)) {
                    change.Add(p.name);
                } 
            }
            End();
            return change.Count > 0;
        }
        
        public bool IsChanged(string varName) {
            return change.Contains(varName);
        }
        
        public bool OnInspectorGUI(params string[] varNames) {
            bool old = GUI.changed;
            foreach (string s in varNames) {
                SerializedProperty p = this[s];
                EditorGUILayout.PropertyField(p, true);
            }
            bool changed = GUI.changed;
            GUI.changed = old;
            return changed;
        }
        
        public SerializedProperty this[string varName] {
            get {
                return map[varName];
            }
        }
    }
}

