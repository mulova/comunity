using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using comunity;
using commons;
using System.Text.Ex;

namespace convinity {
	
	[System.Serializable]
	public class TexImport : IComparable<TexImport> {
		public bool apply = true;
		public SpriteImportMode spriteImportMode = SpriteImportMode.None;
		public string path;
        public TextureImporterSettings setting = new TextureImporterSettings();
        public TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings();
        public TextureImporterFormat format = TextureImporterFormat.Automatic;
        public int maxTexSize = 1024;
		public const string USER_DATA_KEY = "TexImportApplied";
		
		public TexImport() {
			setting.aniso = 1;
			setting.spritePixelsPerUnit = 1;
			setting.spritePivot = new Vector2(0.5f, 0.5f);
            setting.ApplyTextureType(TextureImporterType.Default);
			setting.wrapMode = TextureWrapMode.Clamp;
			setting.filterMode = FilterMode.Bilinear;
		}
		
		public bool ApplyTo(TextureImporter importer) {
			if (apply 
			    && path!=null
			    && importer.assetPath.Contains(path)
			    && (importer.userData==null || !importer.userData.Contains(USER_DATA_KEY))
			    ) {
                importer.SetTextureSettings(setting);
                importer.SetMaxTextureSize(maxTexSize);
                importer.SetFormat(format);
				
				if (string.IsNullOrEmpty(importer.userData)) {
					importer.userData = USER_DATA_KEY;
				} else {
					importer.userData += ","+USER_DATA_KEY;
				}
				return true;
			}
			return false;
		}
		
		private const string STORE_PATH = "importer/.tex_importer_settings";
		private static string GetStorePath() {
			return EditorAssetUtil.GetProjectFileFullPath(STORE_PATH);
		}
		
		public static List<TexImport> Load() {
			if (File.Exists(STORE_PATH))
			{
				BinarySerializer serializer = new BinarySerializer(GetStorePath(), FileAccess.Read);
				List<TexImport> settings = serializer.Deserialize<List<TexImport>>();
                if (settings != null)
                {
                    return settings;
                }
            }
            return new List<TexImport>();
		}
		
		public static void Save(List<TexImport> settings) {
			CustomAssetPostprocessor.texSettings = null;
			BinarySerializer serializer = new BinarySerializer(GetStorePath(), FileAccess.Write);
			serializer.Serialize(settings);
            settings.Sort();
			serializer.Close();
		}

		public int CompareTo(TexImport that)
		{
			if (path.IsNotEmpty()) 
			{
				if (that.path.IsNotEmpty())
				{
					return -path.CompareTo(that.path);
				} else
				{
					return -1;
				}
			} else if (that.path.IsNotEmpty())
			{
				return 1;
			} else {
				return 0;
			}
		}
	}
}