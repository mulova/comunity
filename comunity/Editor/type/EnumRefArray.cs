//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;

namespace comunity
{
    /**
    * @T array item type
    */
    public class EnumRefArray<E> : ReflectionArray<E> where E : struct
    {
        public GUILayoutOption layout = GUILayout.ExpandWidth(false);
        public E[] preset;
        public EnumRefArray(object obj, string variableName) : base (obj, variableName) {
            preset = (E[])Enum.GetValues(typeof(E));
        }
        
        public override bool DrawValue(int i, E e) {
            if (EditorGUILayoutUtil.Popup<E>(ref e, preset, layout)) {
                this[i] = e;
                return true;
            }
            return false;
        }
        
        protected override bool IsExpanded(E data) {
            return false;
        }
    }
}
