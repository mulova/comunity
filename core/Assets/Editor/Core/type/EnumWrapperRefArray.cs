//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using commons;

namespace core
{
    /**
    * @E enum type
    */
    public class EnumWrapperRefArray<E> : ReflectionArray<EnumWrapper> where E : struct
    {
        public GUILayoutOption layout = GUILayout.ExpandWidth(false);
        public E[] preset;
        
        public EnumWrapperRefArray(object objectArray, string variableName) : base(objectArray, variableName) {
            preset = (E[])Enum.GetValues(typeof(E));
        }
        
        protected override bool IsExpanded(EnumWrapper data) {
            return true;
        }
        
        public override bool DrawValue(int i, EnumWrapper w) {
            E e = (E)Enum.Parse(typeof(E), w.Enum.ToString());
            if (EditorGUIUtil.Popup<E>(ref e, preset, layout)) {
                w.Enum = (Enum)Enum.Parse(typeof(E), e.ToString());
                return true;
            }
            return false;
        }
        
        protected override EnumWrapper Clone(EnumWrapper src) {
            EnumWrapper dst = new EnumWrapper(typeof(E));
            if (src != null) {
                dst.Enum = src.Enum;
            }
            return dst;
        }
    }
}
