//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System;
using System.Collections.Generic;
using mulova.commons;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.comunity
{
    /**
    * Enum index를 기반으로 하는 배열에 사용되는 Inspector 확장
    * T: Enum type
    * D: Serializable Data type
    */
    public abstract class EnumArrayInspector<T, D> where T: struct, IComparable, IConvertible, IFormattable
    {
        protected bool[] fold;
        public D[] data;
        public string name;
        public int indent = 2;
        protected Object target;
        private bool foldAll;
        private Dictionary<T, T> excluded = new Dictionary<T, T>();
        private T selected;
        protected bool toggleAll = true; /*  모든 element를 다 보여줄것이면 true, 하나만 선택해서 보여줄 것이면 false */
        private Dictionary<T, int> index = new Dictionary<T, int>();
        
        /**
        * @param target SetDirty()를 호출할 target
        */
        public EnumArrayInspector(ref D[] array, Object target)
        {
            if (ArrayUtil.ResizeEnumIndexedArray<T, D>(ref array)) {
                EditorUtil.SetDirty(target);
            }
            this.target = target;
            this.data = array;
            this.name = typeof(T).Name;
            SetFolding();
            
            T[] arr = (T[])Enum.GetValues(typeof(T));
            for (int i=0; i<arr.Length; i++) {
                index[arr[i]] = i;
            }
        }
        
        public int Length {
            get {
                return data.Length;
            }
        }
        
        public D this[int index] {
            get {
                return data[index];
            }
        }
        
        private void SetFolding() {
            this.fold = new bool[this.data.Length];
            for(int i=0; i< fold.Length; i++) {
                fold[i] = IsExpanded(data[i]);
                foldAll |= fold[i];
            }
        }
        
        public void SetShowAll(bool all) {
            this.toggleAll = all;
        }
        
        /**
        * @param expand if true, all the items are expanded. if false, all items are folded.
        */
        public void SetExpanded(bool expand) {
            for(int i=0; i< this.fold.Length; i++) {
                this.fold[i] = expand;
            }
        }
        
        public void ClearExcludes() {
            excluded.Clear();
        }
        
        /**
        * 특정 Event는 Display하지 않는다.
        */ 
        public void Exclude(params T[] exclude) {
            foreach (T t in exclude) {
                if (!excluded.ContainsKey(t)) {
                    excluded.Add(t, t);
                }
            }
        }
        
        /**
        * @return true if value is changed
        */
        public bool OnInspectorGUI()
        {
            bool changed = false;
            if (!string.IsNullOrEmpty(name)) {
                EditorGUILayout.BeginHorizontal();
                OnTitleGUI();
                EditorGUILayout.EndHorizontal();
            }
            if (toggleAll) {
                if (foldAll) {
                    for (int i = 0; i < data.Length; i++) {
                        T evt = EnumUtil.Parse<T>(data[i].ToString(), default(T));
                        if (excluded.ContainsKey(evt) == false) {
                            changed |= OnInspectorGUI(evt, i);
                        }
                    }
                }
            } else {
                if (foldAll) {
                    T[] arr = (T[])Enum.GetValues(typeof(T));
                    List<T> list = new List<T>();
                    foreach (T t in arr) {
                        if (excluded.ContainsKey(t) == false) {
                            list.Add(t);
                        }
                    }
                    EditorGUI.indentLevel += indent;
                    changed |= EditorGUILayoutUtil.Popup<T>(ref selected, list.ToArray());
                    EditorGUI.indentLevel -= indent;
                    changed |= OnInspectorGUI(selected, index[selected]);
                }
            }
            if (changed) {
                EditorUtil.SetDirty(target);
            }
            return changed;
        }
        
        protected virtual bool OnInspectorGUI(T evt, int i) {
            bool changed = false;
            EditorGUI.indentLevel += indent;
            EditorGUILayout.BeginHorizontal();
            changed |= OnHeaderGUI(evt, i, data[i]);
            EditorGUILayout.EndHorizontal();
            if (fold[i]) {
                EditorGUI.indentLevel += indent;
                changed |= OnInspectorGUI(evt, data[i]);
                EditorGUI.indentLevel -= indent;
            }
            EditorGUI.indentLevel -= indent;
            return changed;
        }
        
        /**
        * @return bool true if value is changed.
        */
        protected virtual bool OnHeaderGUI(T evt, int i, D entry) {
            if (toggleAll) {
                fold[i] = EditorGUILayout.Foldout(fold[i], evt.ToString());
            }
            return false;
        }
        
        protected virtual void OnTitleGUI() {
            foldAll = EditorGUILayout.Foldout(foldAll, name, EditorStyles.foldoutPreDrop);
            toggleAll = EditorGUILayout.Toggle(toggleAll, GUILayout.ExpandWidth(false));
        }
        
        /**
        * D 개체의  Editor Inspector UI를 확장한다.
        */
        protected abstract bool OnInspectorGUI(T evt, D data);
        
        /**
        * @return bool 초기화시 item 을 펼칠것인지 여부 
        */
        protected abstract bool IsExpanded(D data);
    }
}
