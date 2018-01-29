using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;



using System.Text;

namespace comunity {
	[CustomEditor(typeof(AudioTrigger))]
	public class AudioTriggerInspector : Editor {
		private AudioTriggerInspectorImpl impl;
		private AudioTrigger trigger;
		private AudioGroup[] groups;
		private AudioGroup selectedGroup;
//		private string[] clips = new string[0];
		
		void OnEnable() {
			impl = new AudioTriggerInspectorImpl(target as AudioTrigger);
		}

		public override void OnInspectorGUI() {
			impl.DrawManageTableGUI();
			impl.OnInspectorGUI();
		}
	}
}
