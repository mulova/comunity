#if FULL
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace mulova.comunity
{
	
	[CustomEditor(typeof(NestedPrefab))]
	public class NestedPrefabInspector : Editor {
		private static Object folder;
		private NestedPrefab nestedPrefab;
		
		void OnEnable() {
			nestedPrefab = target as NestedPrefab;
		}
		
		public override void OnInspectorGUI() {
			if (nestedPrefab.GetPrefab() != null) {
				EditorGUILayout.ObjectField(nestedPrefab.GetPrefab(), typeof(GameObject), false);
			}
			if (AssetDatabase.IsMainAsset(nestedPrefab.gameObject) || AssetDatabase.IsSubAsset(nestedPrefab.gameObject)) {
				return;
			}
			if (nestedPrefab.IsLinked()) {
				if (GUILayout.Button("Unlink Prefabs")) {
					PrefabRestore restore = new PrefabRestore();
					nestedPrefab.transform.DepthFirstTraversal(restore.Apply);
					restore.Destroy();
				}
			} else {
				if (GUILayout.Button("Link Prefabs")) {
					Build();
				}
			}
			GameObject rootPrefab = PrefabUtility.GetPrefabParent(nestedPrefab.gameObject) as GameObject;
			EditorGUILayout.BeginHorizontal();
			EditorGUIUtil.ObjectField<Object>("Folder", ref folder, false);
			GUI.enabled = folder != null;
			if (GUILayout.Button("Create Prefab")) {
				if (!nestedPrefab.IsLinked()) {
					Build();
				}
				string dir = EditorAssetUtil.GetAssetFileFullPath(AssetDatabase.GetAssetPath(folder));
				if (!Directory.Exists(dir)) {
					Directory.CreateDirectory(dir);
					AssetDatabase.Refresh(ImportAssetOptions.Default);
				}
				
				string path = AssetDatabase.GenerateUniqueAssetPath(PathUtil.Combine(AssetDatabase.GetAssetPath(folder), nestedPrefab.name+".prefab"));
				rootPrefab = PrefabUtility.CreatePrefab(path, nestedPrefab.gameObject);
				Selection.activeGameObject = rootPrefab;
			}
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
			if (rootPrefab != null) {
				if (GUILayout.Button("Disconnect Prefab")) {
					PrefabUtility.DisconnectPrefabInstance(nestedPrefab.gameObject);
				}
			}
			if (EditorGUIUtil.Toggle("Instantiate On Start", ref nestedPrefab.instantiateOnStart)) {
				EditorUtil.SetDirty(nestedPrefab);
			}
			if (EditorGUIUtil.Toggle("Destroy On Instantiation", ref nestedPrefab.destroyOnInstantiation)) {
				EditorUtil.SetDirty(nestedPrefab);
			}
		}
		
		private void Build() {
			PrefabLinker linker = new PrefabLinker();
			nestedPrefab.transform.BreadthFirstTraversal(linker.Apply);
			linker.Destroy();
		}
		
		class PrefabLinker {
			List<GameObject> destroy = new List<GameObject>();
			HashSet<GameObject> prefabSet = new HashSet<GameObject>();
			
			public bool Apply(Transform src)
			{
				GameObject prefab = PrefabUtility.GetPrefabParent(src.gameObject) as GameObject;
				if (prefab != null && PrefabUtility.GetPrefabType(src.gameObject) == PrefabType.PrefabInstance && !prefabSet.Contains(prefab)) {
					prefabSet.Add(prefab);
					GameObject newObj = new GameObject(src.name);
					Transform dst = newObj.transform;
					dst.parent = src.parent;
					dst.localPosition = src.localPosition;
					dst.localScale = src.localScale;
					dst.localRotation = src.localRotation;
					NestedPrefab s = newObj.AddComponent<NestedPrefab>();
					s.Link(prefab);
					destroy.Add(src.gameObject);
					Debug.Log(AssetDatabase.GetAssetPath(prefab));
				} else {
					NestedPrefab s = src.gameObject.GetComponent<NestedPrefab>();
					if (s != null && !s.IsLinked()) {
						s.Link(null);
					}
				}
				return true;
			}
			
			public void Destroy() {
				foreach (GameObject go in destroy) {
					Object.DestroyImmediate(go);
				}
			}
		}
		
		class PrefabRestore {
			List<GameObject> destroy = new List<GameObject>();
			
			public bool Apply(Transform target)
			{
				NestedPrefab s = target.GetComponent<NestedPrefab>();
				if (s != null) {
					if (s.IsLinked() && s.GetPrefab() != null) {
						GameObject instance = PrefabUtility.InstantiatePrefab(s.GetPrefab()) as GameObject;
						Transform src = s.transform;
						Transform dst = instance.transform;
						dst.parent = s.transform.parent;
						dst.localPosition = src.localPosition;
						dst.localScale = src.localScale;
						dst.localRotation = src.localRotation;
						destroy.Add(s.gameObject);
						Debug.Log(AssetDatabase.GetAssetPath(s.GetPrefab()));
					}
					s.ClearLink();
				}
				return true;
			}
			
			public void Destroy() {
				foreach (GameObject go in destroy) {
					Object.DestroyImmediate(go);
				}
			}
		}
	}
}
#endif