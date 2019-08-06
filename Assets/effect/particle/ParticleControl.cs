//----------------------------------------------
// Unity3D common libraries and editor tools
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System;
using comunity;
using mulova.commons;
using UnityEngine.Ex;
using System.Ex;

namespace effect
{
	[ExecuteInEditMode]
	public class ParticleControl : InternalScript, IReleasable
	{
		internal int poolId { get; set; }
		//	public Renderer[] renderers;
		public ParticleSystem[] particleSystems = new ParticleSystem[0];
		private ParticleSystemRenderer[] _renderers;
		public bool ignoreTimeScale;
		public bool ignorePause = true;
		public float duration = -1;
		public bool estimateDurationRecursively;
		public bool ignoreParentRotation;
		public object userObj;
		private Action<ParticleControl> endCallback;
		private float timePassed = -1;
		private float scale = 1;

		public ParticleSystem[] particles
		{
			get
			{
				if (particleSystems.IsEmpty())
				{
					particleSystems = GetComponentsInChildren<ParticleSystem>(true);
				}
				return particleSystems;
			}
		}

		public ParticleSystemRenderer[] renderers
		{
			get
			{
				if (_renderers == null)
				{
					_renderers = particles.Convert(p => p.GetComponent<ParticleSystemRenderer>());
				}
				return _renderers;
			}
		}

//		public void FlipX(bool flip)
//		{
//			if (this.flipped == flip) { return; }
//			this.flipped = flip;
//			foreach (ParticleSystem s in GetParticleSystems())
//			{
//				s.startSpeed  *= -1;
//				ParticleSystemRenderer r = s.GetComponent<ParticleSystemRenderer>();
//				if (r != null)
//				{
//					if (r.renderMode == ParticleSystemRenderMode.Stretch)
//					{
//						r.lengthScale *= -1;
//						r.velocityScale *= -1;
//						r.cameraVelocityScale *= -1;
//					} else
//					{
//						foreach (Material mat in r.materials)
//						{
//							if (flip)
//							{
//								mat.SetTextureOffset("_MainTex", new Vector2(1, 0));
//								mat.SetTextureScale("_MainTex", new Vector2(-1, 1));
//							} else
//							{
//								mat.SetTextureOffset("_MainTex", new Vector2(0, 0));
//								mat.SetTextureScale("_MainTex", new Vector2(1, 1));
//							}
//						}
//					}
//				}
//				s.startRotation *= -1;
//
//				Vector3 v2 = s.transform.localPosition;
//				v2.x *= -1;
//				s.transform.localPosition = v2;
//			}
//		}

		public void Play(Action<ParticleControl> endCallback = null)
		{
			this.endCallback = endCallback;
			if (duration <= 0)
			{
				duration = EstimateDuration();
			}
			gameObject.SetActive(true);
			InitModule();
			foreach (ParticleSystem s in particles)
			{
//				s.emission.enabled = true;
				if (!s.main.loop)
				{
					s.time = 0;
				}
				s.Play();
			}
			timePassed = 0;
			log.Info("Play Particle {0}", name);
		}

		private void InitModule()
		{
			if (particles.IsEmpty())
			{
				return;
			}
			ParticleSystemScalingMode scaleMode = particles[0].main.scalingMode;
			if (scaleMode == ParticleSystemScalingMode.Local)
			{
				scale = trans.localScale.x;
			} else if (scaleMode == ParticleSystemScalingMode.Hierarchy)
			{
				scale = trans.lossyScale.x;
			}
			Scale(scale);
		}

		public void Scale(float s)
		{
			if (s == 1)
			{
				return;
			}
			for (int i=0; i<particles.Length; ++i)
			{
				var p = particles[i];
				var r = renderers[i];
				if (r.renderMode == ParticleSystemRenderMode.Stretch)
				{
					r.lengthScale *= s;
				} else
				{
#if UNITY_5_5_OR_NEWER
					ParticleSystem.MainModule module = p.main;
					module.startSpeedMultiplier = s;
#else
					p.startSpeed *= s;
#endif
//					p.startSize *= s;
					
				}

			}
		}

		private void ResetModule()
		{
			if (scale == 1)
			{
				return;
			}
			Scale(1/scale);
			scale = 1;
		}

		public void Skip()
		{
			timePassed = duration;
		}

		public void Stop()
		{
			foreach (ParticleSystem s in particles)
			{
//				s.emission.enabled = false;
				s.Clear();
				s.Stop();
			}
			timePassed = -1;
			endCallback = null;
			gameObject.SetActiveEx(false);
			log.Debug("Stop Particle {0}", name);
		}

		public void SetScale(float scale)
		{
			float ratio = scale / this.scale;
			this.scale = scale;
			foreach (ParticleSystem s in particles)
			{
#if UNITY_5_5_OR_NEWER
				ParticleSystem.MainModule module = s.main;
				module.startSpeedMultiplier = ratio;
				module.startSizeMultiplier = ratio;
#else
				s.startSize *= ratio;
				s.startSpeed *= ratio;
#endif
			}
		}

