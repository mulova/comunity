﻿//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using mulova.unicore;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.comunity
{
    public class ObjArrInspector<T> : ArrInspector<T> where T: Object
    {
        private bool allowSceneObj;
        
        public ObjArrInspector(object obj, string varName, bool allowSceneObj) : base(obj, varName)
        { 
            SetTitle(null);
            this.allowSceneObj = allowSceneObj;
        }
        
        protected override bool OnInspectorGUI(T obj, int i)
        {
            float width = GetWidth();
            if (EditorGUILayoutEx.ObjectField<T>(ref obj, allowSceneObj, GUILayout.MinWidth(width * 0.4F)))
            {
                this[i] = obj;
                return true;
            }
            return false;
        }
        
        protected override bool DrawFooter()
        {
            return false;
        }
    }
}
