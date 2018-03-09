using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using commons;
using comunity;



namespace convinity {
	class AudioSearchTab : SearchTab<AudioClip>
	{
		private List<AudioFilter> filters = new List<AudioFilter>();

		public AudioSearchTab(TabbedEditorWindow window) : base("AudioClip", window) {
			filters.Add(new AudioNameFilter());
		}
		
		public override void OnHeaderGUI(List<AudioClip> found) {
			EditorGUI.indentLevel += 2;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			foreach (AudioFilter f in filters) {
				f.DrawInspector();
			}
			EditorGUILayout.EndVertical();
			
			if (GUILayout.Button("Search", GUILayout.Width(60), GUILayout.Height(60))) {
				Search();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel -= 2;
		}
		
		protected override void OnInspectorGUI(List<AudioClip> found)
		{
			AudioClip remove = null;
			foreach (AudioClip clip in found) {
				string path = AssetDatabase.GetAssetPath(clip);
				string name = clip.name + (path != null ? " (" + path + ")" : "");
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
					remove = clip;
				}
				EditorGUILayout.LabelField(name, EditorStyles.miniBoldLabel);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.ObjectField(clip, clip.GetType(), false);
			}
			if (remove != null) {
				found.Remove(remove);
			}
		}
		
		public override void OnFooterGUI(List<AudioClip> found) {
			foreach (AudioFilter f in filters) {
				f.Apply(found);
			}
		}
		public override void OnChangePlayMode() {}
		public override void OnChangeScene(string sceneName) {}
		public override void OnSelected(bool sel) {}
		public override void OnFocus(bool focus) {}
		
		protected override List<AudioClip> SearchResource(Object root)
		{
			SetProgress(0);
			List<AudioClip> list = new List<AudioClip>();

            foreach (Object o in SearchAssets(typeof(AudioClip), FileType.Audio)) {
				string path = AssetDatabase.GetAssetPath(o);
				if (!string.IsNullOrEmpty(path)) {
					AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
					if (importer != null) {
						bool pass = true;
						foreach (AudioFilter f in filters) {
							pass &= f.Filter(importer, (AudioClip)o);
						}
						if (pass) {
							list.Add((AudioClip)o);
						}
					}
				}
			}
//			SetProgress(1);
			return list;
		}

		class AudioNameFilter : AudioFilter {
			private string audioName;
			public AudioNameFilter(): base() {}
			protected override bool FilterImpl(AudioImporter importer, AudioClip clip)
			{
				return clip.name.Contains(audioName);
			}
			protected override void DrawInspectorImpl()
			{
				EditorGUIUtil.TextField("Name Filter", ref audioName);
			}
			public override void Apply(List<AudioClip> found) {}
		}
		
		abstract class AudioFilter {
			private bool enabled;
			
			public bool Filter(AudioImporter importer, AudioClip clip) {
				if (!enabled) {
					return true; 
				}
				return FilterImpl(importer, clip);
			}
			protected abstract bool FilterImpl(AudioImporter importer, AudioClip clip);
			public void DrawInspector() {
				EditorGUILayout.BeginHorizontal();
				EditorGUIUtil.Toggle(null, ref enabled, GUILayout.Width(40));
				DrawInspectorImpl();
				EditorGUILayout.EndHorizontal();
			}
			protected abstract void DrawInspectorImpl();
			public abstract void Apply(List<AudioClip> found);
		}
	}
}
