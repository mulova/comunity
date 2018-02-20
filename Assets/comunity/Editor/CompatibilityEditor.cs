using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using commons;

namespace comunity
{
	public static class CompatibilityEditor
	{
		public const TextureImporterType TEXTURE_IMPORTER_TYPE =
			#if UNITY_5_5_OR_NEWER
			TextureImporterType.Default;    
			#else
			TextureImporterType.Advanced;    
			#endif
		
		public static void SetDirty(Object o)
		{
			if (Application.isPlaying)
			{
				return;
			}
			UnityEditor.EditorUtility.SetDirty(o);
			#if UNITY_5_3_OR_NEWER
			if (AssetDatabase.IsSubAsset(o) || AssetDatabase.GetAssetPath(o).IsNotEmpty())
			{
			return;
			}
			if (o is Component && AssetDatabase.IsSubAsset(((o as Component).gameObject)))
			{
			return;
			}
			UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
			#endif
		}
	}
}


