using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


using System.IO;
using commons;

namespace comunity {
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
				SetMatTexture(loader);
			}
			if (GUILayout.Button("Clear")) {
				ClearMatTexture(loader);
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

        public static void SetMatTexture(MaterialTexLoader l)
        {
            foreach (MaterialTexData d in l.materials)
            {
                d.material.SetTexture(MaterialTexLoader.TEX1, AssetDatabase.LoadAssetAtPath<Texture>(d.tex1.GetEditorPath()));
                d.material.SetTexture(MaterialTexLoader.TEX2, AssetDatabase.LoadAssetAtPath<Texture>(d.tex2.GetEditorPath()));
                CompatibilityEditor.SetDirty(d.material);
            }
        }

        public static void ClearMatTexture(MaterialTexLoader l)
        {
            foreach (MaterialTexData d in l.materials)
            {
                d.material.SetTexture(MaterialTexLoader.TEX1, null);
                d.material.SetTexture(MaterialTexLoader.TEX2, null);
                CompatibilityEditor.SetDirty(d.material);
            }
        }
	}
}
