using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace comunity {

	public interface IAssetLoader
	{
        void GetAssetBundle(string url, bool unload, Action<AssetBundle> callback);

		void GetAsset<T>(string url, Action<T> callback, bool asyncHint) where T: Object;

		void GetAssets<T>(string url, Action<IEnumerable<T>> callback, bool asyncHint) where T: Object;

		void GetBytes(string url, Action<byte[]> callback);

		void GetTexture(string url, Action<Texture> callback);

        void GetAudio(string url, AudioClipLoadType loadType, Action<AudioClip> callback);

		void Remove(string url);

		Uri GetCachePath(string url);

		void SetFallback(IAssetLoader fallback);
	}
}