using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using mulova.comunity;
using mulova.unicore;

namespace convinity
{
	[System.Serializable]
	public class ModelImport
	{
		public bool apply = true;
		public string path;
		public const string USER_DATA_KEY = "ModelImportApplied";
		public bool addCollider;
		public ModelImporterAnimationCompression animationCompression;
		public float animationPositionError;
		public float animationRotationError;
		public float animationScaleError;
		public ModelImporterAnimationType animationType;
		public WrapMode animationWrapMode;
		public bool bakeIK;
		public ModelImporterClipAnimation[] clipAnimations;
		public ModelImporterGenerateAnimations generateAnimations;
		public bool generateSecondaryUV;
		public float globalScale = 1;
		public bool importAnimation;
		public ModelImporterMaterialImportMode materialImportMode;
		public bool isBakeIKSupported;
		public bool isReadable;
		public bool isTangentImportSupported;
		public bool isUseFileUnitsSupported;
		public ModelImporterMaterialName materialName;
		public ModelImporterMaterialSearch materialSearch;
		public ModelImporterMeshCompression meshCompression;
		public ModelImporterNormals importerNormals;
		public float normalSmoothingAngle;
		public bool optimizeMeshPolygons;
		public bool optimizeMeshVertices;
		public string[] referencedClips;
		public float secondaryUVAngleDistortion;
		public float secondaryUVAreaDistortion;
		public float secondaryUVHardAngle;
		public float secondaryUVPackMargin;
		public ModelImporterTangents importerTangents;
		public bool swapUVChannels;
		public string[] transformPaths;
		public bool useFileUnits;

		public ModelImport()
		{
		}

		public void Apply(ModelImporter importer)
		{
			if (apply
			    &&path != null
			    &&importer.assetPath.Contains(path)
			    &&(importer.userData == null||!importer.userData.Contains(USER_DATA_KEY)))
			{
				
				importer.addCollider = addCollider;
				importer.animationCompression = animationCompression;
				importer.animationPositionError = animationPositionError;
				importer.animationRotationError = animationRotationError;
				importer.animationScaleError = animationScaleError;
				importer.animationType = animationType;
				importer.animationWrapMode = animationWrapMode;
				importer.bakeIK = bakeIK;
//		importer.clipAnimations;
				importer.generateAnimations = generateAnimations;
				importer.generateSecondaryUV = generateSecondaryUV;
				importer.globalScale = globalScale;
				importer.importAnimation = importAnimation;
				importer.materialImportMode = materialImportMode;
				importer.isReadable = isReadable;
				importer.materialName = materialName;
				importer.materialSearch = materialSearch;
				importer.meshCompression = meshCompression;
				importer.importNormals = importerNormals;
				importer.normalSmoothingAngle = normalSmoothingAngle;
				importer.optimizeMeshPolygons = optimizeMeshPolygons;
				importer.optimizeMeshVertices = optimizeMeshVertices;
//		importer.referencedClips;
				importer.secondaryUVAngleDistortion = secondaryUVAngleDistortion;
				importer.secondaryUVAreaDistortion = secondaryUVAreaDistortion;
				importer.secondaryUVHardAngle = secondaryUVHardAngle;
				importer.secondaryUVPackMargin = secondaryUVPackMargin;
				importer.swapUVChannels = swapUVChannels;
				importer.importTangents = importerTangents;
//		importer.transformPaths;
				importer.useFileUnits = useFileUnits;
				
				if (string.IsNullOrEmpty(importer.userData))
				{
					importer.userData = USER_DATA_KEY;
				} else
				{
					importer.userData += ","+USER_DATA_KEY;
				}
			}
		}

		private const string STORE_PATH = "importer/.model_importer_settings";

		private static string GetStorePath()
		{
			return EditorAssetUtil.GetProjectFileFullPath(STORE_PATH);
		}

		public static List<ModelImport> Load()
		{
			if (File.Exists(STORE_PATH))
			{
				BinarySerializer serializer = new BinarySerializer(GetStorePath(), FileAccess.Read);
				List<ModelImport> settings = serializer.Deserialize<List<ModelImport>>();
				if (settings == null)
				{
					settings = new List<ModelImport>();
				}
				return settings;
			} else
			{
				return new List<ModelImport>();
			}
		}

		public static void Save(List<ModelImport> settings)
		{
			CustomAssetPostprocessor.modelSettings = null;
			BinarySerializer serializer = new BinarySerializer(GetStorePath(), FileAccess.Write);
			serializer.Serialize(settings);
			serializer.Close();
		}
	}
}