using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using Object = UnityEngine.Object;
using commons;

namespace comunity
{
	/// <summary>
	/// TODOM AssetBundle version is not used.
	/// </summary>
	public class WebAssetLoader : IAssetLoader
	{
		public readonly bool caching;
		public static readonly Loggerx log = AssetCache.log;

		public WebAssetLoader(bool caching)
		{
			this.caching = caching;
		}

#if WWW_MODULE
		private void GetWWW(string url, bool dispose, Action<WWW> callback)
		{
            log.Debug("Access web {0}", url);
			Threading.inst.StartCoroutine(LoadWWW(url, dispose, callback));
		}
#endif

		public void GetAsset<T>(string url, Action<T> callback, bool asyncHint) where T: Object
		{
#if WWW_MODULE
			GetWWW(url, true, www =>
			{
				if (www != null)
				{
					T asset = www.GetAsset<T>();
					callback(asset);
				} else
				{
					callback(null);
				}
			});
#else
			throw new Exception("Not implemented");
#endif
		}

		public void GetAssets<T>(string url, Action<IEnumerable<T>> callback, bool asyncHint) where T: Object
		{
#if WWW_MODULE
			GetWWW(url, true, www =>
			{
				if (www != null&&www.assetBundle != null)
				{
					T[] assets = www.assetBundle.LoadAllAssets<T>();
					callback(assets);
				} else
				{
					callback(null);
				}
			});
#else
			throw new Exception("Not implemented");
#endif
		}

		public void IsCached(string url, Action<bool> callback)
		{
			callback(IsCached(url));
		}

		public bool IsCached(string url)
		{
#pragma warning disable 0618
			return Caching.IsVersionCached(url, Cdn.GetFileVersion(url));
#pragma warning restore 0618
		}

		public bool IsValid(string url, object version)
		{
			return IsCached(url);
		}

        public void GetAssetBundle(string url, bool unload, Action<AssetBundle> callback)
        {
            throw new NotImplementedException();
//            fallback.GetAssetBundle(url, unload, callback);
        }

		public void GetBytes(string url, Action<byte[]> callback)
		{
#if WWW_MODULE
			GetWWW(url, true, www =>
			{
				if (www != null)
				{
					if (www.assetBundle != null)
					{
						TextAsset text = www.assetBundle.mainAsset as TextAsset;
						callback(text.bytes);
					} else
					{
						callback(www.bytes);
					}
				} else
				{
					callback(null);
				}
			});
#else
			throw new Exception("Not implemented");
#endif
		}

		public void GetTexture(string url, Action<Texture2D> callback)
		{
#if WWW_MODULE
			GetWWW(url, true, www =>
			{
				if (www != null)
				{
					if (www.assetBundle != null)
					{
						Texture2D tex = www.assetBundle.mainAsset as Texture2D;
						callback(tex);
					} else
					{
						Texture2D tex = www.textureNonReadable as Texture2D;
						callback(tex);
					}
				}
			});
#else
			throw new Exception("Not implemented");
#endif
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
					callback(a);
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
			log.Warn("no remove effect");
			fallback.Remove(url);
		}

		public Uri GetCachePath(string url)
		{
			return new Uri(url, UriKind.RelativeOrAbsolute);
		}

#if WWW_MODULE
		public WWW CreateWWW(string url)
		{
			string parent = PathUtil.GetParent(url);
			string filename = WWW.EscapeURL(Path.GetFileName(url));
			if (caching)
			{
				int version = Cdn.GetFileVersion(url);
				filename = PathUtil.ReplaceExtension(filename, FileTypeEx.ASSET_BUNDLE);
				string uri = PathUtil.Combine(parent, filename);
				if (log.IsLoggable(LogLevel.DEBUG))
				{
#pragma warning disable 0618
					log.Debug("{0} (ver {1}): {2}", uri, version, Caching.IsVersionCached(uri, version)? "Cached": "Download & Cache");
#pragma warning restore 0618
				}
				return WWW.LoadFromCacheOrDownload(uri, version);
			} else
			{
				string uri = PathUtil.Combine(parent, filename);
				return new WWW(uri);
			}
		}
		private IEnumerator LoadWWW(string url, bool dispose, Action<WWW> callback)
		{
			WWW www = CreateWWW(url);
			yield return www;
			string err = www.error;

			if (err.IsNotEmpty())
			{
				log.Warn("{0}: {1} ({2})", err, www.url, url);
				callback(null);
			} else
			{
//				PlatformMethods.inst.SetNoBackupFlag(encoded, version);
				callback(www);
			}
			if (dispose)
			{
				www.DisposeEx();
			}
		}
#endif


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
