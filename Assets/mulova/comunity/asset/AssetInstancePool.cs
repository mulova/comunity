using UnityEngine;

using Object = UnityEngine.Object;
using System.Collections;
using System;
using System.Collections.Generic;
using commons;

namespace comunity {
	/// <summary>
	/// instantiate or get asset from instance pool
	/// </summary>
	public class AssetInstancePool : Script {
        public LayerMask hideLayer; // if set, do not deactivate instance, but just move to the behind the cam
		private AssetPool<GameObject> assetPool;
		private MultiMap<string, GameObject> instancePool = new MultiMap<string, GameObject>();

		public void Init(AssetPool<GameObject> assetPool) {
			this.assetPool = assetPool;
		}
		
		public void Get(string url, Action<string, GameObject> callback) {
            assetPool.GetAsset(url, prefab=> {
				if (prefab == null) {
					callback(url, null);
				} else {
                    GameObject o = instancePool.RemoveOne(url);
					if (o == null) {
						o = prefab.InstantiateEx();
					} else {
						o.transform.SetLocal(prefab.transform);
					}
					o.SetActive(true);
					callback(url, o);
				}
			});
		}

		public void Put(string key, GameObject obj) {
            if (hideLayer == 0)
            {
                obj.transform.SetParent(transform, false);
                obj.SetActive(false);
            } else
            {
                obj.layer = hideLayer.value;
            }
			instancePool.Add(key, obj);
		}

        public void Preload(IList<string> ids, Action<GameObject[]> callback) {
            assetPool.PreloadAll(ids, callback);
		}
	}
}