using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using commons;
using comunity;
using UnityEngine.Ex;

namespace convinity {
	class TexSearchTab : SearchTab<Texture>
	{
		private List<TexFilter> filters = new List<TexFilter>();
		
		public TexSearchTab(TabbedEditorWindow window) : base("Texture", window) {
			filters.Add(new TexNameFilter());
			filters.Add(new TexTypeFilter());
			filters.Add(new TexFormatFilter());
			filters.Add(new TexFilterFilter());
			filters.Add(new TexSizeFilter());
			filters.Add(new TexCompressionFilter());
			filters.Add(new TexReadableFilter());
			filters.Add(new TexMipmapFilter());
			filters.Add(new TexNPOTFilter());
		}

		public override void OnSelected(bool sel) {}
		public override void OnFocus(bool focus) {}
		
		public override void OnHeaderGUI(List<Texture> found) {
			EditorGUI.indentLevel += 2;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			foreach (TexFilter f in filters) {
				f.DrawInspector();
			}
			EditorGUILayout.EndVertical();
			
			if (GUILayout.Button("Search (Max100)", GUILayout.Width(60), GUILayout.Height(60))) {
				Search();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel -= 2;
		}
		
		protected override List<Texture> SearchResource()
		{
			List<Texture> list = new List<Texture>();
			foreach (Object o in SearchAssets(typeof(Texture), FileType.Image, FileType.Prefab)) {
				string path = AssetDatabase.GetAssetPath(o);
				if (!string.IsNullOrEmpty(path)) {
					TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
					if (importer != null) {
						bool pass = true;
						foreach (TexFilter f in filters) {
							pass &= f.Filter(importer, (Texture)o);
						}
						if (pass) {
							list.Add((Texture)o);
						}
					}
				}
			}
			return list;
		}
		
		protected override void OnInspectorGUI(List<Texture> found)
		{
			Vector2 MAX_SIZE = new Vector2(256, 256);
			Texture remove = null;
			foreach (Object o in found) {
				Texture tex = o as Texture;
				string path = AssetDatabase.GetAssetPath(tex);
				string name = o.name + (path != null ? " (" + path + ")" : "");
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
					remove = tex;
				}
				EditorGUILayout.LabelField(name, EditorStyles.miniBoldLabel);
				EditorGUILayout.EndHorizontal();
				Vector2 size = GUIUtil.Resize(new Vector2(tex.width, tex.height), MAX_SIZE);
				EditorGUILayout.ObjectField(o, o.GetType(), false, GUILayout.Width(size.x), GUILayout.Height(size.y));
			}
			if (remove != null) {
				found.Remove(remove);
			}
		}
		
		public override void OnFooterGUI(List<Texture> found) {}
		
		abstract class TexFilter {
			private string name;
			private bool enabled;
			
			public TexFilter(string name) {
				this.name = name;
			}
			public bool Filter(TextureImporter importer, Texture tex) {
				if (!enabled) {
					return true; 
				}
				return FilterImpl(importer, tex);
			}
			protected abstract bool FilterImpl(TextureImporter importer, Texture tex);
			public void DrawInspector() {
				EditorGUILayout.BeginHorizontal();
				EditorGUIUtil.Toggle(null, ref enabled, GUILayout.Width(40));
				DrawInspectorImpl();
				EditorGUILayout.EndHorizontal();
			}
			protected abstract void DrawInspectorImpl();

			public override string ToString ()
			{
				return name;
			}
		}

		class TexNameFilter: TexFilter {
			private string texName;
			public TexNameFilter(): base("Name") {}
			protected override bool FilterImpl (TextureImporter importer, Texture tex) {
				return tex.name.Contains(texName);
			}
			protected override void DrawInspectorImpl () {
				EditorGUIUtil.TextField("Name Filter", ref texName);
			}
		}

		class TexTypeFilter: TexFilter {
            private TextureImporterType texImportType = TextureImporterType.Default;
			public TexTypeFilter(): base("ImporterType") {}
			protected override bool FilterImpl (TextureImporter importer, Texture tex) {
				return importer.textureType == texImportType;
			}
			protected override void DrawInspectorImpl () {
				EditorGUIUtil.PopupEnum<TextureImporterType>("Importer Type", ref texImportType);
			}
		}
		class TexFormatFilter: TexFilter {
#pragma warning disable 0618
			private TextureImporterFormat texImportFormat = TextureImporterFormat.AutomaticTruecolor;
#pragma warning restore 0618
			public TexFormatFilter(): base("ImporterFormat") {}
			protected override bool FilterImpl (TextureImporter importer, Texture tex) {
				return importer.GetFormat() == texImportFormat;
			}
			protected override void DrawInspectorImpl () {
				EditorGUIUtil.PopupEnum<TextureImporterFormat>("Importer Format", ref texImportFormat);
			}
		}
		class TexFilterFilter: TexFilter {
			private FilterMode filterMode = FilterMode.Bilinear;
			public TexFilterFilter(): base("Filter") {}
			protected override bool FilterImpl (TextureImporter importer, Texture tex) {
				return importer.filterMode == filterMode;
			}
			protected override void DrawInspectorImpl () {
				EditorGUIUtil.PopupEnum<FilterMode>(null, ref filterMode);
			}
		}
		class TexSizeFilter: TexFilter {
			private int texSize = 2048;
			public TexSizeFilter(): base("Size") {}
			protected override bool FilterImpl (TextureImporter importer, Texture tex) {
				return tex.width >= texSize || tex.height >= texSize;
			}
			protected override void DrawInspectorImpl () {
				EditorGUIUtil.Popup<int>(null, ref texSize, new int[] { 512, 1024, 2048, 4096 });
			}
		}
		
		class TexCompressionFilter: TexFilter {
			private int compressed;
			public TexCompressionFilter(): base("Compression") {}
			protected override bool FilterImpl (TextureImporter importer, Texture tex) {
				return importer.compressionQuality <= compressed;
			}
			protected override void DrawInspectorImpl () {
				EditorGUIUtil.IntField("Compressed", ref compressed);
			}
		}
		class TexReadableFilter: TexFilter {
			private bool texReadable = true;
			public TexReadableFilter(): base("Readable") {}
			protected override bool FilterImpl (TextureImporter importer, Texture tex) {
				return importer.isReadable == texReadable;
			}
			protected override void DrawInspectorImpl () {
				EditorGUIUtil.Toggle("Readable", ref texReadable);
			}
		}
		class TexMipmapFilter: TexFilter {
			private bool texMipmap = true;
			public TexMipmapFilter(): base("Mipmap") {}
			protected override bool FilterImpl (TextureImporter importer, Texture tex) {
				return importer.mipmapEnabled == texMipmap;
			}
			protected override void DrawInspectorImpl () {
				EditorGUIUtil.Toggle("Mipmap", ref texMipmap);
			}
		}
		class TexNPOTFilter: TexFilter {
			private bool npot;
			public TexNPOTFilter(): base("NPOT") {}
			protected override bool FilterImpl (TextureImporter importer, Texture tex) {
				if (npot) {
					return true;
				}
				return tex.IsPOT();
			}
			protected override void DrawInspectorImpl () {
				EditorGUIUtil.Toggle("NPOT", ref npot);
			}
		}

		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
	}
}
