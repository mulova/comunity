using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using mulova.comunity;

namespace convinity {
	
	[System.Serializable]
	public class AudioImport {
		public bool apply = true;
		public string path;
		public bool forceToMono;
		public const string USER_DATA_KEY = "AudioImportApplied";
		
		public AudioImport() {
		}
		
		public void Apply(AudioImporter importer) {
			if (apply 
				&& path!=null
				&& importer.assetPath.Contains(path)
				&& (importer.userData==null || !importer.userData.Contains(USER_DATA_KEY))
				) {
				importer.forceToMono = forceToMono;
				
				if (string.IsNullOrEmpty(importer.userData)) {
					importer.userData = USER_DATA_KEY;
				} else {
					importer.userData += ","+USER_DATA_KEY;
				}
			}
		}
		
		private const string STORE_PATH = "importer/.audio_importer_settings";
		private static string GetStorePath() {
			return EditorAssetUtil.GetProjectFileFullPath(STORE_PATH);
		}

		public static List<AudioImport> Load() {
			if (File.Exists(STORE_PATH))
			{
				BinarySerializer serializer = new BinarySerializer(GetStorePath(), FileAccess.Read);
				List<AudioImport> settings = serializer.Deserialize<List<AudioImport>>();
				if (settings == null) {
					settings = new List<AudioImport>();
				}
				return settings;
			} else
			{
				return new List<AudioImport>();
			}
		}
		
		public static void Save(List<AudioImport> settings) {
			CustomAssetPostprocessor.audioSettings = null;
			BinarySerializer serializer = new BinarySerializer(GetStorePath(), FileAccess.Write);
			serializer.Serialize(settings);
			serializer.Close();
		}
	}
}