//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using UnityEngine;

namespace mulova.comunity
{
	public static class CameraEx {
		
		/**
		* @param windowWidth
		* @param distance   the distance from the monitor to your face ( lense to image)
		*/
		public static float CalculateFOV(float windowWidth, float focalDistance) {
			return Mathf.Atan(windowWidth*focalDistance/2)*2;
		}
		
		public static void SetFOV(this Camera cam, float d1, float fov1, float d2, float fov2, float distance) {
			float fov = (fov2-fov1)/(d2-d1)*(distance-d1)+fov1;
			cam.fieldOfView = fov;
		}
		
		/// <summary>
		/// Sets the oblique.
		/// </summary>
		/// <param name="cam">Cam.</param>
		/// <param name="horizObl">Horizontal oblique. -1 ~ 1</param>
		/// <param name="vertObl">Vertical oblique. -1 ~ 1</param>
		public static void SetOblique(this Camera cam, float horizObl, float vertObl) {
			Matrix4x4 mat = cam.projectionMatrix;
			mat[0, 2] = horizObl;
			mat[1, 2] = vertObl;
			cam.projectionMatrix = mat;
		}
		
		public static Camera GetCamera(int layer)
		{
			int layerMask = 1<<layer;
			foreach (Camera c in Camera.allCameras) {
				if (c.enabled) {
					if ((c.cullingMask & layerMask) != 0) {
						return c;
					} else if (layer == 0 && c.cullingMask == 0) {
						return c;
					}
				}
			}
			return null;
		}
		
		public static Vector3 ConvertPos(Transform src, int dstLayer, float dstZ = float.NaN)
		{
			return ConvertPos(src.gameObject.layer, src.position, dstLayer, dstZ);
		}
		
		public static Vector3 ConvertPos(int srcLayer, Vector3 srcPos, int dstLayer)
		{
			return ConvertPos(srcLayer, srcPos, dstLayer, float.NaN);
		}
		
		public static Vector3 ConvertPos(int srcLayer, Vector3 srcPos, int dstLayer, float dstZ = float.NaN)
		{
			if (srcLayer == dstLayer)
			{
				return srcPos;
			}
			var srcCam = GetCamera(srcLayer);
			var dstCam = GetCamera(dstLayer);
			return ConvertPos(srcCam, srcPos, dstCam, dstZ);
		}
		
		public static Vector3 ConvertPos(Camera srcCam, Vector3 srcPos, Camera dstCam, float dstZ)
		{
			if (srcCam == dstCam)
			{
				return srcPos;
			}
			var screen = srcCam.WorldToScreenPoint(srcPos);
			if (!float.IsNaN(dstZ))
			{
				screen.z = dstZ-dstCam.transform.position.z;
			}
			return dstCam.ScreenToWorldPoint(screen);
		}
	}
}
