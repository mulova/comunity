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
    public class StrObjArrInspector<T, O> : ArrInspector<T> where T:ICloneable, new() where O : Object {
        private string stringVar;
        private string objVar;
        private bool allowSceneObj; 
        
        public StrObjArrInspector(Object obj, string varName, string stringVar, string objVar, bool allowSceneObj) : base(obj, varName) { 
            SetTitle(null);
            this.stringVar = stringVar;
            this.objVar = objVar;
            this.allowSceneObj = allowSceneObj;
        }
        
        protected override bool OnInspectorGUI (T t, int i)
        {
            float width = GetWidth();
            bool changed = false;
            string str = ReflectionUtil.GetFieldValue<string>(t, stringVar);
            if (DrawEnum(ref str, GUILayout.MinWidth(width/2))) {
                ReflectionUtil.SetFieldValue<string>(t, stringVar, str);
                changed = true;
            }
            O obj = ReflectionUtil.GetFieldValue<O>(t, objVar);
            if (EditorGUIUtil.ObjectField<O>(ref obj, allowSceneObj)) {
                ReflectionUtil.SetFieldValue<O>(t, objVar, obj);
                changed = true;
            }
            return changed;
        }
        
        protected override bool DrawFooter() {
            return false;
        }
    }
}

