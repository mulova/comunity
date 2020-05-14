#if FULL
using UnityEngine;
using UnityEditor;
using System.IO;

namespace etc
{
	public class LZFWindow : EditorWindow {
		
		[MenuItem("Tools/unilova/Asset/LZF")]
		static void Init()
		{
			EditorWindow window = EditorWindow.GetWindow(typeof(LZFWindow));
			window.Show();
		}
		
		private static TextAsset assetToCompress;
		private static TextAsset assetToDecompress;
		void OnGUI() {
			EditorGUILayout.BeginHorizontal();
			EditorGUIEx.ObjectField<TextAsset>(ref assetToCompress, false);
			GUI.enabled = assetToCompress != null;
			if (GUILayout.Button("Compress")) {
				string src = AssetDatabase.GetAssetPath(assetToCompress);
				string dst = StringUtil.InsertSuffix(src, "_compress");
				dst = AssetDatabase.GenerateUniqueAssetPath(dst);
				
				byte[] compressed = CLZF.Compress(assetToCompress.bytes);
				FileStream os = new FileStream(dst, FileMode.CreateNew);
				os.Write(compressed, 0, compressed.Length);
				os.Close();
				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			}
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
			EditorGUILayout.BeginHorizontal();
			EditorGUIEx.ObjectField<TextAsset>(ref assetToDecompress, false);
			GUI.enabled = assetToDecompress != null;
			if (GUILayout.Button("Decompress")) {
				string src = AssetDatabase.GetAssetPath(assetToDecompress);
				string dst = StringUtil.InsertSuffix(src, "_decompress");
				dst = AssetDatabase.GenerateUniqueAssetPath(dst);
				
				byte[] decompressed = CLZF.Decompress(assetToCompress.bytes);
				FileStream os = new FileStream(dst, FileMode.CreateNew);
				os.Write(decompressed, 0, decompressed.Length);
				os.Close();
				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			}
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
		}
	}
	
}

#endif