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
    public class Vec3RefArray : ReflectionArray<Vector3>
    {
        public GUILayoutOption[] layout = new GUILayoutOption[] {GUILayout.ExpandWidth(false)};
        public Transform trans;
        public bool local;
        
        public Vec3RefArray(object obj, string variableName) : base (obj, variableName) {
            SetVertical(false);
        }
        public Vec3RefArray(Object obj, string variableName, Transform t) : base(obj, variableName) {
            SetVertical(false);
            trans = t;
        }
        
        public override bool DrawValue(int i, Vector3 old) {
            bool changed = false;
            Vector3 v = EditorGUILayout.Vector3Field("", old);
            if (v != old) {
                this[i] = v;
                changed = true;
            }
            if (trans != null && GUILayout.Button(new GUIContent("C", "Copy Transform"), GUILayout.ExpandWidth(false))) {
                if (local) {
                    this[i] = trans.localPosition;
                } else {
                    this[i] = trans.position;
                }
                changed = true;
            }
            return changed;
        }
        
        protected override bool IsExpanded(Vector3 data) {
            return false;
        }
        
        public void SetLayout(params GUILayoutOption[] options) {
            this.layout = options;
        }
    }
}
