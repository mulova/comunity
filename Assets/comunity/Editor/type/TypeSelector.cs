//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Reflection;
using commons;
using System.Collections.Generic.Ex;
using System.Text.Ex;

namespace comunity
{
    public class TypeSelector
    {
        private string typeStr;
        private Type sel = typeof(Object);
        private Dictionary<string, Type> types;
        private List<Type> typeMatches = new List<Type>();
        private Type[] typeMatchesArr = new Type[0];
        
        public TypeSelector(Type baseType)
        {
            SetBaseType(baseType);
        }
        
        public void SetSelected(Type type) {
            if (type != null) {
                this.sel = type;
                this.typeStr = type.FullName;
            }
        }
        
        public Type GetSelected() {
            return sel;
        }
        
        public void SetBaseType(Type baseType) {
            List<Type> classes = ReflectionUtil.FindClasses(baseType);
            types = new Dictionary<string, Type>();
            foreach (Type t in classes)
            {
                types[t.FullName] = t;
            }
        }
        
        /// <summary>
        /// Draws the selector.
        /// </summary>
        /// <returns><c>true</c>, if selection was changed, <c>false</c> otherwise.</returns>
        public bool DrawSelector() {
            Type oldType = sel;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type", GUILayout.Width(50));
            if (((sel == null && EditorGUIUtil.TextField(null, ref typeStr))
                || (sel != null && EditorGUIUtil.TextField(null, ref typeStr, EditorStyles.toolbarTextField)))
                && typeStr.Length > 0) {
                sel = types.Get(typeStr);
                if (!typeStr.IsEmpty() && typeStr.Length >= 2) {
                    typeMatches.Clear();
                    foreach (KeyValuePair<string, Type> pair in types) {
                        if (pair.Key.IndexOf(typeStr, StringComparison.OrdinalIgnoreCase) >= 0) {
                            typeMatches.Add(pair.Value);
                        }
                    }
                    typeMatchesArr = typeMatches.ToArray();
                }
            }
            EditorGUIUtil.PopupNullable<Type>(null, ref sel, typeMatchesArr);
            EditorGUILayout.EndHorizontal();
            return oldType != sel;
        }
    }
}

