using UnityEngine;

namespace comunity
{
	public static class RigidbodyEx
	{
		public static void MoveDelta(this Rigidbody body, Vector3 delta, Transform t) {
			Vector3 pos = t.position + delta;
			body.MovePosition(pos);
		}

		public static void MoveDelta(this Rigidbody body, Vector3 delta) {
			MoveDelta(body, delta, body.transform);
		}
	}
}

