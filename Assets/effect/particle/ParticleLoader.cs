using UnityEngine;
using System;
using comunity;
using commons;

namespace effect
{
	public class ParticleLoader : InternalScript
	{
		public AssetRef asset;
		public bool loop;
		public bool ignoreScale;
		public bool ignorePause;
		public bool playOnAwake;
		private ParticleControl particle;
		private bool loading;

		void Start()
		{
			if (playOnAwake)
			{
				Play();
			}
		}

		public bool IsPlaying()
		{
			return particle != null&&particle.IsPlaying();
		}

		[ContextMenu("Play")]
		public void Play()
		{
			Play(null, null);
		}

		public void Play(Action<ParticleControl> endCallback)
		{
			Play(null, endCallback);
		}

		public void Play(Action<ParticleControl> loadCallback, Action<ParticleControl> endCallback)
		{
			if (!asset.isEmpty)
			{
				if (particle == null)
				{
					if (!loading)
					{
						loading = true;
						ParticleManager.inst.GetParticle(asset.path, p =>
						{
							loading = false;
							particle = p;
							if (p != null)
							{
								InitParticle();
								loadCallback.Call(particle);
								particle.Play(e =>
								{
									particle = null;
									endCallback.Call(e);
								});
							} else
							{
								loadCallback.Call(null);
								endCallback.Call(null);
								log.Error("Missing particle {0}", asset.actualPath);
							}
						});
					}
				} else if (!particle.IsPlaying())
				{
					InitParticle();
					loadCallback.Call(particle);
					particle.Play(e =>
					{
						particle = null;
						endCallback.Call(e);
					});
				}
			} else
			{
				endCallback.Call(null);
			}
		}

		private void InitParticle()
		{
			particle.SetParent(transform);
			if (loop)
			{
				particle.duration = float.MaxValue;
			} else
			{
				particle.duration = particle.EstimateDuration();
			}
			particle.gameObject.SetLayer(gameObject.layer);
			particle.ignoreTimeScale = ignoreScale;
			particle.ignorePause = ignorePause;
		}

		public void Stop()
		{
			if (particle == null)
			{
				return;
			}
			particle.Release();
			loading = false;
			particle = null;
		}
		
	}
}