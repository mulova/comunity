//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Ex;
using UnityEngine;
using Object = UnityEngine.Object;
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
            E e = o.GetFieldValue<E>(enumVarName);
            if (EditorGUILayoutEx.PopupEnum<E>(null, ref e, GUILayout.MinWidth(width*0.3f))) {
                o.SetFieldValue<E>(enumVarName, e);
                changed = true;
            }
            O obj = o.GetFieldValue<O>(objVarName);
            if (EditorGUILayoutEx.ObjectField<O>(ref obj, true, GUILayout.MinWidth(width*0.7f))) {
                o.SetFieldValue<O>(objVarName, obj);
                changed = true;
            }
            return changed;
        }
        
        protected override bool DrawFooter() {
            return false;
        }
    }
}