		public void Init(int id)
		{
			this.poolId = id;
		}

		public void Release()
		{
			Stop();
			ResetModule();
			EventRegistry.SendEvent(ParticlePool.RELEASE_PARTICLE, this);
		}

		public bool IsPlaying()
		{
			return timePassed >= 0&&timePassed < duration;
		}

		void Update()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			
			if (timePassed >= 0)
			{
				if (ignoreParentRotation)
				{
					trans.rotation = Quaternion.identity;
				}
				if (ignoreTimeScale)
				{
					if (ignorePause||Time.timeScale != 0)
					{
						timePassed += Time.unscaledDeltaTime;
						float invScale = 1 / Time.timeScale;
						foreach (ParticleSystem p in particles)
						{
							if (Time.timeScale == 0)
							{
								p.Simulate(Time.unscaledDeltaTime, true, false);
							} else
							{
#if UNITY_5_5_OR_NEWER
								ParticleSystem.MainModule module = p.main;
								module.simulationSpeed = invScale;
#else
								p.playbackSpeed = invScale;
#endif
							}
						}
					}
				} else
				{
					timePassed += Time.deltaTime;
				}
				
				if (timePassed >= duration)
				{
					Action<ParticleControl> c = endCallback;
					Release();
					c.Call(this);
				}
			}
		}

		/*
		void LateUpdate ()
		{
			if (flipped) {
				foreach (ParticleSystem p in GetParticleSystems()) {
					// check array size
					if (particles == null || particles.Length < p.maxParticles) {
						particles = new ParticleSystem.Particle[p.maxParticles];
					}

					int count = p.GetParticles(particles);
					if (count > 0) {
						for(int i = 0; i < count; i++)
						{
							Vector3 v = particles[i].velocity;
							v.x = -v.x;
							v.y = -v.y;
							v.z = -v.z;
							particles[i].velocity = v;
							Vector3 pos = particles[i].position;
							pos.x = -pos.x;
							pos.y = -pos.y;
							pos.z = -pos.z;
							particles[i].position = pos;
							particles[i].angularVelocity *= -1;
							particles[i].rotation *= -1;
						}
						p.SetParticles(particles, count);
					}
				}
			}
		}
*/
		public float EstimateDuration()
		{
			float duration = 0;
			ParticleSystem[] ps = this.particles;
			if (ps.Length > 0&&!estimateDurationRecursively)
			{
				ps = new ParticleSystem[] { ps[0] };
			}
			foreach (ParticleSystem s in ps)
			{
#if UNITY_5_5_OR_NEWER
				ParticleSystem.MainModule module = s.main;
				float particleDuration = module.startDelayMultiplier+module.duration+module.startLifetimeMultiplier;
#else
				float particleDuration = s.startDelay+s.duration+s.startLifetime;
#endif
				if (particleDuration > duration)
				{
					duration = particleDuration;
				}
			}
			log.Debug("particle '{0}' lifetime is estimated: {1} sec", name, duration);
			return duration;
		}

		public void SetRenderQueue(int renderQueue)
		{
			foreach (ParticleSystem p in particles)
			{
				Renderer r = p.GetComponent<Renderer>();
				if (r != null)
				{
					Material[] materials = r.materials;
					bool mod = false;
					foreach (Material m in materials)
					{
						if (m.renderQueue != renderQueue)
						{
							m.renderQueue = renderQueue;
							mod = true;
						}
					}
					if (mod)
					{
						r.materials = materials;
					}
				}
			}
		}

		public void SetRqDelta(Renderer r, int delta)
		{
			go.SetLayer(r.gameObject.layer);
			SetRenderLayer(r.sortingLayerName, r.sortingOrder+delta);
			SetRenderQueue(r.material.renderQueue+delta);
		}

		public void SetSortOrder(int sortOrder)
		{
			SetRenderLayer(null, sortOrder);
		}

		public void SetRenderLayer(string sortLayer, int sortOrder)
		{
			if (particles != null)
			{
				foreach (ParticleSystem s in particles)
				{
					Renderer r = s.GetComponent<Renderer>();
					if (r != null)
					{
						if (sortLayer != null)
						{
							r.sortingLayerName = sortLayer;
						}
						r.sortingOrder = sortOrder;
					} else
					{
						log.Error("Missing ParticleRenderer");
					}
				}
			}
		}

		public void SetParent(Transform parent)
		{
			if (parent != null)
			{
				int parentLayer = parent.gameObject.layer;
				if (gameObject.layer != parentLayer)
				{
					gameObject.SetLayer(parentLayer);
				}
			}
			transform.SetParent(parent, false);
		}
	}
}
