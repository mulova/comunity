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
	public DrawCameraMode renderMode;

	public SceneView sceneView {
		get {
			if (SceneView.lastActiveSceneView != null)
			{
				return SceneView.lastActiveSceneView;
			} else
			{
				if (SceneView.sceneViews.Count > 0)
				{
					return (SceneView)SceneView.sceneViews[0];
				}
			}
			return null;
		}
	}

	public void Collect()
	{
		var view = sceneView;
		if (view == null)
		{
			Debug.LogWarning("Can't access SceneView.");
			return;
		}
		size = view.size;
		in2dMode = view.in2DMode;
		rot = sceneView.rotation;
		pivot = sceneView.pivot;
		ortho = view.orthographic;
//		fov = ortho? view.camera.orthographicSize: view.camera.fieldOfView;
		rotationLocked = sceneView.isRotationLocked;
		renderMode = sceneView.renderMode;
	}

	public void Apply()
	{
		var view = sceneView;
		if (view == null)
		{
			Debug.LogWarning("Can't access SceneView.");
			return;
		}
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
		sceneView.isRotationLocked = rotationLocked;
		sceneView.renderMode = renderMode;
	}
}