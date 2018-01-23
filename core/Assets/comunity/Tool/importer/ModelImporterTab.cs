using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;



namespace core
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

		public override void OnChangePlayMode() {}
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
					EditorGUIUtil.Toggle("Apply", ref s.apply);
					GUI.enabled = s.apply;
					// ANIMATION
					//					EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
					EditorGUIUtil.Toggle("Import Animation", ref s.importAnimation);
					if (s.importAnimation) {
						EditorGUI.indentLevel += 2;
						EditorGUIUtil.PopupEnum<ModelImporterGenerateAnimations>("Generate Type", ref s.generateAnimations);
						EditorGUIUtil.PopupEnum<ModelImporterAnimationType>("Animation Type", ref s.animationType);
						EditorGUIUtil.PopupEnum<WrapMode>("WrapMode", ref s.animationWrapMode);
						EditorGUIUtil.PopupEnum<ModelImporterAnimationCompression>("Compression", ref s.animationCompression);
						EditorGUIUtil.FloatField("PositionError", ref s.animationPositionError);
						EditorGUIUtil.FloatField("RotationError", ref s.animationRotationError);
						EditorGUIUtil.FloatField("ScaleError", ref s.animationScaleError);
						EditorGUI.indentLevel -= 2;
					}
					
					// MODEL
					EditorGUILayout.LabelField("Model", EditorStyles.boldLabel);
					EditorGUI.indentLevel += 2;
					EditorGUIUtil.Toggle("Readable", ref s.isReadable);
					EditorGUIUtil.Toggle("Add Collider", ref s.addCollider);
					if (s.isBakeIKSupported) {
						EditorGUIUtil.Toggle("Bake IK", ref s.bakeIK);
					}
					EditorGUIUtil.FloatField("Global Scale", ref s.globalScale);
					EditorGUIUtil.PopupEnum<ModelImporterMeshCompression>("Mesh Compression", ref s.meshCompression);
					EditorGUIUtil.Toggle("Import Materials", ref s.importMaterials);
					if (s.importMaterials) {
						EditorGUI.indentLevel += 2;
						EditorGUIUtil.PopupEnum<ModelImporterMaterialName>("Material Name", ref s.materialName);
						EditorGUIUtil.PopupEnum<ModelImporterMaterialSearch>("Material Search", ref s.materialSearch);
						EditorGUI.indentLevel -= 2;
					}
					EditorGUI.indentLevel -= 2;
					
					// UV
					EditorGUILayout.LabelField("UV", EditorStyles.boldLabel);
					EditorGUI.indentLevel += 2;
					EditorGUIUtil.Toggle("Swap UV Channels", ref s.swapUVChannels);
					EditorGUIUtil.Toggle("Generate 2nd UV", ref s.generateSecondaryUV);
					if (s.generateSecondaryUV) {
						EditorGUIUtil.FloatField("2nd UV AngleDistortion", ref s.secondaryUVAngleDistortion);
						EditorGUIUtil.FloatField("2nd UV AreaDistortion", ref s.secondaryUVAreaDistortion);
						EditorGUIUtil.FloatField("2nd UV HardAngle", ref s.secondaryUVHardAngle);
						EditorGUIUtil.FloatField("2nd UV PackMargin", ref s.secondaryUVPackMargin);
					}
					EditorGUI.indentLevel -= 2;
					
					// NORMAL
					EditorGUILayout.LabelField("Normal", EditorStyles.boldLabel);
					EditorGUI.indentLevel += 2;
					EditorGUIUtil.PopupEnum<ModelImporterNormals>("ImporterNormals", ref s.importerNormals);
					if (s.importerNormals != ModelImporterNormals.None) {
						EditorGUIUtil.FloatField("SmoothingAngle", ref s.normalSmoothingAngle);
					}
					EditorGUI.indentLevel -= 2;
					
					// TANGENT
					if (s.isTangentImportSupported) {
						EditorGUILayout.LabelField("Tangent", EditorStyles.boldLabel);
						EditorGUI.indentLevel += 2;
						EditorGUIUtil.PopupEnum<ModelImporterTangents>("ImporterTangents", ref s.importerTangents);
						EditorGUI.indentLevel -= 2;
					}
					EditorGUI.indentLevel += 2;
					if (s.isUseFileUnitsSupported) {
						EditorGUIUtil.Toggle("Use FileUnits", ref s.useFileUnits);
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