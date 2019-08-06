//----------------------------------------------
// Unity3D common libraries and editor tools
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System;
using comunity;
using System.Collections.Generic;
using System.IO;
using mulova.commons;
using System.Collections.Generic.Ex;
using UnityEngine.Ex;

namespace effect
{
	/// <summary>
	/// Load Particles from url.
	/// Preload() must precede before other method calls
	/// </summary>
	public class ParticleManager : SingletonBehaviour<ParticleManager>, IDisposable
	{
		private AssetPool<GameObject> assetPool;
		private Dictionary<string, ParticlePool> particlePool = new Dictionary<string, ParticlePool>();
		private int minInstanceCount;

		protected override void Awake()
		{
			assetPool = new AssetPool<GameObject>(null);
		}

		protected override void OnDestroy()
		{
			Dispose();
		}

		public void SetMinInstanceCount(int min)
		{
			this.minInstanceCount = min;
			foreach (ParticlePool p in particlePool.Values)
			{
				p.SetMinInstanceCount(min);
			}
		}

		private ParticleControl GetParticleInstance(string url)
		{
			ParticlePool pool = particlePool.Get(url);
			if (pool != null)
			{
				return pool.GetInstance();
			} else
			{
				return null;
			}
		}

		public void Preload(IList<string> urls, Action callback = null)
		{
			assetPool.PreloadAll(urls, objs =>
			{
				foreach (KeyValuePair<string, GameObject> p in assetPool)
				{
					if (!particlePool.ContainsKey(p.Key))
					{
						GameObject poolGO = new GameObject(Path.GetFileNameWithoutExtension(p.Key));
						poolGO.transform.SetParent(transform, false);
						ParticlePool pool = poolGO.AddComponent<ParticlePool>();
						GameObject rawPrefab = p.Value.InstantiateEx(poolGO.transform);
						GameObject prefab = rawPrefab.CreateParent(rawPrefab.name);
						pool.SetPrefab(prefab);
						pool.SetMinInstanceCount(minInstanceCount);
						particlePool[p.Key] = pool;
					}
				}
			});
		}

		public void GetParticle(string path, Action<ParticleControl> loadCallback)
		{
			Preload(new string[] { path }, () =>
			{
				ParticleControl p = GetParticleInstance(path);
				loadCallback(p);
			});
		}

		public void Dispose()
		{
			assetPool.Dispose();
			particlePool.ForEach(p =>
			{
				p.Value.Dispose();
				p.Value.gameObject.DestroyEx();
			});
			particlePool.Clear();
		}
	}
}