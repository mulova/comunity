//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------


using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using comunity;
using UnityEngine.Ex;

namespace convinity {
	class PerformanceTab : EditorTab {
		
		public PerformanceTab(TabbedEditorWindow window) : base("Performance", window) {}
		
		public override void OnEnable() { }
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
			CheckCamera();
			CheckFixedTimestamp();
			CheckAnimation();
			CheckShader();
		}
		
		public override void OnFooterGUI() {
		}
		
		private void CheckShader() {
			// TODO_mulova check if using mobile shader
			// check if using cutoff shader
		}
		
		private void CheckAnimation() {
			List<Animation> anims = new List<Animation>();
			foreach (Animation anim in EditorAssetUtil.FindSceneComponents<Animation>()) {
				if (anim.cullingType == AnimationCullingType.AlwaysAnimate) {
					anims.Add(anim);
				}
			}
			if (anims.Count > 0) {
				GUILayout.Label("AnimationCullingType.AlwaysAnimate", EditorStyles.boldLabel);
				var drawer = new ObjReorderList<Animation>(null, anims);
                drawer.Draw();
			}
		}
		
		private void CheckFixedTimestamp() {
			if (Time.unscaledDeltaTime < 1f/30f) {
				SetWarning(string.Format("Fixed TimeStamp is {0:0.0}Hz", 1/Time.unscaledDeltaTime));
			}
		}
		
		private void CheckCamera() {
			Camera[] cams = Camera.allCameras;
			for (int i=0; i<cams.Length; i++) {
				if (cams[i].eventMask != 0) {
					SetWarning(cams[0].transform.GetScenePath() + " has the event masks");
				}
				for (int j=i+1; j<cams.Length; j++) {
					if ((cams[i].cullingMask & cams[j].cullingMask) != 0) {
						SetWarning(string.Format("{0}, {1} has duplicate culling masks",
							cams[0].transform.GetScenePath(),
							cams[1].transform.GetScenePath()));
						return;
					}
				}
			}
		}
	}
}