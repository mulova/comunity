//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Ex;
using UnityEngine;
using Object = UnityEngine.Object;
using mulova.unicore;

namespace mulova.comunity
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
            string str = t.GetFieldValue<string>(stringVar);
            if (DrawEnum(ref str, GUILayout.MinWidth(width/2))) {
                t.SetFieldValue(stringVar, str);
                changed = true;
            }
            O obj = t.GetFieldValue<O>(objVar);
            if (EditorGUILayoutUtil.ObjectField<O>(ref obj, allowSceneObj)) {
                t.SetFieldValue<O>(objVar, obj);
                changed = true;
            }
            return changed;
        }
        
        protected override bool DrawFooter() {
            return false;
        }
    }
}

