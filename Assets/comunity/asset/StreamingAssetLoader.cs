#if !UNITY_WEBGL
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using commons;
using System.Text.Ex;
using System.Collections.Generic.Ex;

namespace comunity
{

	public class StreamingAssetLoader : IAssetLoader
	{

		public class Entry
		{
			public readonly string url;
			public string assetPath;
			public bool init;
			public bool exists;

			public Entry(string url, string assetPath)
			{
				this.url = url;
				this.assetPath = assetPath;
			}

			public FileInfo GetFile()
			{
				if (init&&!exists)
				{
					return null;
				}
				if (assetPath.IsEmpty()||(Platform.platform == RuntimePlatform.Android&&!Application.isEditor))
				{
					return null;
				}
				FileInfo f = new FileInfo(assetPath);
				if (!f.Exists)
				{
					// .asset exists?
					f = new FileInfo(PathUtil.ReplaceExtension(assetPath, FileTypeEx.ASSET_BUNDLE));
				}
				init = true;
				exists = f.Exists;
				if (f.Exists)
				{
					return f;
				} else
				{
					return null;
				}
			}
		}

		public const string VERSION_FILE = "asset_version.txt";
		private ConvertedKeyMap<string, Entry> url2local = new ConvertedKeyMap<string, Entry>((k) =>
		{
			return k.ToLower();
		});
		// key: remote url   value: local file path
#if WWW_MODULE
		private MultiMap<string, Action<WWW>> callbacks = new MultiMap<string, Action<WWW>>();
#endif
		public static readonly Loggerx log = AssetCache.log;

		public StreamingAssetLoader()
		{
		}

		public static int version
		{
			get
			{
				return BuildConfig.STREAMING_ASSET_VERSION;
			}
		}

		private string root;

		private string Root
		{
			get
			{
				if (root == null)
				{
                    root = PathUtil.Combine(Platform.streamingAssetsPath, Cdn.DIR);
				}
				return root;
			}
		}

		private Entry GetCacheEntry(string url)
		{
			Entry e = url2local.Get(url);
			if (e == null)
			{
				if (version > 0&&Cdn.Path.IsNotEmpty())
				{
					// find local cdn first
					string absPath = url.Replace(Cdn.Path, Root);
					e = new Entry(url, absPath);
				} else
				{
					e = new Entry(url, null);
				}
				url2local.Add(url, e);
			}
			return e;
		}

#if WWW_MODULE
		private void GetWWW(string url, bool dispose, Action<WWW> callback)
		{
			callbacks.Add(url, callback);
			if (callbacks.GetCount(url) > 1)
			{
				return;
			}
			Threading.inst.StartCoroutine(GetWWWCo(url, dispose, www =>
			{
				List<Action<WWW>> slot = callbacks.GetSlot(url);
				callbacks.Remove(url);
				slot.ForEach(s => s(www));
			}));
		}

		private IEnumerator GetWWWCo(string url, bool dispose, Action<WWW> callback)
		{
			Entry e = GetCacheEntry(url);
			if (e.init&&!e.exists)
			{
				callback.Call(null);
			} else
			{
				if (!e.assetPath.Contains("://"))
				{
					e.assetPath = "file://"+e.assetPath;
				}
				WWW www = new WWW(e.assetPath);
				yield return www;
				if (www.error.IsNotEmpty()&&!e.assetPath.Is(FileType.Asset))
				{
					www.DisposeEx();
					e.assetPath = PathUtil.ReplaceExtension(e.assetPath, FileTypeEx.ASSET_BUNDLE);
					www = new WWW(e.assetPath);
					yield return www;
				}
				e.init = true;
				e.exists = www.error.IsEmpty();
				if (e.exists)
				{
					callback.Call(www);
				} else
				{
					callback.Call(null);
				}
				if (dispose)
				{
					www.DisposeEx();
				}
			}
		}
#endif


