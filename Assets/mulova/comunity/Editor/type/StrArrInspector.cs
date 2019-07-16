//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using Object = UnityEngine.Object;
using System;
using UnityEditor;

namespace comunity
{
    public class StrArrInspector : ArrInspector<string>
    {
        private string[] preset;
        public StrArrInspector(object obj, string varName) : base(obj, varName) { 
            SetTitle(null);
        }
        
        public void SetPreset(params string[] preset) {
            this.preset = preset;
        }
        
        protected override bool OnInspectorGUI(string obj, int i)
        {
            float width = GetWidth();
            if (preset != null) {
                if (EditorGUIUtil.PopupNullable<string>(null, ref obj, preset, GUILayout.MinWidth(width*0.4F))) {
                    this[i] = obj;
                    return true;
                }
            } else if (DrawEnum(ref obj, GUILayout.MinWidth(width*0.4F))) {
                this[i] = obj;
                return true;
            }
            return false;
        }
        
        protected override bool DrawFooter() {
            return false;
        }
    }
}

