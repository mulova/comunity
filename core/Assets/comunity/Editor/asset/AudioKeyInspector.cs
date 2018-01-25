
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace core {
	public class AudioKeyInspector {
		
		private AudioGroup[] groups;
		private AudioGroup selectedGroup;
		private string[] keys;

		public AudioKeyInspector() {
			groups = GameObject.FindObjectsOfType<AudioGroup>();
			if (groups.Length > 0) {
				SetGroup(groups[0]);
			}
		}
		
		private void SetGroup(AudioGroup group) {
			selectedGroup = group;
			keys = new List<string>(selectedGroup.clips).ToArray();
		}

		public void DrawGroup() {
			if (selectedGroup == null) {
				EditorGUILayout.HelpBox("No AudioGroup is found", MessageType.Warning);
			} else {
				if (EditorGUIUtil.Popup("Audio Group", ref selectedGroup, groups, GUILayout.ExpandWidth(false))) {
					SetGroup(selectedGroup);
				}
			}
		}

		public bool DrawKey(string title, ref string key) {
			if (selectedGroup == null) {
				return false;
			} else {
				return EditorGUIUtil.Popup<string>(title, ref key, keys);
			}
		}
	}
}
