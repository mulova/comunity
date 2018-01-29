//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using commons;


namespace comunity
{
    /**
    * SerializedObject를 사용하지 않는 ArrayInspector
    * @T array item type
    */
    public abstract class ReflectionArray<T> : IEnumerable<T>
    {
        private object obj;
        public readonly string variableName;
        public string title;
        private T[] arr;
        private int minLength;
        public bool[] fold;
        private bool vertical = true;
        private bool expand = true;
        public int separatorCount = 0;
        public bool dynamicReference = false; // true if reference can be changed
        public int indentLevel = 1;
        public bool resizable = true;
        
        public object Obj {
            get { return obj; }
        }
        
        public int MinLength {
            get { return minLength; }
            set {
                minLength = value;
                if (Length < minLength) {
                    Length = minLength;
                }
            }
        }
        
        public T this[int index] {
            get { return Arr[index]; }
            set { Arr[index] = value; }
        }
        
        public int Length {
            get {
                return Arr.Length;
            }
            set {
                Array.Resize(ref arr, value);
                Arr = arr;
            }
        }
        
        public T[] Arr {
            get {
                if (dynamicReference || arr == null) {
                    arr = ReflectionUtil.GetFieldValue<T[]>(obj, variableName);
                }
                return arr;
            } set {
                arr = value;
                ReflectionUtil.SetFieldValue<T[]>(obj, variableName, arr);
            }
        }
        
        /**
        * @param obj array를 포함하고 있는 object
        */
        public ReflectionArray(object obj, string variableName)
        {
            this.obj = obj;
            this.variableName = variableName;
            this.title = variableName;
            this.arr = ReflectionUtil.GetFieldValue<T[]>(obj, variableName);
            if (arr == null) {
                Arr = (T[])Array.CreateInstance(typeof(T), 0);
            }
            InitFolding();
        }
        
        private void InitFolding() {
            this.fold = new bool[Length];
            for(int i=0; i< fold.Length; i++) {
                fold[i] = IsExpanded(Arr[i]);
                expand |= fold[i];
            }
        }
        
        /**
        * vertical = false 일 경우 사용되지 않는다.
        * @return bool 초기화시 item 을 펼칠것인지 여부 
        */
        protected abstract bool IsExpanded(T data);
        
        public void SetVertical(bool vertical) {
            this.vertical = vertical;
        }
        
        
        /**
        * @return 값이 바뀌었으면 true
        */
        public abstract bool DrawValue(int index, T item);
        
        protected virtual bool DrawHeader(int i, T item) { 
            string itemName = "";
            if (item != null) {
                itemName = item.ToString();
            }
            fold[i] = EditorGUILayout.Foldout(fold[i], itemName);
            return false; 
        }
        
        protected virtual bool DrawInspectorGUI(int i) {
            if (fold == null) {
                InitFolding();
            }
            bool changed = false;
            if (vertical) {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                changed = DrawHeader(i, this[i]);
                EditorGUILayout.EndHorizontal();
            }
            if (fold[i] || !vertical) {
                EditorGUI.indentLevel += indentLevel;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField((i+1).ToString(), GUILayout.Width(40));
                changed |= DrawValue(i, this[i]);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel -= indentLevel;
            }
            if (vertical) {
                EditorGUILayout.EndVertical();
            }
            return changed;
        }
        
        protected virtual T Clone(T src) {
            if (typeof(T).IsValueType) {
                return src;
            } else if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T))) {
                return default(T);
            } else {
                return ReflectionUtil.NewInstance<T>();
            }
        }
        
        
        
        private List<T> list = new List<T>();
        public bool OnInspectorGUI() {
            list.Clear();
            bool changed = false;
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(title)) {
                expand = EditorGUILayout.Foldout(expand, new GUIContent(title));
            }
            if (Length == 0) {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add")) {
                    list.Add(Clone(default(T)));
                    changed = true;
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            if (expand) {
                if (!string.IsNullOrEmpty(title)) {
                    EditorGUI.indentLevel += indentLevel;
                }
                for (int i=0; i< Length; i++) {
                    EditorGUILayout.BeginHorizontal();
                    changed |= DrawInspectorGUI(i);
                    if (resizable) {
                        if (Length > minLength && GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
                            changed = true;
                        } else {
                            list.Add(this[i]);
                        }
                        if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
                            list.Add(Clone(this[i]));
                            changed = true;
                        }
                    } else {
                        list.Add(this[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                    for (int j=0; j<separatorCount; j++) {
                        EditorGUILayout.Separator();
                    }
                }
                
                if (changed) {
                    Arr = list.ToArray();
                    Array.Resize(ref fold, Length);
                    Invalidate();
                }
                if (!string.IsNullOrEmpty(title)) {
                    EditorGUI.indentLevel -= indentLevel;
                }
            }
            return changed;
        }
        
        public void Add(T t) {
            int size = Length+1;
            Length = size;
            Arr[size-1] = t;
            Invalidate();
        }
        
        private void Invalidate() {
            if (obj is UnityEngine.Object) {
                CompatibilityEditor.SetDirty((UnityEngine.Object)obj);
            }
        }
        
        public void Set(T[] arr) {
            Arr = arr;
        }
        
        #region IEnumerable implementation
        IEnumerator<T> IEnumerable<T>.GetEnumerator ()
        {
            return new ArrayEnumerator<T>(arr);
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ArrayEnumerator<T>(arr);
        }
        #endregion
    }
}
