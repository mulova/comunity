using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using comunity;



namespace convinity
{
	public class ModelImporterTab : EditorTab
	{
		private static List<ModelImport> settings = new List<ModelImport>();
		
		public ModelImporterTab(TabbedEditorWindow window) : base("Model", window) {}
		
		public override void OnEnable()
		{
			settings = ModelImport.Load();
		}
		
		public override void OnDisable() {}

		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		
		public override void OnHeaderGUI() {
		}
		
		public override void OnInspectorGUI()
		{
			ModelImport remove = null;
			foreach (ModelImport s in settings) {
				if (EditorUI.DrawHeader(s.path)) {
					EditorUI.BeginContents();
					GUILayout.BeginHorizontal();
					Object obj = AssetDatabase.LoadAssetAtPath(s.path, typeof(Object));
					if (EditorGUILayoutUtil.ObjectField<Object>(ref obj, false)) {
						if (obj != null) {
							s.path = AssetDatabase.GetAssetPath(obj);
						} else {
							s.path = string.Empty;
						}
					}
					if (GUILayout.Button("-", GUILayout.Width(20))) {
						remove = s;
					}
					EditorGUILayoutUtil.Toggle("Apply", ref s.apply);
					GUI.enabled = s.apply;
					// ANIMATION
					//					EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
					EditorGUILayoutUtil.Toggle("Import Animation", ref s.importAnimation);
					if (s.importAnimation) {
						EditorGUI.indentLevel += 2;
						EditorGUILayoutUtil.PopupEnum<ModelImporterGenerateAnimations>("Generate Type", ref s.generateAnimations);
						EditorGUILayoutUtil.PopupEnum<ModelImporterAnimationType>("Animation Type", ref s.animationType);
						EditorGUILayoutUtil.PopupEnum<WrapMode>("WrapMode", ref s.animationWrapMode);
						EditorGUILayoutUtil.PopupEnum<ModelImporterAnimationCompression>("Compression", ref s.animationCompression);
						EditorGUILayoutUtil.FloatField("PositionError", ref s.animationPositionError);
						EditorGUILayoutUtil.FloatField("RotationError", ref s.animationRotationError);
						EditorGUILayoutUtil.FloatField("ScaleError", ref s.animationScaleError);
						EditorGUI.indentLevel -= 2;
					}
					
					// MODEL
					EditorGUILayout.LabelField("Model", EditorStyles.boldLabel);
					EditorGUI.indentLevel += 2;
					EditorGUILayoutUtil.Toggle("Readable", ref s.isReadable);
					EditorGUILayoutUtil.Toggle("Add Collider", ref s.addCollider);
					if (s.isBakeIKSupported) {
						EditorGUILayoutUtil.Toggle("Bake IK", ref s.bakeIK);
					}
					EditorGUILayoutUtil.FloatField("Global Scale", ref s.globalScale);
					EditorGUILayoutUtil.PopupEnum<ModelImporterMeshCompression>("Mesh Compression", ref s.meshCompression);
					EditorGUILayoutUtil.Toggle("Import Materials", ref s.importMaterials);
					if (s.importMaterials) {
						EditorGUI.indentLevel += 2;
						EditorGUILayoutUtil.PopupEnum<ModelImporterMaterialName>("Material Name", ref s.materialName);
						EditorGUILayoutUtil.PopupEnum<ModelImporterMaterialSearch>("Material Search", ref s.materialSearch);
						EditorGUI.indentLevel -= 2;
					}
					EditorGUI.indentLevel -= 2;
					
					// UV
					EditorGUILayout.LabelField("UV", EditorStyles.boldLabel);
					EditorGUI.indentLevel += 2;
					EditorGUILayoutUtil.Toggle("Swap UV Channels", ref s.swapUVChannels);
					EditorGUILayoutUtil.Toggle("Generate 2nd UV", ref s.generateSecondaryUV);
					if (s.generateSecondaryUV) {
						EditorGUILayoutUtil.FloatField("2nd UV AngleDistortion", ref s.secondaryUVAngleDistortion);
						EditorGUILayoutUtil.FloatField("2nd UV AreaDistortion", ref s.secondaryUVAreaDistortion);
						EditorGUILayoutUtil.FloatField("2nd UV HardAngle", ref s.secondaryUVHardAngle);
						EditorGUILayoutUtil.FloatField("2nd UV PackMargin", ref s.secondaryUVPackMargin);
					}
					EditorGUI.indentLevel -= 2;
					
					// NORMAL
					EditorGUILayout.LabelField("Normal", EditorStyles.boldLabel);
					EditorGUI.indentLevel += 2;
					EditorGUILayoutUtil.PopupEnum<ModelImporterNormals>("ImporterNormals", ref s.importerNormals);
					if (s.importerNormals != ModelImporterNormals.None) {
						EditorGUILayoutUtil.FloatField("SmoothingAngle", ref s.normalSmoothingAngle);
					}
					EditorGUI.indentLevel -= 2;
					
					// TANGENT
					if (s.isTangentImportSupported) {
						EditorGUILayout.LabelField("Tangent", EditorStyles.boldLabel);
						EditorGUI.indentLevel += 2;
						EditorGUILayoutUtil.PopupEnum<ModelImporterTangents>("ImporterTangents", ref s.importerTangents);
						EditorGUI.indentLevel -= 2;
					}
					EditorGUI.indentLevel += 2;
					if (s.isUseFileUnitsSupported) {
						EditorGUILayoutUtil.Toggle("Use FileUnits", ref s.useFileUnits);
					}
					EditorGUI.indentLevel -= 2;
					GUI.enabled = true;
					EditorUI.EndContents();
					GUILayout.EndHorizontal();
				}
			}
			if (remove != null) {
				settings.Remove(remove);
			}
		}
		
		public override void OnFooterGUI() {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("+")) {
				settings.Add(new ModelImport());
			}
			if (GUILayout.Button("Load")) {
				settings = ModelImport.Load();
				CustomAssetPostprocessor.modelSettings = settings;
			}
			if (GUILayout.Button("Save")) {
				ModelImport.Save(settings);
			}
			EditorGUILayout.EndHorizontal();
		}

		public override void OnFocus(bool focus) {}
		public override void OnSelected(bool sel) {}
	}
}