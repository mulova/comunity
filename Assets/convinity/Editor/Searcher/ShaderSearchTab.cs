using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using commons;
using comunity;
using UnityEditor.SceneManagement;
using System;
using System.Globalization;

namespace convinity {
	class ShaderSearchTab : SearchTab<ShaderSearchItem> {
		
		public ShaderSearchTab(TabbedEditorWindow window) : base("Shader", window) {
		}

		public override void OnSelected(bool sel) {}
		public override void OnFocus(bool focus) {}

		
		public override void OnHeaderGUI(List<ShaderSearchItem> found) {
			EditorGUILayout.BeginHorizontal();
			EditorGUIUtil.TextField(null, ref shaderName);
			if (GUILayout.Button("Search")) {
				Search();
			}
			EditorGUILayout.EndHorizontal();
		}
		
		protected override List<ShaderSearchItem> SearchResource() {
			List<ShaderSearchItem> list = new List<ShaderSearchItem>();
			foreach (var root in roots)
			{
				if (root is GameObject)
				{
					var rends = (root as GameObject).GetComponentsInChildren<Renderer>(true);
					foreach (var r in rends)
					{
						AddMatch(r, list);
					}
				} else
				{
					foreach (Object o in SearchAssets(typeof(Material), FileType.Prefab, FileType.Material)) {
						Material mat = o as Material;
						if (mat != null && mat.shader.name.ContainsIgnoreCase(shaderName)) {
							list.Add(new ShaderSearchItem(null, mat));
						}
					}
					foreach (Object o in SearchAssets(typeof(Renderer), FileType.Prefab, FileType.Material)) {
						Renderer r = o as Renderer;
						AddMatch(r, list);
					}
				}
			}
			list.Sort();
			return list;
		}

        protected override void SelectGameObjects(List<ShaderSearchItem> found)
        {
            List<Object> list = new List<Object>();
            foreach (var i in found)
            {
                GameObject o = (GameObject)i;
                if (o != null)
                {
                    list.Add(o);
                }
            }

            Selection.objects = list.ToArray();
        }

        protected override void SelectObjects(List<ShaderSearchItem> found)
        {
            List<Object> list = found.ConvertAll<Object>(i=> i.material);
            Selection.objects = list.ToArray();
        }

        private bool IsMatch(Renderer r)
        {
			if (r == null || r.sharedMaterials.Length == 0)
			{
				return false;
			}
			foreach (var m in r.sharedMaterials)
			{
				if (m != null && m.shader != null && m.shader.name.Contains(shaderName))
				{
					return true;
				}
			}
			return false;
        }

		private void AddMatch(Renderer r, List<ShaderSearchItem> store)
		{
			if (r == null || r.sharedMaterials.Length == 0)
			{
				return;
			}
			foreach (var m in r.sharedMaterials)
			{
				if (m != null && m.shader != null && m.shader.name.ContainsIgnoreCase(shaderName))
				{
					store.Add(new ShaderSearchItem(r, m));
				}
			}
		}

		protected override void OnInspectorGUI(List<ShaderSearchItem> found) {
			var drawer = new ShaderSearchReorderList(found);
			drawer.Draw();
		}
		
		public override void OnFooterGUI(List<ShaderSearchItem> found) {
		}
		
		private bool showShader;
		private string shaderName;

		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
	}
	
}
