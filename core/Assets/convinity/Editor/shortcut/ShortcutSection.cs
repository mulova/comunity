using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using commons;
using comunity;

namespace convinity {
	[System.Serializable]
	public class ShortcutSection {
		public static readonly string ASSET_EXT = ".asc";
		public static readonly string SCENE_EXT = ".ssc";
		
		public string name;
		public string dir;
		[System.NonSerialized] private List<Object> assetRefs = new List<Object>();
		[System.NonSerialized] private List<Object> sceneRefs = new List<Object>(); // current scene's object refs
		[System.NonSerialized] private Dictionary<string, List<string>> sceneObjPaths = new Dictionary<string, List<string>>();
		[System.NonSerialized] private string cacheName;
		[System.NonSerialized] public Vector2 scroll = new Vector2();
		
		public ShortcutSection() {}
		
		public ShortcutSection(string name, string dir) {
			this.name = name;
			this.dir = "Assets/../"+dir;
		}

		public List<Object> GetAssetRefs() {
			return assetRefs;
		}

		public List<Object> GetSceneRefs() {
			return sceneRefs;
		}

		public List<Object> GetSceneObjects(string sceneName) {
			if (cacheName != sceneName) {
				cacheName = sceneName;
				sceneRefs.Clear();
				List<string> list = LoadSceneObjList(sceneName);
				foreach (string s in list) {
					sceneRefs.Add(EditorAssetUtil.GetObject(s));
				}
			}
			return sceneRefs;
		}
		
		public void ClearCache() {
			cacheName = null;
			sceneRefs.Clear();
		}
		
		public void Load() {
			assetRefs = EditorAssetUtil.LoadReferencesFromFile<Object>(GetAssetStore());
			LoadSceneObjList(EditorAssetUtil.GetCurrentScene());
		}
		
		public List<string> LoadSceneObjList(string sceneName) {
			List<string> list = null;
			if (!sceneObjPaths.TryGetValue(sceneName, out list)) {
                string path = GetSceneStore(sceneName);
                if (File.Exists(path))
                {
                    list = SerializationUtil.ReadObject<List<string>>(path);
                    sceneObjPaths[sceneName] = list;
                }
			}
			if (list == null) {
				list = new List<string>();
				sceneObjPaths[sceneName] = list;
			}
			return list;
		}
		
		/// <summary>
		/// Save current scene references to file
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		public void Save(string sceneName) {
			// save asset objects
			EditorAssetUtil.SaveReferences(GetAssetStore(), assetRefs);
			// save scene objects
			List<Object> list = GetSceneObjects(sceneName);
			if (list != null) {
				string path = GetSceneStore(sceneName);
				if (list.Count > 0) {
					List<string> refs = EditorAssetUtil.SaveReferences(path, list);
					sceneObjPaths[sceneName] = refs;
				} else {
					if (File.Exists(path)) {
						File.Delete(path);
					}
					sceneObjPaths.Remove(sceneName);
				}
			}
		}
		
		public void Delete() {
			string path = GetAssetStore();
			if (File.Exists(path)) {
				File.Delete(path);
			}
			
			FileInfo[] files = AssetUtil.ListFiles(dir, name + "_*"+SCENE_EXT);
			foreach (FileInfo f in files) {
				f.Delete();
			}
		}
		
		public bool AddObject(string sceneName, Object o) {
			if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(o))) {
				if (!sceneRefs.Contains(o)) {
					List<string> list = null;
					if (!sceneObjPaths.TryGetValue(sceneName, out list)) {
						list = new List<string>();
						sceneObjPaths[sceneName] = list;
					}
					sceneRefs.Add(o);
					string objPath = null;
					if (o as GameObject) {
						objPath = (o as GameObject).transform.GetScenePath();
					} else if (o as Component) {
						objPath = (o as Component).transform.GetScenePath();
					}
					if (!list.Contains(objPath)) {
						list.Add(objPath);
						return true;
					}
				}
				return false;
			} else {
				if (!assetRefs.Contains(o)) {
					assetRefs.Add(o);
					return true;
				}
				return false;
			}
		}
		
		private string GetAssetStore() {
			return PathUtil.Combine(dir, name + ASSET_EXT);
		}
		
		private string GetSceneStore(string sceneName) {
			return PathUtil.Combine(dir, name + "_" + sceneName + SCENE_EXT);
		}
		
		public override string ToString() {
			return name;
		}
	}
	
	[System.Serializable]
	public class ShortcutWindowInfo {
		public int columnCount = 1;
		public List<string> sections = new List<string>();
		
		public bool IsEmpty() {
			return sections.Count == 0;
		}
		
		public void Set(List<ShortcutSection> list) {
			sections.Clear();
			foreach (ShortcutSection s in list) {
				sections.Add(s.name);
			}
		}
	}
}
