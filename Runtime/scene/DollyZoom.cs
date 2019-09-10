#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;

/// <summary>
/// From http://docs.unity3d.com/Documentation/Manual/DollyZoom.html
/// </summary>

namespace mulova.comunity
{
	public class DollyZoom : MonoBehaviour
	{
		public Transform target;
		private float initHeightAtDist;
		private Vector3 pos;
		private Camera cam;
	
		// Calculate the frustum height at a given distance from the camera.
		public float FrustumHeightAtDistance(float distance)
		{
			return 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		}
	
	
		// Calculate the FOV needed to get a given frustum height at a given distance.
		public float FOVForHeightAndDistance(float height, float distance)
		{
			return 2 * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
		}
	
	
		// Start the dolly zoom effect.
		void OnEnable()
		{
			float distance = Vector3.Distance(transform.position, target.position);
			cam = GetComponent<Camera>();
			initHeightAtDist = FrustumHeightAtDistance(distance);
		}
	
		void Update()
		{
			Vector3 newPos = transform.localPosition;
			if (pos != newPos) {
				pos = newPos;
				// Measure the new distance and readjust the FOV accordingly.
				float currDistance = Vector3.Distance(transform.position, target.position);
				cam.fieldOfView = FOVForHeightAndDistance(initHeightAtDist, currDistance);
			}
		}
	}

}
#endif