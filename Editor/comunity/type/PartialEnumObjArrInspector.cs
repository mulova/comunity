//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;
using mulova.commons;
using mulova.unicore;

namespace mulova.comunity
{
    public class PartialEnumObjArrInspector<E, O> : ArrInspector<O> 
        where E:struct, IComparable, IConvertible, IFormattable
        where O: Object {
        
        private string[] enumNames;
        private string enumVarName;
        private string objVarName;
        
        public PartialEnumObjArrInspector(Object obj, string varName, string enumVarName, string objVarName) : base(obj, varName) { 
            enumNames = EnumUtil.ToStrings<E>();
            FixedLength = enumNames.Length;
            this.enumVarName = enumVarName;
            this.objVarName = objVarName;
        }
        
        protected override bool OnInspectorGUI (O o, int i)
        {
            float width = GetWidth();
            bool changed = false;
            E e = ReflectionUtil.GetFieldValue<E>(o, enumVarName);
            if (EditorGUILayoutUtil.PopupEnum<E>(null, ref e, GUILayout.MinWidth(width*0.3f))) {
                ReflectionUtil.SetFieldValue<E>(o, enumVarName, e);
                changed = true;
            }
            O obj = ReflectionUtil.GetFieldValue<O>(o, objVarName);
            if (EditorGUILayoutUtil.ObjectField<O>(ref obj, true, GUILayout.MinWidth(width*0.7f))) {
                ReflectionUtil.SetFieldValue<O>(o, objVarName, obj);
                changed = true;
            }
            return changed;
        }
        
        protected override bool DrawFooter() {
            return false;
        }
    }
}

