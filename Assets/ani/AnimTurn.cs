using UnityEngine;
using System.Collections.Generic;
using System;
using mulova.commons;
using comunity;
using UnityEngine.Ex;
using System.Ex;

namespace ani {
	public class AnimTurn : SeqTurn {
		private Animation anim;
		public List<string> clipNames = new List<string>();
		private Action callback;
		private int totalCount;
		private int count;
		
		public AnimTurn() { }
		
		public AnimTurn(Animation anim, params string[] clips) {
			this.anim = anim;
			int layer = 1;
			foreach (AnimationState s in anim) {
				s.layer = layer++;
			}
			clipNames.AddRange(clips);
			if (Application.isEditor) {
				foreach (string c in clips) {
					if (anim.GetClip(c) == null) {
						Seq.log.Error("No clip named '{0}'", c);
					}
				}
			}
		}
		
		public void Rewind() {
			if (anim != null) {
				anim.Rewind ();
			}
		}
		
		public void Play(Action callback) {
			if (clipNames.IsNotEmpty()) {
				this.callback = callback;
				anim.gameObject.SetActive(true);
//				anim.AddOneShotCallback(OnComplete);
				totalCount = clipNames.Count;
				count = 0;
				for (int i=0; i<clipNames.Count; ++i) {
					AnimationState s = anim[clipNames[i]];
					if (s != null) {
						anim.Rewind(clipNames[i]);
						anim.PlayIgnoreScale(clipNames[i], OnComplete);
					} else {
						totalCount--;
                        Assert.Fail(null, "No animation clip '{0}'", clipNames[i]);
					}
				}
			} else {
				callback.Call();
			}
		}
		
		public void Skip() {
			for (int i=0; i<clipNames.Count; ++i) {
				AnimationState s = anim[clipNames[i]];
				if (s != null) {
					s.time = s.length-Time.unscaledDeltaTime;
				} else {
					Assert.Fail(null, "No animation clip '{0}'", clipNames[i]);
				}
			}
			anim.Sample();
		}

		public void Stop() {
			if (anim == null) {
				return;
			}
			anim.Stop();
			callback = null;
		}
		
		private void OnComplete() {
			//			for (int i=0; i<clipNames.Count; ++i) {
			//				AnimationState s = anim[clipNames[i]];
			//				if (s != null) {
			//					s.time = s.length;
			//				}
			//			}
			//			anim.Sample();
			count++;
			if (totalCount == count) {
				callback.Call();
			}
		}
		
		public override string ToString()
		{
			return String.Join(",", clipNames.ToArray());
		}
	}
}
