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
        [System.NonSerialized] private UnityObjList assetRefs = new UnityObjList();
        [System.NonSerialized] private UnityObjList sceneRefs = new UnityObjList(); // current scene's object refs
        [System.NonSerialized] private Dictionary<string, UnityObjList> sceneObjPaths = new Dictionary<string, UnityObjList>();
		[System.NonSerialized] private string cacheName;
		[System.NonSerialized] public Vector2 scroll = new Vector2();
		
		public ShortcutSection() {}
		
		public ShortcutSection(string name, string dir) {
			this.name = name;
			this.dir = "Assets/../"+dir;
		}

        public UnityObjList GetAssetRefs() {
			return assetRefs;
		}

        public UnityObjList GetSceneRefs() {
			return sceneRefs;
		}

        public UnityObjList GetSceneObjects(string sceneName) {
			if (cacheName != sceneName) {
				cacheName = sceneName;
				sceneRefs.Clear();
                sceneRefs = LoadSceneObjList(sceneName);
			}
			return sceneRefs;
		}
		
		public void ClearCache() {
			cacheName = null;
			sceneRefs.Clear();
		}

		public void Load() {
            assetRefs = UnityObjList.Load(GetAssetStore());
			LoadSceneObjList(EditorAssetUtil.GetCurrentScene());
		}
		
        public UnityObjList LoadSceneObjList(string sceneName) {
            var list = UnityObjList.Load(GetSceneStore(sceneName));
            sceneObjPaths[sceneName] = list;
			return list;
		}
		
		/// <summary>
		/// Save current scene references to file
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		public void Save(string sceneName) {
			// save asset objects
            assetRefs.Save(GetAssetStore());
			// save scene objects
            UnityObjList list = GetSceneObjects(sceneName);
			if (list != null) {
				string path = GetSceneStore(sceneName);
				if (list.Count > 0) {
                    list.Save(GetSceneStore(sceneName));
                    sceneObjPaths[sceneName] = list;
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
                    UnityObjList list = null;
					if (!sceneObjPaths.TryGetValue(sceneName, out list)) {
                        list = new UnityObjList();
						sceneObjPaths[sceneName] = list;
					}
					sceneRefs.Add(o);
					if (!list.Contains(o)) {
						list.Add(o);
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