		/// <summary>
		/// Gets the bytes. Support Web Platform
		/// </summary>
		/// <param name="cdnRelativeUrl">Cdn relative URL.</param>
		/// <param name="callback">Callback.</param>
		public void GetBytes(string url, Action<byte[]> callback)
		{
			Entry e = GetCacheEntry(url);
			if (e != null)
			{
				FileInfo f = e.GetFile();
				if (f != null)
				{
                    FileAssetLoader.GetBytesFromFile(f, callback);
				} else
				{
#if WWW_MODULE
					GetWWW(url, true, www =>
					{
						if (www != null&&www.error.IsEmpty())
						{
							callback.Call(www.GetAsset<byte[]>());
						} else
						{
							fallback.GetBytes(url, callback);
						}
					});
#else
					throw new Exception("Not implemented");
#endif
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <returns> return the local cache path if asset already exists in local cache, or null</returns>
		/// <param name="url">URL.</param>
		public Uri GetCachePath(string url)
		{
			Entry e = GetCacheEntry(url);
			return new Uri(e.assetPath, UriKind.RelativeOrAbsolute);
		}

		public void GetAsset<T>(string url, Action<T> callback, bool asyncHint) where T: Object
		{
			Entry e = GetCacheEntry(url);
			FileInfo f = asyncHint? null : e.GetFile();
			if (f != null)
			{
                FileAssetLoader.GetAssetBundleFromFile(f.FullName, bundle=> {
                    if (bundle != null)
                    {
                        T asset = bundle.mainAsset as T;
                        bundle.Unload(false);
                        callback.Call(asset);
                    } else
                    {
                        fallback.GetAsset<T>(url, callback, asyncHint);
                    }
                });
			} else
			{
#if WWW_MODULE
				GetWWW(url, true, www =>
				{
					if (www != null)
					{
						T asset = www.GetAsset<T>();
						callback.Call(asset);
					} else
					{
						fallback.GetAsset<T>(url, callback, asyncHint);
					}
				});
#else
				throw new Exception("Not implemented");
#endif
			}
		}

        public void GetAssetBundle(string url, bool unload, Action<AssetBundle> callback)
        {
            throw new NotImplementedException();
//            fallback.GetAssetBundle(url, unload, callback);
        }

		public void GetAssets<T>(string url, Action<IEnumerable<T>> callback, bool asyncHint) where T: Object
		{
			Entry e = GetCacheEntry(url);
			FileInfo f = asyncHint? null : e.GetFile();
			if (f != null)
			{
                FileAssetLoader.GetAssetBundleFromFile(f.FullName, bundle=> {
                    if (bundle != null)
                    {
                        T[] assets = bundle.LoadAllAssets<T>();
                        bundle.Unload(false);
                        callback.Call(assets);
                    } else
                    {
                        fallback.GetAssets<T>(url, callback, asyncHint);
                    }
                });
			} else
			{
#if WWW_MODULE
				GetWWW(url, true, www =>
				{
					if (www != null)
					{
						T[] assets = www.GetAssets<T>();
						callback.Call(assets);
					} else
					{
						fallback.GetAssets<T>(url, callback, asyncHint);
					}
				});
#else
				throw new Exception("Not implemented");
#endif
			}
		}

		public void GetTexture(string url, Action<Texture2D> callback)
		{
			Entry e = GetCacheEntry(url);
			FileInfo f = e.GetFile();
			if (f != null)
			{
				FileAssetLoader.GetTextureFromFile(url, f, callback);
			} else
			{
#if WWW_MODULE
				GetWWW(url, true, www =>
				{
					if (www != null)
					{
						// load image directly to set texture without delay
						log.Debug("Loading texture {0}", url);
						Texture2D texture = www.textureNonReadable;
						if (texture == null)
						{
							AssetCache.log.Warn("Can't load textures {0}", url);
						}
						callback.Call(texture);
					} else
					{
						fallback.GetTexture(url, callback);
					}
				});
#else
				throw new Exception("Not implemented");
#endif
			}
		}

        public void GetAudio(string url, AudioClipLoadType loadType, Action<AudioClip> callback)
		{
            bool streaming = (loadType & AudioClipLoadType.Streaming) != 0;
            bool threeD = (loadType & AudioClipLoadType.ThreeD) != 0;
            bool compressed = (loadType & AudioClipLoadType.Compressed) != 0;

#if WWW_MODULE
			GetWWW(url, !streaming, www =>
			{
				if (www != null)
				{
                    AudioClip a = compressed? www.GetAudioClipCompressed(threeD): www.GetAudioClip(threeD, streaming);
					callback.Call(a);
				} else
				{
					fallback.GetAudio(url, loadType, callback);
				}
			});
#else
			throw new Exception("Not implemented");
#endif
		}

		public void Remove(string url)
		{
			fallback.Remove(url);
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
#endif
	