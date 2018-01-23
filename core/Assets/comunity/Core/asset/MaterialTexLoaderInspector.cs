using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


using System.IO;
using commons;

namespace core {
	[CustomEditor(typeof(MaterialTexLoader))]
	public class MaterialTexLoaderInspector : Editor {
		
		private MaterialTexLoader loader;
		
		void OnEnable() {
			loader = target as MaterialTexLoader;
		}
		
		public override void OnInspectorGUI() {
			DrawDefaultInspector ();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Set")) {
				MaterialTexLoaderBuildProcessor.SetMatTexture(loader);
			}
			if (GUILayout.Button("Clear")) {
				MaterialTexLoaderBuildProcessor.ClearMatTexture(loader);
			}
			if (GUILayout.Button("Scan")) {
				EditorAssetUtil.ScanFolder("MaterialTexLoader_ScanFolder_"+loader.name, FileType.Material, files=> {
					if (loader.materials == null) {
						loader.materials = new MaterialTexData[0];
					}
					foreach (FileInfo f in files) {
						string path = EditorAssetUtil.GetProjectRelativePath(f.FullName);
						MaterialTexData data = new MaterialTexData();
						data.material = AssetDatabase.LoadAssetAtPath<Material>(path);
						if (data.material != null) {
							if (data.material.HasProperty(MaterialTexLoader.TEX1)) {
								Texture tex1 = data.material.GetTexture(MaterialTexLoader.TEX1);
								if (tex1 != null) {
									data.tex1.SetPath(tex1);
								}
							}
							if (data.material.HasProperty(MaterialTexLoader.TEX2)) {
								Texture tex2 = data.material.GetTexture(MaterialTexLoader.TEX2);
								if (tex2 != null) {
									data.tex2.SetPath(tex2);
								}
							}
							ArrayUtil.Add(ref loader.materials, data);
						}
					}
					CompatibilityEditor.SetDirty(loader);
				});
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
