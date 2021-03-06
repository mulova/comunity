﻿using System;
using System.Collections.Generic;
using System.Ex;
using mulova.commons;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.comunity
{
    public class DummyAssetLoader : IAssetLoader
	{
        public static readonly ILog log = AssetCache.log;

        public void GetAssetBundle(string url, bool unload, Action<AssetBundle> callback)
        {
            log.Warn("Failed to access {0}", url);
            callback.Call(null);
        }
#if WWW_MODULE
		public void GetWWW(string url, Action<WWW> callback)
		{
			log.Warn("Failed to access {0}", url);
			callback.Call(null);
		}
#endif
		public void GetAsset<T>(string url, Action<T> callback, bool asyncHint) where T : Object 
		{
			log.Warn("Failed to access {0}", url);
			callback.Call(null);
		}
		public void GetAssets<T>(string url, Action<IEnumerable<T>> callback, bool asyncHint) where T : Object
		{
			log.Warn("Failed to access {0}", url);
			callback.Call(null);
		}
		public void GetBytes(string url, Action<byte[]> callback)
		{
			log.Warn("Failed to access {0}", url);
			callback.Call(null);
		}
		public void GetTexture(string url, Action<Texture2D> callback)
		{
			log.Warn("Failed to access {0}", url);
			callback.Call(null);
		}
        public void GetAudio(string url, AudioClipLoadType loadType, Action<AudioClip> callback)
		{
			log.Warn("Failed to access {0}", url);
			callback.Call(null);
		}
		public void Remove(string url)
		{
		}
		public Uri GetCachePath(string url)
		{
			return null;
		}
		public void SetFallback(IAssetLoader fallback)
		{
		}

        public void UnloadAsset(string url)
        {
        }
	}
}

