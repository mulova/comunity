using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System.Collections.Generic;
using mulova.commons;
using System.Text.Ex;
using System.Ex;

namespace comunity
{
	public class EditorAssetLoader : IAssetLoader
	{
		public Func<string, Type, Object> loadAssetFunc;
        private Transform hiddenRoot;

		public EditorAssetLoader()
		{
			this.loadAssetFunc = UnityEditor.AssetDatabase.LoadAssetAtPath;
		}

		public EditorAssetLoader(Func<string, Type, Object> loadFunc)
		{
			this.loadAssetFunc = loadFunc;
		}

		private string ToLocalPath(string url)
		{
			return PathUtil.Combine(Application.dataPath, Cdn.ToRelativePath(url));
		}

		private T LoadEditorAsset<T>(string url) where T:Object
		{
			if (loadAssetFunc == null)
			{
				Type t = ReflectionUtil.GetType("UnityEditor.AssetDatabase");
				System.Reflection.MethodInfo m = t.GetMethod("LoadAssetAtPath", new Type[]{ typeof(string), typeof(Type) });
				loadAssetFunc = (Func<string, Type, Object>)Delegate.CreateDelegate(t, m);
			}
			T asset = loadAssetFunc(GetEditorAssetPath<T>(url), typeof(T)) as T;
            if (asset == null)
            {
                asset = Resources.Load<T>(url);
            }
            // in order not to affect existing asset (like adding component, etc), clone it
//            if (asset != null)
//            {
//                if (hiddenRoot == null)
//                {
//                    GameObject o = new GameObject("_editor_asset");
//                    o.hideFlags = HideFlags.HideAndDontSave;
//                    o.SetActive(false);
//                    hiddenRoot = o.transform;
//                }
//                
//                asset = asset.InstantiateEx(hiddenRoot, false);
//            }
            return asset;
		}

		private string GetEditorAssetPath<T>(string url) where T: Object
		{
			if (url.IsEmpty())
			{
				return string.Empty;
			}
			string u = url.ToUnixPath();
			if (u.StartsWith(Application.dataPath))
			{
				u = url.Substring(Application.dataPath.Length);
			} else
			{
				u = Cdn.ToRelativePath(u);
			}

			if (typeof(T) == typeof(Object))
			{
				u = FindAsset(u, ".prefab", ".asset");
			} else if (typeof(T).IsAssignableFrom(typeof(Texture)))
			{
				u = FindAsset(u, FileType.Image.GetExt());
			} else if (typeof(T).IsAssignableFrom(typeof(GameObject)))
			{
				u = FindAsset(u, FileType.Prefab.GetExt());
			} else if (typeof(T).IsAssignableFrom(typeof(Animation)))
			{
				u = FindAsset(u, FileType.Anim.GetExt());
			} else if (typeof(T).IsAssignableFrom(typeof(Material)))
			{
				u = FindAsset(u, FileType.Material.GetExt());
			} else if (typeof(T).IsAssignableFrom(typeof(TextAsset)))
			{
				u = FindAsset(u, FileType.Text.GetExt());
			} else if (typeof(T).IsAssignableFrom(typeof(AudioClip)))
			{
				u = FindAsset(u, FileType.Audio.GetExt());
            } else if (typeof(T).IsAssignableFrom(typeof(AssetBundle)))
            {
                u = FindAsset(u, FileTypeEx.ASSET_BUNDLE);
			} else
			{
				// Error Handling
				u = FindAsset(u, ".anim", ".prefab", ".mat", ".png", ".jpg", ".tga", ".bmp", ".bytes", ".txt", ".csv", ".asset");
			}
			return "Assets/"+u;
		}

		public void GetAsset<T>(string url, Action<T> callback, bool asyncHint) where T: Object
		{
			T asset = LoadEditorAsset<T>(url);
			if (asset != null)
			{
				callback.Call(asset);
			} else
			{
				fallback.GetAsset<T>(url, callback, asyncHint);
			}
		}

		public void GetAssets<T>(string url, Action<IEnumerable<T>> callback, bool asyncHint) where T: Object
		{
			GetAsset<T>(url, t => callback.Call(new T[] { t }), asyncHint);
		}

		/// <summary>
		/// Finds asset for the given extensions.
		/// </summary>
		/// <returns>The asset path</returns>
		/// <param name="url">URL.</param>
		/// <param name="extensions">Extensions.</param>
		private string FindAsset(string relativeUrl, params string[] extensions)
		{
			foreach (string ext in extensions)
			{
				string path = PathUtil.ReplaceExtension(relativeUrl, ext);
				string localPath = PathUtil.Combine(Application.dataPath, path);
				if (File.Exists(localPath))
				{
					return path;
                }
			}
			return null;
		}

        public void GetAssetBundle(string url, bool unload, Action<AssetBundle> callback)
        {
            string path = GetEditorAssetPath<AssetBundle>(url);
            if (path.IsNotEmpty())
            {
                AssetBundle ab = AssetBundle.LoadFromFile(path);
                if (ab != null)
                {
                    callback(ab);
                } else
                {
                    fallback.GetAssetBundle(url, unload, callback);
                }
            } else
            {
                fallback.GetAssetBundle(url, unload, callback);
            }
        }

		public void GetBytes(string url, Action<byte[]> callback)
		{
			TextAsset asset = LoadEditorAsset<TextAsset>(url);
			if (asset != null)
			{
				callback.Call(asset.bytes);
			} else
			{
				fallback.GetBytes(url, callback);
			}
		}

		public void GetTexture(string url, Action<Texture2D> callback)
		{
			Texture2D asset = LoadEditorAsset<Texture2D>(url);
			if (asset != null)
			{
				callback(asset);
			} else
			{
				fallback.GetTexture(url, callback);
			}
		}

        public void GetAudio(string url, AudioClipLoadType loadType, Action<AudioClip> callback)
		{
			AudioClip asset = LoadEditorAsset<AudioClip>(url);
			if (asset != null)
			{
				callback.Call(asset);
			} else
			{
				fallback.GetAudio(url, loadType, callback);
			}
		}

		public bool IsCached(string url)
		{
			url = Cdn.ToRelativePath(url);
			if (url.Is(FileType.Asset))
			{
				return FindAsset(url, ".asset", ".prefab", ".png", ".jpg", ".dds", ".tga", ".tiff", ".tif", ".psd", ".ogg", ".mp3", ".wav", ".unity", ".mat", ".anim", ".txt", ".bytes", ".csv") != null;
			} else
			{
				string localPath = PathUtil.Combine(Application.dataPath, url);
				return File.Exists(localPath);
			}
		}

		public bool IsValid(string url, object version)
		{
			return true;
		}

		public void Remove(string url)
		{
			fallback.Remove(url);
		}

		public Uri GetCachePath(string url)
		{
			#pragma warning disable 0162
			#if UNITY_EDITOR
			return new Uri(url, UriKind.RelativeOrAbsolute);
			#else
			return null;
			#endif
			#pragma warning restore 0162
		}

		private IAssetLoader fallback = new DummyAssetLoader();

		public void SetFallback(IAssetLoader fallback)
		{
			this.fallback = fallback;
		}

        public void UnloadAsset(string url)
        {
            this.fallback.UnloadAsset(url);
        }
	}
}
