using UnityEngine;
using UnityEditor;
using System;
using commons;

[Serializable]
public class SceneCamProperty
{
	public bool in2dMode;
	public float size;
	public bool ortho;
	public Vector3 pivot;
	public Quaternion rot;
	public bool rotationLocked;
	#if UNITY_2018_1_OR_NEWER
	public SceneView.CameraMode camMode;
	#else
	public DrawCameraMode camMode;
	#endif


	public void Collect(SceneView view)
	{
		size = view.size;
		in2dMode = view.in2DMode;
		rot = view.rotation;
		pivot = view.pivot;
		ortho = view.orthographic;
//		fov = ortho? view.camera.orthographicSize: view.camera.fieldOfView;
		rotationLocked = view.isRotationLocked;
		#if UNITY_2018_1_OR_NEWER
		camMode = sceneView.cameraMode;
		#else
		camMode = view.renderMode;
		#endif
	}

	public void Apply(SceneView view)
	{
		view.size = size;
		view.in2DMode = in2dMode;
		view.rotation = rot;
		view.pivot = pivot;
		view.orthographic = ortho;

		if (!in2dMode)
		{
			var svRot = ReflectionUtil.GetFieldValue<object>(view, "svRot");
			ReflectionUtil.Invoke(svRot, "ViewFromNiceAngle", view, !in2dMode);
		}

//		if (ortho) {
//			view.camera.orthographicSize = fov;
//		} else
//		{
//			view.camera.fieldOfView = fov;
//		}
		view.isRotationLocked = rotationLocked;
		#if UNITY_2018_1_OR_NEWER
		view.cameraMode = camMode;
		#else
		view.renderMode = camMode;
		#endif
	}
}