﻿using UnityEngine;
using UnityEditor;

public class SceneCamProperty
{
	public bool in2dMode;
	public float size;
	public float fov;
	public bool ortho;
	public Quaternion lastSceneViewRotation;
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
		size = view.size;
		in2dMode = view.in2DMode;
//		lastSceneViewRotation = sceneView.lastSceneViewRotation;
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
		view.size = size;
//		view.lastSceneViewRotation = lastSceneViewRotation;
		view.in2DMode = in2dMode;
		sceneView.rotation = rot;
		sceneView.pivot = pivot;
		view.orthographic = ortho;
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