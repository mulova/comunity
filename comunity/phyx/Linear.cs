using System;
using UnityEngine;

namespace comunity 
{
	public class Linear : PhyxObj
	{
		public float speed = 1;

		protected override void Init (Collider2D target, float dt)
		{
			Rigidbody2D body = GetRigidbody();
			body.gravityScale = 0;
			body.velocity = (target.transform.position-transform.position)/dt*speed;
		}
	}
}
