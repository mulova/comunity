using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

using math.ex;
using comunity;

namespace convinity {

	public class TexImporterTab : EditorTab {
		private static List<TexImport> settings = new List<TexImport>();
		
		public TexImporterTab(TabbedEditorWindow window) : base("Texture", window) {}
		
		public override void OnEnable() {
			settings = TexImport.Load();
		}
		
		public override void OnDisable() {}
		
		public override void OnChangePlayMode() {}
		public override void OnChangeScene(string sceneName) {}
		
		public override void OnHeaderGUI() { }
		
		public override void OnInspectorGUI() {
			TexImport remove = null;
			foreach (TexImport s in settings) {
				if (EditorUI.DrawHeader(s.path)) {
					EditorUI.BeginContents();
					GUILayout.BeginHorizontal();
					Object obj = AssetDatabase.LoadAssetAtPath(s.path, typeof(Object));
					if (EditorGUIUtil.ObjectField<Object>(ref obj, false)) {
						if (obj != null) {
							s.path = AssetDatabase.GetAssetPath(obj);
						} else {
							s.path = string.Empty;
						}
					}
					if (GUILayout.Button("-", GUILayout.Width(20))) {
						remove = s;
					}
					GUILayout.EndHorizontal();
					TextureImporterSettings setting = s.setting;
					EditorGUIUtil.Toggle("Apply", ref s.apply);
					GUI.enabled = s.apply;
                    TextureImporterType[] imTypes = new TextureImporterType[] { // remove deprecated
                        TextureImporterType.Default,
                        TextureImporterType.Sprite,
                        TextureImporterType.GUI,
                        TextureImporterType.SingleChannel,
                        TextureImporterType.NormalMap,
                        TextureImporterType.Lightmap,
                        TextureImporterType.Cursor,
                        TextureImporterType.Cookie,
                    };
                    var texType = s.setting.textureType;
                    if (EditorGUIUtil.PopupEnum<TextureImporterType>("Import Type", ref texType, imTypes)) {
                        setting.ApplyTextureType(texType);
					}
                    EditorGUIUtil.PopupEnum<TextureImporterFormat>("Texture Format", ref s.format);

                    if (s.setting.textureType == TextureImporterType.Sprite) {
						DrawSpriteMenu(s);
					} else {
						setting.wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Texture WrapMode", setting.wrapMode);
						setting.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", setting.filterMode);
						setting.readable = EditorGUILayout.Toggle("Readable", setting.readable);
						setting.mipmapEnabled = EditorGUILayout.Toggle("Generate Mip Maps", setting.mipmapEnabled);
						if (setting.mipmapEnabled) {
							EditorGUI.indentLevel += 2;
							setting.borderMipmap = EditorGUILayout.Toggle("Border MipMaps", setting.borderMipmap);
							setting.mipmapFilter = (TextureImporterMipFilter)EditorGUILayout.EnumPopup("MipMap Filtering", setting.mipmapFilter);
							setting.fadeOut = EditorGUILayout.Toggle("FadeOut MipMaps", setting.fadeOut);
							setting.mipmapBias = EditorGUILayout.FloatField("MipMap Bias", setting.mipmapBias);
							setting.mipmapFadeDistanceStart = EditorGUILayout.IntField("MipMap Fade Distance Start", setting.mipmapFadeDistanceStart);
							setting.mipmapFadeDistanceEnd = EditorGUILayout.IntField("MipMap Fade Distance End", setting.mipmapFadeDistanceEnd);
							EditorGUI.indentLevel -= 2;
						}
						DrawSpriteMenu(s);
						setting.alphaIsTransparency = EditorGUILayout.Toggle("AlphaIsTransparency", setting.alphaIsTransparency);
						setting.aniso = MathUtil.Clamp(EditorGUILayout.IntField("Aniso Level", setting.aniso), 1, 9);
						setting.convertToNormalMap = EditorGUILayout.Toggle("Convert To NormalMap", setting.convertToNormalMap);
						setting.generateCubemap = (TextureImporterGenerateCubemap)EditorGUILayout.EnumPopup("Generate CubeMap", setting.generateCubemap);
						setting.heightmapScale = EditorGUILayout.FloatField("HightMap Scale", setting.heightmapScale);
						setting.normalMapFilter = (TextureImporterNormalFilter)EditorGUILayout.EnumPopup("Filter Mode", setting.normalMapFilter);
						setting.npotScale = (TextureImporterNPOTScale)EditorGUILayout.EnumPopup("Non Power of 2", setting.npotScale);
						int maxSize = s.maxTexSize;
						if (EditorGUIUtil.Popup<int>("Max Size", ref maxSize, new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096})) {
                            s.maxTexSize = maxSize;
						}
					}
					EditorUI.EndContents();
					GUI.enabled = true;
				}
			}
			if (remove != null) {
				settings.Remove(remove);
			}
		}

		private void DrawSpriteMenu(TexImport s) {
			s.spriteImportMode = (SpriteImportMode)EditorGUILayout.EnumPopup("Sprite Mode", s.spriteImportMode);
				EditorGUI.indentLevel++;
				s.setting.spritePixelsPerUnit = EditorGUILayout.FloatField("Pixels Per Units", s.setting.spritePixelsPerUnit);
				s.setting.spritePivot = EditorGUILayout.Vector2Field("Pivot", s.setting.spritePivot);
				EditorGUI.indentLevel--;
		}
		
		public override void OnFooterGUI() {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("+")) {
				settings.Add(new TexImport());
			}
			if (GUILayout.Button("Load")) {
				settings = TexImport.Load();
				CustomAssetPostprocessor.texSettings = settings;
			}
			if (GUILayout.Button("Save")) {
				TexImport.Save(settings);
			}
			EditorGUILayout.EndHorizontal();
		}
		
		public override void OnFocus(bool focus) {}
		public override void OnSelected(bool sel) {}
	}
}