//----------------------------------------------
// Unity3D common libraries and editor tools
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using core;

namespace effect {
	/// <summary>
	/// Feature
	/// 1. Reuse particle
	/// 2. Limit particle instance count
	/// 3. Select strong / weak reference of particles instance
	///
	/// Direct Call
	/// <code>
	/// ParticlePool pool;
	/// ParticleElement e = pool.Dequeue();
	/// e.Play();
	/// </code>
	/// 
	/// or By Event
	/// </summary>
	public class ParticlePool : InternalScript, IDisposable
	{
		public AssetRef asset = new AssetRef(); // type of AssetRef
		private GameObject prefab;
		private Queue<ParticleControl> queue = new Queue<ParticleControl>();
		public int instanceLimit = int.MaxValue;
		private int minInstanceCount;
		private int runningCount = 0; // currently running particle count (except in queue)
		private ParticleControl dummyParticle;
		private GameObject trashBin;
		private int id;

		public const string RELEASE_PARTICLE = "particle.release";

		void OnEnable() {
			EventRegistry.RegisterListener(RELEASE_PARTICLE, PutObj);
			id = GetHashCode();
		}

		void OnDisable() {
			EventRegistry.DeregisterListener(RELEASE_PARTICLE, PutObj);
		}

		private void PutObj(object o) {
			ParticleControl e = o as ParticleControl;
			if (this.id != e.poolId || e == dummyParticle) {
				return;
			}
			Put(e);
		}
		
		public void Put(ParticleControl p) {
			if (p == dummyParticle) {
				return;
			}
			p.Stop();
			Transform ptrans = p.transform;
			ptrans.SetParent(transform, false);
			p.gameObject.SetActive(false);
			queue.Enqueue(p);
			log.Debug("Enqueue {0}", p);
			runningCount--;
		}
		
		/// <summary>
		/// return null if size reaches the limit.
		/// </summary>
		public ParticleControl GetInstance() {
			ParticleControl e = null;
			if (queue.Count > 0) {
				e = queue.Dequeue();
			} else {
				if (runningCount < instanceLimit) {
					prefab = asset.GetReference() as GameObject;
					string name = prefab.name;
					if (Platform.isEditor) {
						name += runningCount;
					}
					GameObject inst = prefab.InstantiateEx(transform);
					e = inst.GetComponentEx<ParticleControl>();
					e.gameObject.SetActive(false);
				} else {
					if (dummyParticle == null) {
						GameObject go = new GameObject ("Dummy Particle");
						go.transform.SetParent (transform, false);
						dummyParticle = go.AddComponent<ParticleControl> ();
						go.SetActive (false);
					}
					return dummyParticle;
				}
			}
			InitInstance(e);
			runningCount++;
			log.Debug("Dequeue {0}", e);
			return e;
		}

		/// <summary>
		/// return null if size reaches the limit.
		/// </summary>
		public void Get(Action<ParticleControl> callback) {
			if (queue.Count > 0) {
				ParticleControl e = queue.Dequeue();
				log.Debug("Dequeue {0}", e);
				runningCount++;
				InitInstance(e);
				callback.Call(e);
			} else {
				if (runningCount < instanceLimit) {
					LoadParticle(p=> {
						log.Debug("Dequeue {0}", p);
						runningCount++;
						InitInstance(p);
						callback(p);
					});
				} else {
					if (dummyParticle == null) {
						GameObject go = new GameObject ("Dummy Particle");
						go.transform.SetParent (transform, false);
						dummyParticle = go.AddComponent<ParticleControl> ();
						go.SetActive (false);
					}
					callback.Call(dummyParticle);
				}
			}
		}

		private void LoadParticle(Action<ParticleControl> callback)
		{
			if (prefab == null) {
				asset.LoadAsset<GameObject>(o=> {
					prefab = o;
					prefab.SetLayer(gameObject.layer);
					callback(CreateInstance());
				});
			} else {
				callback(CreateInstance());
			}
		}

		private ParticleControl CreateInstance() {
			GameObject inst = prefab.InstantiateEx(transform);
			if (Platform.isEditor) {
				inst.name = prefab.name+runningCount;
			}
			ParticleControl e = inst.GetComponentEx<ParticleControl>();
			return e;
		}

		private void InitInstance(ParticleControl e)
		{
			e.poolId = id;
			e.trans.SetLocal(prefab.transform);
		}
		
		public void Play() {
			Get(p=> {
				p.Play();
			});
		}

		public void SetPrefab (GameObject p)
		{
			if (p.GetComponent<ParticleControl>() == null) {
				ParticleControl e = p.AddComponent<ParticleControl>();
				e.duration = e.EstimateDuration();
			}
			p.SetActive(false);
			p.transform.SetParent(transform);
			asset.reference = p;
			SetMinInstanceCount(minInstanceCount);
		}

		public void SetMinInstanceCount(int minInstanceCount) {
			this.minInstanceCount = minInstanceCount;
			LoadParticle(p=> {
				while (runningCount+queue.Count < minInstanceCount-1) {
					queue.Enqueue(p.InstantiateEx());
				}
				queue.Enqueue(p);
			});
		}
		
		public void Dispose ()
		{
			if (Application.isEditor) {
				return;
			}
			// Let particles destroyed when scene changes
			bool now = false;
			if (now) {
				queue.ForEach(o=>o.gameObject.DestroyEx());
			} else {
				if (trashBin == null) {
					trashBin = new GameObject("Trash Bin");
					trashBin.tag = "EditorOnly";
					trashBin.SetActive(false);
				}
				Transform t = trashBin.transform;
				queue.ForEach(o=>o.transform.parent = t);
			}
			queue.Clear();
		}
		
		public static ParticlePool Create(GameObject particlePrefab) {
			GameObject go = new GameObject(particlePrefab.name+"Pool");
			ParticlePool pool = go.AddComponent<ParticlePool>();
			pool.asset.reference = particlePrefab;
			particlePrefab.SetActive(false);
			return pool;
		}

	}
}
