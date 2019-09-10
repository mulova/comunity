using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using mulova.commons;
using System.Text.Ex;

namespace mulova.comunity
{
	/// <summary>
	/// Editor 성능을 위해 가능하면 AssetDatabase를 사용하지 않고, Cache에서 가져온다.
	/// </summary>
	public class EditorAssetCache
	{
		private Dictionary<string, Object> cache = new Dictionary<string, Object>();
		/// <summary>
		/// Gets the cache.
		/// </summary>
		/// <returns>The cache.</returns>
		/// <param name="path">Path.</param>
		public Object GetCache(string path) {
			if (path.IsEmpty()) {
				return null;
			}
			Object obj = null;
			if (cache.TryGetValue(path, out obj)) {
				return obj;
			}
			string key = path;
			if (!key.StartsWith("Assets/")) {
				key = "Assets/"+key;
			}
			obj = AssetDatabase.LoadAssetAtPath(key, typeof(Object));
			if (obj != null) {
				cache[path] = obj;
			}
			return obj;
		}

		public bool OnInspectorGUI<T>(ref string srcPath) where T:Object{
			Object oldAsset = GetCache(srcPath);
			Object newAsset = EditorGUILayout.ObjectField (oldAsset, typeof(T), false);
			if (oldAsset != newAsset) {
				if (newAsset == null) {
					srcPath = null;
				} else {
					srcPath = EditorAssetUtil.GetAssetRelativePath(newAsset);
				}
				return true;
			}
			return false;
		}

		public void Clear() {
			cache.Clear();
		}
	}
}

