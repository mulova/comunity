//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;

namespace mulova.comunity
{
    /**
    * @T array item type
    */
    public class ObjectRefArray<T> : ReflectionArray<T> where T : Object
    {
        public GUILayoutOption layout = GUILayout.ExpandWidth(true);
        public T[] preset;
        public bool allowSceneObject = true;
        
        public ObjectRefArray(Object objectArray, string variableName) : base(objectArray, variableName) {}
        
        public override bool DrawValue(int i, T old) {
            T t = EditorGUILayout.ObjectField(old, typeof(T), allowSceneObject, layout) as T;
            if (t != old) {
                this[i] = t;
                return true;
            }
            return false;
        }
        
        protected override bool IsExpanded(T data) {
            return data != null;
        }
    }
}
