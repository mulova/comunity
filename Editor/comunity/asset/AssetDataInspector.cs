using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

using System;



namespace mulova.comunity {
	public class AssetDataInspector
	{
//		private AssetRefInspector cache = new AssetRefInspector();
//		private AssetDB db;
//		public ScanDelegate scan;
//		public ClearDelegate clear;
//		private static bool showRef = true;
//		
//		public AssetDataInspector(AssetDB db, string varName, Type enumClass) : base(db, varName) { 
//			SetTitle("DB");
//			SetEnum(enumClass);
//			this.db = db;
//			scan = Scan;
//			clear = Clear;
//			InitEnumTypeSelector(typeof(Enum), "enumType");
//		}
//		
//		protected override bool OnInspectorGUI(AssetData info, int i)
//		{
//			float BTN_WIDTH = 25;
//			float width = GetWidth()-BTN_WIDTH;
//			bool changed = false;
//			
//			EditorGUILayout.BeginVertical();
//			EditorGUILayout.BeginHorizontal();
//			changed |= DrawEnum(ref info.key, GUILayout.MinWidth(width*0.3F));
//			
//			if (showRef) {
//				if (cache.OnInspectorGUI<Object>(ref info.srcPath)) {
//					if (info.IsUnderResources()) {
//						HardLink(info, false);
//					}
//					changed = true;
//				}
//				if (!info.IsUnderResources()) {
//					changed |= EditorGUIEx.PopupEnum<AssetType> (null, ref info.type, GUILayout.MinWidth (width * 0.2F));
//					bool link = info.reference != null;
//					if (GUILayout.Button (link ? "O" : "X", EditorStyles.toolbarButton, GUILayout.MinWidth (BTN_WIDTH))) {
//						HardLink (info, !link);
//						changed = true;
//					}
//				}
//			} else {
//				changed |= EditorGUIEx.PopupEnum<AssetType>(null, ref info.type, GUILayout.MinWidth(width*0.2F));
//			}
//			EditorGUILayout.EndHorizontal();
//			if (!showRef) {
//				EditorGUILayout.SelectableLabel(info.srcPath);
//			}
//			EditorGUILayout.EndVertical();
//			return changed;
//		}
//		
//		private FileType fileType = FileType.Null;
//		private AssetType assetType = AssetType.Asset;
//		protected override bool DrawHeader() {
//			bool changed = base.DrawHeader();
//			//				float width = GetWidth();
//			if (DrawEnumTypeSelector()) {
//				changed = true;
//			}
//			if (EditorGUIEx.PopupEnum<AssetType>(null, ref assetType, GUILayout.ExpandWidth(false))) {
//				SetAssetType(assetType);
//				changed = true;
//			}
//			string linkTitle = db.IsReferenced() ? "Linked(O)":"UnLinked(X)";
//			if (GUILayout.Button(linkTitle, EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
//				HardLink(!db.IsReferenced());
//				changed = true;
//			}
//			return changed;
//		}
//		
//		protected override bool DrawFooter() {
//			bool changed = false;
//			EditorGUILayout.BeginHorizontal();
//			EditorGUIEx.PopupEnum<FileType>(null, ref fileType);
//			if (GUILayout.Button("Scan", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
//				scan();
//				showRef = true;
//				HardLink(db.IsReferenced());
//				EditorUtil.SetDirty(db);
//				changed = true;
//			}
//			if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) && EditorUtility.DisplayDialog("Warning", "Clear DB?", "OK", "Cancel")) {
//				clear();
//				EditorUtil.SetDirty(db);
//				changed = true;
//			}
//			EditorGUILayout.EndHorizontal();
//			EditorGUIEx.Toggle("Show Ref", ref AssetDataArrInspector.showRef);
//			return changed;
//		}
//		
//		/**
//			 * @param folderPath canonical path 혹은 Assets/로 시작하는 path
//			 */
//		public bool Scan() {
//			bool found = false;
//			string folder = EditorPrefs.GetString("AssetDBScanFolder_"+db.name, "Assets/");
//			folder = EditorUtility.OpenFolderPanel("Scan Folder", folder, "");
//			
//			if (!string.IsNullOrEmpty(folder)) {
//				folder = EditorAssetUtil.GetProjectRelativePath(folder);
//				if (fileType == FileType.Null) {
//					found |= ScanFolder(folder, FileType.Prefab);
//					found |= ScanFolder(folder, FileType.Asset);
//					found |= ScanFolder(folder, FileType.Material);
//					found |= ScanFolder(folder, FileType.Image);
//					found |= ScanFolder(folder, FileType.Audio);
//					found |= ScanFolder(folder, FileType.Text);
//				} else {
//					found |= ScanFolder(folder, fileType);
//				}
//				EditorPrefs.SetString("AssetDBScanFolder_"+db.name, folder);
//			}
//			return found;
//		}
//		
//		private void OnScanSelected(object obj, string[] items, int i) {
//		}
//		
//		public void Clear() {
//			cache.Clear();
//			db.Clear();
//		}
//		
//		public void Check() {
//			StringBuilder str = new StringBuilder();
//			for (int i=0; i<db.assets.Length; i++) {
//				string path = db.GetBundleUrl(db.assets[i].key);
//				string protocol = "file:///";
//				if (path.StartsWith(protocol, StringComparison.OrdinalIgnoreCase)) {
//					path = path.Substring(protocol.Length);
//				}
//				if (!File.Exists(path)) {
//					str.Append(db.assets[i].key).Append(" - ").Append(path).Append("\n");
//					break;
//				}
//			}
//			
//			if (str.Length > 0) {
//				EditorUtility.DisplayDialog("Missing Buildle", str.ToString(), "OK");
//			} else {
//				EditorUtility.DisplayDialog("Success", "Valid", "OK");
//			}
//		}
//		
//		public void SetAssetType(AssetType type) {
//			for (int i=0; i<db.assets.Length; i++) {
//				db.assets[i].type = type;
//			}
//			EditorUtil.SetDirty(db);
//		}
//		
//		/// <summary>
//		/// Link or unlink asset reference. 
//		/// Related to the inclusion of assets in build
//		/// </summary>
//		/// <param name="link">If set to <c>true</c> link.</param>
//		public void HardLink(bool link) {
//			for (int i=0; i<db.assets.Length; i++) {
//				HardLink(db.assets[i], link);
//			}
//			db.SetReferenced(link);
//			EditorUtil.SetDirty(db);
//		}
//		
//		private void HardLink(AssetData a, bool link) {
//			if (link) {
//				a.reference = AssetDatabase.LoadAssetAtPath("Assets/"+a.srcPath, typeof(Object));
//			} else {
//				a.reference = null;
//			}
//		}
//		
//		public bool ScanFolder(string folderPath, FileType fileType) {
//			bool changed = false;
//			foreach (string ext in fileType.GetExt()) {
//				changed |= ScanFolder(folderPath, "*"+ext);
//			}
//			return changed;
//		}
//		
//		/**
//			 * @param folderPath canonical path 혹은 Assets/로 시작하는 path
//			 */
//		public bool ScanFolder(string folderPath, string wildcard) {
//			string[] paths = EditorAssetUtil.ListAssetPaths(folderPath, wildcard);
//			for (int i=0; i<paths.Length; i++) {
//				Object go = cache.GetCache(paths[i]);
//				if (go != null) {
//					Put(go.name, EditorAssetUtil.GetAssetRelativePath(paths[i]));
//				}
//			}
//			return paths.Length > 0;
//		}
//		
//		public void Put(string key, string val) {
//			db.Put (key, val);
//		}
//		
//		public int GetIndex(string key) {
//			return db.GetIndex(key);
//		}
	}
}
