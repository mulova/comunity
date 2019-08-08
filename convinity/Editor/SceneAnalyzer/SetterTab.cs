//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Text;
using System.Reflection;
using mulova.commons;
using comunity;

namespace convinity {
	class SetterTab : EditorTab {
		
		public SetterTab(TabbedEditorWindow window) : base("Setter", window) {
		}
		
		public override void OnEnable() {
		}
		
		public override void OnDisable() {}
		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		
		private Transform root;
		private FieldInfo field;
		private PropertyInfo property;
		private int intVal;
		private bool boolVal;
		private Vector2 vec2Val;
		private Vector3 vec3Val;
		private Vector4 vec4Val;
		private Color colorVal;
		private string strVal;
		private Enum enumVal = default(Enum);
		private Object objVal;
		private object val;
        private MultiMap<Type, Component> targets = new MultiMap<Type, Component>();
		private FieldInspector fieldInspector = new FieldInspector();
		private TypeSelector typeSelector = new TypeSelector(typeof(Object));

		private bool applyOnSelection;
		public override void OnHeaderGUI() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			bool changed = false;
			changed |= EditorGUIUtil.Toggle("Apply On Selection", ref applyOnSelection);
			if (!applyOnSelection) {
				changed |= EditorGUIUtil.ObjectField<Transform>("Root", ref root, true);
			}
			changed |= typeSelector.DrawSelector();
			Type type = typeSelector.GetSelected();
			if (typeSelector.GetType() != null) {
				if (fieldInspector.DrawFieldPopup(type, ref field)) {
					changed = true;
					property = null;
				}
				if (fieldInspector.DrawPropertyPopup(type, ref property)) {
					changed = true;
					field = null;
				}
			}
			EditorGUILayout.EndVertical();
			if (type != null && changed) {
				if (applyOnSelection) {
					if (Selection.gameObjects.Length > 0) {
						targets.Clear();
						foreach (GameObject o in Selection.gameObjects) {
							foreach (Component c in o.GetComponentsInChildren(type, true)) {
								targets.Add(type, c);
							}
						}
					} else if (Selection.objects.Length > 0) {
						targets.Clear();
						foreach (Object o in Selection.objects) {
							if (o is Component) {
								targets.Add(o.GetType(), o as Component);
							}
						}
					}
				} else if (!applyOnSelection && root != null){
					targets.Clear();
					foreach (Component c in root.GetComponentsInChildren(type, true)) {
						targets.Add(type, c);
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI() {
			foreach (Type type in targets.Keys) {
				EditorGUILayout.TextField(type.FullName, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(false));
				EditorGUIUtil.ObjectFieldReorderList(targets[type]);
			}
		}
		
		public override void OnFooterGUI() {
			EditorGUILayout.BeginHorizontal();
			Type fieldType = null;
			if (field != null) {
				fieldType = field.FieldType;
			} else if (property != null) {
				fieldType = property.PropertyType;
			}

			if (fieldType != null) {
				if (fieldType == typeof(int)) {
					EditorGUIUtil.IntField("Value", ref intVal);
					val = intVal;
				} else if (fieldType == typeof(bool)) {
					EditorGUIUtil.Toggle("Value", ref boolVal);
					val = boolVal;
				} else if (fieldType == typeof(Vector2)) {
					EditorGUIUtil.Vector2Field("Value", ref vec2Val);
					val = vec2Val;
				} else if (fieldType == typeof(Vector3)) {
					EditorGUIUtil.Vector3Field("Value", ref vec3Val);
					val = vec3Val;
				} else if (fieldType == typeof(Vector4)) {
					EditorGUIUtil.Vector4Field("Value", ref vec4Val);
					val = vec4Val;
				} else if (fieldType == typeof(Color)) {
					EditorGUIUtil.ColorField("Value", ref colorVal);
					val = colorVal;
				} else if (fieldType == typeof(string)) {
					EditorGUIUtil.TextField("Value", ref strVal);
					val = strVal;
				} else if (fieldType.IsEnum) {
					EditorGUIUtil.PopupEnum(fieldType, "Value", ref enumVal);
					val = enumVal;
				} else {
					EditorGUIUtil.ObjectField<Object>("Value", ref objVal, true);
					val = objVal;
					if (objVal != null) {
						if (objVal is GameObject && !fieldType.IsInstanceOfType(val)) {
							Component c = (objVal as GameObject).GetComponent(fieldType);
							if (c != null) {
								val = c;
							}
						}
					}
				}
			}
			GUI.enabled = (applyOnSelection || root != null) && fieldType != null;
			if (GUILayout.Button("Allocate")) {
				Allocate();
			}
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
		}

		private void Allocate() {
			foreach (Type t in targets.Keys) {
				foreach (Component c in targets[t]) {
					if (field != null) {
						field.SetValue(c, val);
					} else if (property != null) {
						property.SetValue(c, val, null);
					}
					EditorUtil.SetDirty(c.gameObject);
				}
			}
		}

		public override void OnFocus(bool focus) {}
		public override void OnSelected(bool sel) {}
	}
}