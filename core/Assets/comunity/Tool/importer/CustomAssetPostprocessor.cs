using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

namespace core {
	public class CustomAssetPostprocessor : AssetPostprocessor {
		public static List<TexImport> texSettings;
		public static List<AudioImport> audioSettings;
		public static List<ModelImport> modelSettings;
		
		void OnPreprocessTexture() {
			if (texSettings == null) {
				texSettings = TexImport.Load();
			}
	    	TextureImporter importer = assetImporter as TextureImporter;
			foreach (TexImport i in texSettings) {
				if (i.ApplyTo(importer))
				{
					break;
				}
			}
	    }
		
		void OnPreprocessAudio() {
			if (audioSettings == null) {
				audioSettings = AudioImport.Load();
			}
	    	AudioImporter importer = assetImporter as AudioImporter;
			foreach (AudioImport i in audioSettings) {
				i.Apply(importer);
			}
	    }
	}
}

namespace core {
	public class AssetImporterWindow : TabbedEditorWindow {
		protected override void CreateTabs() {
			AddTab(new TexImporterTab(this));
			AddTab(new ModelImporterTab(this));
			AddTab(new AudioImporterTab(this));
		}
		
		[MenuItem("Tools/unilova/Asset/ImporterSetting")]
		public static void ShowAssetImporterWindow() 
		{
			// Get existing open window or if none, make a new one:
			AssetImporterWindow window = EditorWindow.GetWindow (typeof(AssetImporterWindow)) as AssetImporterWindow;
			window.titleContent = new GUIContent("AssetImporter");
			window.Show ();
		}
	}
	
}