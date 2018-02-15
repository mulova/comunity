//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace comunity
{
    /**
    * @T array item type
    */
    public class Vec2RefArray : ReflectionArray<Vector2>
    {
        public GUILayoutOption[] layout = new GUILayoutOption[] {GUILayout.ExpandWidth(false)};
        
        public Vec2RefArray(object obj, string variableName) : base (obj, variableName) {}
        
        public override bool DrawValue(int i, Vector2 old) {
            bool changed = false;
            Vector2 v = EditorGUILayout.Vector2Field("", old);
            if (v != old) {
                this[i] = v;
                changed = true;
            }
            return changed;
        }
        
        protected override bool IsExpanded(Vector2 data) {
            return false;
        }
        
        public void SetLayout(params GUILayoutOption[] options) {
            this.layout = options;
        }
    }
}
