using UnityEngine;
using System.Collections;
using System;


namespace comunity 
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Parabolic : PhyxObj {

		public bool velocityAsLookRotation;
		
		protected override void Init(Collider2D target, float dt) {
			Rigidbody2D body = GetRigidbody();
			body.gravityScale = 1;
			Vector3 begin = body.transform.position;
			Vector3 end = target.transform.position;
			Vector3 d = end - begin;
			
			float g = Physics2D.gravity.y;
			float vx = d.x / dt;
			float vy = d.y/dt -g * dt/2;
			body.velocity = new Vector2 (vx, vy);
		}

		private Transform trans;
		void Update() {
			if (velocityAsLookRotation) {
				if (trans == null) {
					trans = transform;
				}
				Vector2 dir = GetRigidbody().velocity.normalized;
				Quaternion q = new Quaternion();
				q.SetLookRotation(new Vector3(dir.x, dir.y, 0));
				trans.localRotation = q;
			}
		}
	}
}
