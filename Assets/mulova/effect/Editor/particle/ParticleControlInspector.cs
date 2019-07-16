using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using comunity;

namespace effect {
	[CustomEditor(typeof(ParticleControl)), CanEditMultipleObjects]
	public class ParticleControlInspector : Editor
	{
		private ParticleControl element;
		void OnEnable() {
			element = target as ParticleControl;
			Search();
		}

		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if (GUILayout.Button("Search All")) {
				Search();
			}
		}

		private void Simulate() {
			foreach (ParticleSystem s in element.particles) {
				s.Simulate(element.duration, false, true);
			}
		}
		
		private void Search() {
			if (element.duration <= 0) {
				element.duration = element.EstimateDuration();
				EditorUtility.SetDirty(element);
			}
		}
	}
	
}
