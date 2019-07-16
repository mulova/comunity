//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------


using UnityEditor;
using UnityEngine;
using comunity;

namespace convinity {
	class VisibilityCheckTab : EditorTab {
		
		public VisibilityCheckTab(TabbedEditorWindow window) : base("Visibility", window) {}
		
		public override void OnEnable() {
			cam = Camera.main;
		}
		public override void OnDisable() {}
		public override void OnSelected(bool sel) {}
		public override void OnFocus(bool focus) {}
		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		
		public override void OnHeaderGUI() {
		}
		
		private Camera cam;
		private GameObject obj;
		public override void OnInspectorGUI() {
			EditorGUIUtil.ObjectField<GameObject>("GameObject", ref obj, true);
			EditorGUIUtil.Popup<Camera>("Camera", ref cam, Camera.allCameras);
			
			if (obj != null) {
				Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
				if (renderers.Length == 0) {
					SetInfo("No active renderers");
				} else {
					foreach (Renderer r in renderers) {
						CheckScale(r);
						if (cam != null) {
							CheckFrustum(r);
							CheckLayer(r);
							CheckMaterial(r);
						}
					}
				}
			}
		}
		
		public override void OnFooterGUI() {
		}
		
		private void CheckMaterial(Renderer r) {
			if (r.sharedMaterial.HasProperty("_Color") && r.sharedMaterial.color.a == 0) {
				SetInfo(r.transform.GetScenePath() + " material has alpha 0");
			}
		}
		
		private void CheckLayer(Renderer r) {
			int layer = 1 << r.gameObject.layer;
			if ((cam.cullingMask & layer) == 0) {
				SetInfo(r.transform.GetScenePath()+" layer mismath with camera cullingMask");
			}
		}
		
		private void CheckFrustum(Renderer r) {
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
			if (!GeometryUtility.TestPlanesAABB(planes, r.bounds)) {
				SetInfo(r.transform.GetScenePath()+" is Out of Camera Frustum");
			}
		}
		
		private void CheckScale(Renderer r) {
			Transform t = r.transform;
			Vector3 scale = t.lossyScale;
			if (IsZero(scale.x) || IsZero(scale.y) || IsZero(scale.z)) {
				while (t != null) {
					if (t.localScale == Vector3.zero) {
						SetInfo(t.GetScenePath() + " has Zero scale");	
						t = t.parent;
					}
				}
			}
		}
		
		private float ZERO_THRESHOLD = 0.0001f;
		private bool IsZero(float f) {
			return f < ZERO_THRESHOLD && f >= -ZERO_THRESHOLD;
		}
		
	}
}