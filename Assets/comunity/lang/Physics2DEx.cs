using UnityEngine;
using System.Collections.Generic;
using commons;

namespace comunity
{
	public static class Physics2DEx
	{
		public static T Raycast<T>(Vector2 screenPos, int layer) where T: Component {
			return Raycasts<T>(screenPos, layer).GetFirst();
		}
		
		public static List<T> Raycasts<T>(Vector2 screenPos, int layer) where T: Component {
			Camera cam = CameraEx.GetCamera(layer);
			Ray ray = cam.ScreenPointToRay(screenPos);
			RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, float.PositiveInfinity, cam.cullingMask);
			List<T> list = new List<T>();
			foreach (RaycastHit2D r in hits) {
				if (r.collider != null) {
					T t = r.collider.GetComponent<T>();
					if (t != null) {
						list.Add(t);
					}
				}
			}
			return list;
		}
	}
}

