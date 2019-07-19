using commons;

using System;
using System.Ex;

using UnityEngine;
using UnityEngine.Ex;

namespace comunity
{
    /// <summary>
    /// Phyx object.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
	public abstract class PhyxObj : MonoBehaviour {
		
		private Rigidbody2D body;
		private Collider2D target;
		private float dt;
		private Action callback;
		private Predicate<Rigidbody2D> threshold;
		public static readonly Loggerx log = LogManager.GetLogger(typeof(PhyxObj));
		
		public Rigidbody2D GetRigidbody() {
			if (body == null) {
				body = gameObject.FindComponent<Rigidbody2D>();
			}
			return body;
		}
		
		public void SetThreshold(Predicate<Rigidbody2D> threshold) {
			this.threshold = threshold;
		}
		
		public void SetTarget(Collider2D target, float dt) {
			this.target = target;
			this.dt = dt;
		}

		public void Play(Action callback) {
			if (GetComponent<Collider2D>() == null) {
				log.Error("No collider for {0}", name);
			}
			this.callback = callback;
			enabled = true;
			Threading.InvokeLater(()=> {
				Rigidbody2D body = GetRigidbody();
				body.isKinematic = false;
				Init(target, dt);
			});
		}
		protected abstract void Init(Collider2D target, float dt);

		public void Stop() {
			enabled = false;
			Rigidbody2D body = GetRigidbody();
			body.isKinematic = true;
			body.velocity = Vector2.zero;
		}

		public void Skip() {
			transform.position = target.transform.position;
			Stop();
			ActionEx.CallAfterRelease(ref callback);
		}
		
		protected void OnTriggerEnter2D(Collider2D c) {
			if (c == target || threshold.Call(GetRigidbody())) {
				Stop();
			}
		}
	}
}
