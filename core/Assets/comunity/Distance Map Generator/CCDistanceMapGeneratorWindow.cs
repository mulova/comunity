/*
	Copyright 2012, Jasper Flick
	http://catlikecoding.com/
	Version 1.0.1
	
	1.0.1: changed menu item placement and removed help item
	1.0.0: initial version
	
	Distance Map Generator is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.
	
	Distance Map Generator is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.
	
	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace core
{
    public class CCDistanceMapGeneratorWindow : EditorWindow {
        
        [MenuItem("Tools/unilova/Asset/Distance Map Generator")]
        public static void OpenWindow () {
            EditorWindow.GetWindow<CCDistanceMapGeneratorWindow>(true, "Distance Map Generator");
        }
        
        private static string
        rgbModeKey = "CCDistanceMapGeneratorWindow.rgbMode",
        insideDistanceKey = "CCDistanceMapGeneratorWindow.insideDistance",
        outsideDistanceKey = "CCDistanceMapGeneratorWindow.outsideDistance",
        postProcessDistanceKey = "CCDistanceMapGeneratorWindow.postProcessDistance";
        
        private Texture2D source, destination;
        private float postProcessDistance = 0f;
        private float insideDistance = 3f, outsideDistance = 3f;
        private CCDistanceMapGenerator.RGBMode rgbMode;
        private bool allowSave;
        private bool showAlpha;
        private bool realtime;
        private float scale = 1;
        
        void OnEnable () {
            source = Selection.activeObject as Texture2D;
            rgbMode = (CCDistanceMapGenerator.RGBMode)EditorPrefs.GetInt(rgbModeKey);
            insideDistance = EditorPrefs.GetFloat(insideDistanceKey, 3f);
            outsideDistance = EditorPrefs.GetFloat(outsideDistanceKey, 3f);
            postProcessDistance = EditorPrefs.GetFloat(postProcessDistanceKey);
        }
        
        void OnGUI () {
            //      float menuWidth = 240f;
            //      float menuHeight = 260f;
            bool changed = false;
            GUILayout.BeginHorizontal();
            //      GUILayout.BeginArea(new Rect(2f, 2f, menuWidth, menuHeight));
            GUILayout.BeginVertical(GUILayout.MaxWidth(240));
            Texture2D newSource = (Texture2D)EditorGUILayout.ObjectField("Source Texture", source, typeof(Texture2D), false);
            if(newSource != source){
                source = newSource;
                DestroyImmediate(destination);
                allowSave = false;
                if (source != null) {
                    changed = true;
                }
            }
            if(source == null){
                GUILayout.EndVertical();
                //          GUILayout.EndArea();
                return;
            }
            
            CCDistanceMapGenerator.RGBMode oldMode = rgbMode;
            rgbMode = (CCDistanceMapGenerator.RGBMode)EditorGUILayout.EnumPopup("RGB Mode", rgbMode);
            if(rgbMode != oldMode){
                EditorPrefs.SetInt(rgbModeKey, (int)rgbMode);
                allowSave = false;
                changed = true;
            }
            
            float oldValue = insideDistance;
            insideDistance = EditorGUILayout.FloatField("Inside Distance", insideDistance);
            insideDistance = Mathf.Max(0, insideDistance);
            if(insideDistance != oldValue) {
                EditorPrefs.SetFloat(insideDistanceKey, insideDistance);
                allowSave = false;
                changed = true;
            }
            oldValue = outsideDistance;
            outsideDistance = EditorGUILayout.FloatField("Outside Distance", outsideDistance);
            outsideDistance = Mathf.Max(0, outsideDistance);
            if(outsideDistance != oldValue){
                EditorPrefs.SetFloat(outsideDistanceKey, outsideDistance);
                allowSave = false;
                changed = true;
            }
            oldValue = postProcessDistance;
            postProcessDistance = EditorGUILayout.FloatField("Post-process", postProcessDistance);
            if(postProcessDistance != oldValue){
                EditorPrefs.SetFloat(postProcessDistanceKey, postProcessDistance);
                allowSave = false;
                changed = true;
            }
            
            if(GUILayout.Button("Generate") || (changed && realtime)){
                if(destination == null){
                    destination = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
                    destination.hideFlags = HideFlags.HideAndDontSave;
                }
                CCDistanceMapGenerator.Generate(source, destination, insideDistance, outsideDistance, postProcessDistance, rgbMode);
                destination.Apply();
                allowSave = true;
            }
            
            if(allowSave && GUILayout.Button("Export to PNG")){
                string filePath = EditorUtility.SaveFilePanel(
                    "Save Distance Map",
                    new FileInfo(AssetDatabase.GetAssetPath(source)).DirectoryName,
                    source.name + " distance map",
                    "png");
                if(filePath.Length > 0){
                    File.WriteAllBytes(filePath, destination.EncodeToPNG());
                    PlatformMethods.inst.SetNoBackupFlag(filePath);
                    AssetDatabase.Refresh();
                }
            }
            if(allowSave && GUILayout.Button("Save")){
                string path = AssetDatabase.GetAssetPath(source);
                File.WriteAllBytes(path, destination.EncodeToPNG());
                AssetDatabase.ImportAsset(path);
                source = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(source), typeof(Texture2D)) as Texture2D;
            }
            GUILayout.EndVertical();
            //      GUILayout.EndArea();
            
            GUILayout.BeginVertical();
            showAlpha = GUILayout.Toggle(showAlpha, "Only Show Alpha");
            realtime = GUILayout.Toggle(realtime, "Realtime");
            scale = GUILayout.HorizontalSlider(scale, 0.25f, 3f);
            Rect sliderRect = GUILayoutUtility.GetLastRect();
            
            if(destination != null){
                if (showAlpha) {
                    EditorGUI.DrawTextureAlpha(new Rect(sliderRect.x, sliderRect.y+sliderRect.height, source.width*scale, source.height*scale), destination);
                } else {
                    EditorGUI.DrawTextureTransparent(new Rect(sliderRect.x, sliderRect.y+sliderRect.height, source.width*scale, source.height*scale), destination);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}

