//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Collections.Generic;
using mulova.unicore;

namespace mulova.comunity
{
	
	/**
	* - title이 null이면 folding을 하지 않는다.
	*/
	public class ArrayInspector {
		
		public readonly ArrayInspectorRow rows;
		private bool showArray = true;
		private Object obj;
		public string title;
		
		public int MinLength {
			get { return rows.MinLength; }
			set { rows.MinLength = value; }
		}
		
		public int Length {
			get { return rows.Length; }
			set { rows.Length = value; }
		}
		
		
		/**
		* variable 이 null이면 안된다.
		* @param make
		*/
		public ArrayInspector(Object obj, string title, ArrayInspectorRow item)
		{
			this.obj = obj;
			this.title = title;
			this.rows = item;
		}
		
		public object GetValue(int col, int index) {
			return rows.GetRow(index)[col];
		}
		
		public void AddRow(params object[] row) {
			rows.AddRow(row);
		}
		
		public void Set(List<object[]> row) {
			rows.Set(row);
			EditorUtil.SetDirty(obj);
		}
		
		public void SetPreset(params object[] preset) {
			rows.SetPreset(preset);
		}
		
		private List<object[]> list = new List<object[]>();
		public virtual bool OnInspectorGUI() {
			Undo.RecordObject(obj, "Array Change");
			bool changed = false;
			list.Clear();
			EditorGUILayout.BeginHorizontal();
			if (!string.IsNullOrEmpty(title)) {
				showArray = EditorGUILayout.Foldout(showArray, title);
			}
			if (Length == 0) {
				if (GUILayout.Button("+", GUILayout.ExpandWidth(false))) {
					list.Add(rows.GetDefault());
					changed = true;
					showArray = true;
				}
			}
			EditorGUILayout.EndHorizontal();
			if (showArray) {
				if (!string.IsNullOrEmpty(title)) {
					EditorGUI.indentLevel += 2;
				}
				for (int i=0; i< Length; i++) {
					EditorGUILayout.BeginHorizontal();
					changed |= rows.OnInspectorGUI(i);
					if (Length > MinLength && GUILayout.Button("-", GUILayout.ExpandWidth(false))) {
						changed = true;
					} else {
						list.Add(rows.GetRow(i));
					}
					if (GUILayout.Button("+", GUILayout.ExpandWidth(false))) {
						list.Add(rows.GetRow(i));
						changed = true;
					}
					EditorGUILayout.EndHorizontal();
				}
				if (!string.IsNullOrEmpty(title)) {
					EditorGUI.indentLevel -= 2;
				}
			}
			if (changed) {
				rows.Set(list);
				EditorUtil.SetDirty(obj);
			} else {
				Undo.ClearUndo(obj);
			}
			return changed;
		}
	}
	
}