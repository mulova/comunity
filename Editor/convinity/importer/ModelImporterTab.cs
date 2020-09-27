using System.Collections.Generic;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
				using (var area = new EditorUI.ContentArea(s.path))
                {
					if (area)
                    {
						GUILayout.BeginHorizontal();
						Object obj = AssetDatabase.LoadAssetAtPath(s.path, typeof(Object));
						if (EditorGUILayoutEx.ObjectField<Object>(ref obj, false)) {
							if (obj != null) {
								s.path = AssetDatabase.GetAssetPath(obj);
							} else {
								s.path = string.Empty;
							}
						}
						if (GUILayout.Button("-", GUILayout.Width(20))) {
							remove = s;
						}
						EditorGUILayoutEx.Toggle("Apply", ref s.apply);
						GUI.enabled = s.apply;
						// ANIMATION
						//					EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
						EditorGUILayoutEx.Toggle("Import Animation", ref s.importAnimation);
						if (s.importAnimation) {
							EditorGUI.indentLevel += 2;
							EditorGUILayoutEx.PopupEnum<ModelImporterGenerateAnimations>("Generate Type", ref s.generateAnimations);
							EditorGUILayoutEx.PopupEnum<ModelImporterAnimationType>("Animation Type", ref s.animationType);
							EditorGUILayoutEx.PopupEnum<WrapMode>("WrapMode", ref s.animationWrapMode);
							EditorGUILayoutEx.PopupEnum<ModelImporterAnimationCompression>("Compression", ref s.animationCompression);
							EditorGUILayoutEx.FloatField("PositionError", ref s.animationPositionError);
							EditorGUILayoutEx.FloatField("RotationError", ref s.animationRotationError);
							EditorGUILayoutEx.FloatField("ScaleError", ref s.animationScaleError);
							EditorGUI.indentLevel -= 2;
						}
					
						// MODEL
						EditorGUILayout.LabelField("Model", EditorStyles.boldLabel);
						EditorGUI.indentLevel += 2;
						EditorGUILayoutEx.Toggle("Readable", ref s.isReadable);
						EditorGUILayoutEx.Toggle("Add Collider", ref s.addCollider);
						if (s.isBakeIKSupported) {
							EditorGUILayoutEx.Toggle("Bake IK", ref s.bakeIK);
						}
						EditorGUILayoutEx.FloatField("Global Scale", ref s.globalScale);
						EditorGUILayoutEx.PopupEnum("Mesh Compression", ref s.meshCompression);
						EditorGUILayoutEx.PopupEnum("Import Materials", ref s.materialImportMode);
						if (s.materialImportMode != ModelImporterMaterialImportMode.None) {
							EditorGUI.indentLevel += 2;
							EditorGUILayoutEx.PopupEnum<ModelImporterMaterialName>("Material Name", ref s.materialName);
							EditorGUILayoutEx.PopupEnum<ModelImporterMaterialSearch>("Material Search", ref s.materialSearch);
							EditorGUI.indentLevel -= 2;
						}
						EditorGUI.indentLevel -= 2;
					
						// UV
						EditorGUILayout.LabelField("UV", EditorStyles.boldLabel);
						EditorGUI.indentLevel += 2;
						EditorGUILayoutEx.Toggle("Swap UV Channels", ref s.swapUVChannels);
						EditorGUILayoutEx.Toggle("Generate 2nd UV", ref s.generateSecondaryUV);
						if (s.generateSecondaryUV) {
							EditorGUILayoutEx.FloatField("2nd UV AngleDistortion", ref s.secondaryUVAngleDistortion);
							EditorGUILayoutEx.FloatField("2nd UV AreaDistortion", ref s.secondaryUVAreaDistortion);
							EditorGUILayoutEx.FloatField("2nd UV HardAngle", ref s.secondaryUVHardAngle);
							EditorGUILayoutEx.FloatField("2nd UV PackMargin", ref s.secondaryUVPackMargin);
						}
						EditorGUI.indentLevel -= 2;
					
						// NORMAL
						EditorGUILayout.LabelField("Normal", EditorStyles.boldLabel);
						EditorGUI.indentLevel += 2;
						EditorGUILayoutEx.PopupEnum<ModelImporterNormals>("ImporterNormals", ref s.importerNormals);
						if (s.importerNormals != ModelImporterNormals.None) {
							EditorGUILayoutEx.FloatField("SmoothingAngle", ref s.normalSmoothingAngle);
						}
						EditorGUI.indentLevel -= 2;
					
						// TANGENT
						if (s.isTangentImportSupported) {
							EditorGUILayout.LabelField("Tangent", EditorStyles.boldLabel);
							EditorGUI.indentLevel += 2;
							EditorGUILayoutEx.PopupEnum<ModelImporterTangents>("ImporterTangents", ref s.importerTangents);
							EditorGUI.indentLevel -= 2;
						}
						EditorGUI.indentLevel += 2;
						if (s.isUseFileUnitsSupported) {
							EditorGUILayoutEx.Toggle("Use FileUnits", ref s.useFileUnits);
						}
						EditorGUI.indentLevel -= 2;
						GUI.enabled = true;
						GUILayout.EndHorizontal();
					}
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