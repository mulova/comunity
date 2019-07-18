using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using commons;
using System.Text.Ex;

namespace comunity {
	public class ObjListFilter<T> where T:class {

		private string title;
		private bool asset;
		private bool[] show;
		private Predicate<T>[] predicates;
		private ToString toString;

		/// <summary>
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="asset"><c>true</c> If target objects are asset types, false if scene objects</param>
		public ObjListFilter(string title, bool asset, ToString toString, params Predicate<T>[] predicates) {
			this.title = title;
			this.asset = asset;
			this.toString = toString!=null? toString: ObjToString.ScenePathToString;
			this.predicates = predicates;
			this.show = new bool[predicates.Length];
		}

		private static Dictionary<object, bool> bools = new Dictionary<object, bool>();
		private string filter;
		private FileType fileType = FileType.All;
		public AndPredicate<T> GetPredicate(List<T> list)  {
			Color oldColor = GUI.color;
			GUI.color = Color.cyan;
			AndPredicate<T> predicate = new AndPredicate<T>();
			EditorGUILayout.BeginHorizontal();
			bool selected = GUILayout.Button(title, EditorStyles.toolbarButton, GUILayout.MaxWidth(100));
			EditorGUIUtil.TextField(null, ref filter);
			if (!filter.IsEmpty()) {
				predicate.AddPredicate(new ToStringFilter(toString, filter).Filter<T>);
			}
			if (asset) {
				EditorGUIUtil.PopupEnum(null, ref fileType);
			}
			EditorGUILayout.EndHorizontal();
			for (int i=0; i<predicates.Length; ++i) {
				Predicate<T> p = predicates[i];
				bool b = false;
				if (!bools.TryGetValue(p, out b)) {
					bools[p] = b;
				}
				show[i] = EditorGUILayout.Toggle(p.Method.Name, show[i]);
				if (bools[p]) {
					predicate.AddPredicate(p);
				}
			}
			predicate.AddPredicate(new FileTypeFilter(fileType).Filter<T>);

			if (selected) {
				List<Object> filtered = new List<Object>();
				foreach (T o in list) {
					if (o is Object && predicate.Accept(o)) {
						filtered.Add(o as Object);
					}
				}
				Selection.objects = filtered.ToArray();
			}

			GUI.color = oldColor;

			return predicate;
		}
	}
}


