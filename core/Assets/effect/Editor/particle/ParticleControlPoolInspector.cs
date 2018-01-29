using UnityEngine;
using UnityEditor;

namespace effect {
	[CustomEditor(typeof(ParticlePool))]
	public class ParticleControlPoolInspector : Editor {
		private ParticlePool pool;
		public const string EVENT_ID = "event/particle_event_id";
		
		void OnEnable() {
			pool = (ParticlePool) target;
		}
		
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			
			if (Application.isPlaying && GUILayout.Button("Play")) {
				pool.Play();
			}
		}
	}
	
}