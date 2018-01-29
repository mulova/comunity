using UnityEngine;

using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;
using System.Collections;
using commons;

namespace comunity {
	/// <summary>
	/// Preload assets and pooling
	/// </summary>
	public class AssetPool<T> : IDisposable, IEnumerable<KeyValuePair<string, T>> where T: Object {
		private WeakValueDictionary<string, T> pool = new WeakValueDictionary<string, T>();
		private string urlFormat;
		public LoadDelegate assetLoader;
		public delegate void LoadDelegate(string key, Action<T> callback);
        private Loggerx _log;
        public Loggerx log
        {
            get
            {
                if (_log == null)
                {
                    _log = LogManager.GetLogger(GetType());
                }
                return _log;
            }
        }

		public AssetPool(string format) {
            pool.SetRefType(AssetRefType.Weak);
			assetLoader = LoadAsset;
			if (format != null) {
				urlFormat = format;
			}
		}

        public void SetRefType(AssetRefType refType)
        {
            pool.SetRefType(refType);
        }

		/// <summary>
		/// Gets the asset.
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="key">url if urlFormat is null or parameter for urlFormat otherwise.</param>
		public void GetAsset(string key, Action<T> callback) {
            T ins = pool.Get(key);
			if (ins != null) {
				callback(ins);
			} else {
				Preload(key, a=> {
                    callback(a);
				});
			}
		}

		public void Preload(string id, Action<T> callback)
		{
            string url = urlFormat.IsNotEmpty()? string.Format(urlFormat, id): id;
            assetLoader(url, a=> {
                if (a != null) {
                    pool[id] = a;
                } else {
                    log.Warn("Can't access {0}", url);
                }
                callback(a);
            });
		}

		/// <summary>
		/// Preload the specified urls and callback.
		/// </summary>
		/// <param name="urls">Asset urls to load</param>
		/// <param name="callback">Callback with actually loaded asset urls</param>
		public void PreloadAll(IList<string> ids, Action<T[]> callback)
		{
            if (ids.IsNotEmpty())
            {
                int c = ids.Count;
                T[] assets = new T[ids.Count];
                for (int i=0; i<ids.Count; ++i)
                {
                    string id = ids[i];
                    int index = i;
                    Preload(id, a=> {
                        assets[index] = a;
                        c--;
                        if (c == 0)
                        {
                            callback.Call(assets);
                        }
                    });
                }
            } else
            {
                callback.Call(new T[0]);
            }
		}

		public void LoadAsset(string url, Action<T> callback) {
			Cdn.cache.GetAsset<T>(url, a => {
				callback.Call(a);
			});
		}

		public void Dispose() {
//			foreach (KeyValuePair<string, T> pair in pool)
//			{
//				T o = pair.Value;
//				if (o != null)
//				{
//					o.DestroyEx();
//				}
//			}
			pool.Clear();
		}

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			return pool.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return pool.GetEnumerator();
		}
	}
}