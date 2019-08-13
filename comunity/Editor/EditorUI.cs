using UnityEngine;
using UnityEditor;
using mulova.commons;
using System.Text.Ex;

namespace comunity
{
	/// <summary>
	/// From NGUIEditorTools
	/// </summary>
    public class EditorUI {
        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>
        
        static public bool DrawHeader (string text) { return DrawHeader(text, text, false); }
        
        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>
        
        static public bool DrawHeader (string text, string key) { return DrawHeader(text, key, false); }
        
        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>
        
        static public bool DrawHeader (string text, bool forceOn) { return DrawHeader(text, text, forceOn); }
        
        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>
        
        static public bool DrawHeader (string text, string key, bool forceOn)
        {
            bool state = EditorPrefs.GetBool(key, true);
            
            GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(3f);
            
            GUI.changed = false;
            #if UNITY_3_5
            if (state) text = "\u25B2 " + text;
            else text = "\u25BC " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            #else
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25B2 " + text;
            else text = "\u25BC " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            #endif
            if (GUI.changed) EditorPrefs.SetBool(key, state);
            
            GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }
        
        /// <summary>
        /// Begin drawing the content area.
        /// </summary>
        
        static public void BeginContents ()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            #if UNITY_2017_1_OR_NEWER
            EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
            #else
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
            #endif
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }
        
        /// <summary>
        /// End drawing the content area.
        /// </summary>
        
        static public void EndContents ()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }
        
        /// <summary>
        /// Create an undo point for the specified objects.
        /// </summary>
        
        static public void RegisterUndo (string name, params Object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                #if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
                UnityEditor.Undo.RegisterUndo(objects, name);
                #else
                UnityEditor.Undo.RecordObjects(objects, name);
                #endif
                foreach (Object obj in objects)
                {
                    if (obj == null) continue;
                    EditorUtil.SetDirty(obj);
                }
            }
        }
        
        /// <summary>
        /// Convenience function that converts Class + Function combo into Class.Function representation.
        /// </summary>
        
        static public string GetFuncName (object obj, string method)
        {
            if (obj == null) return "<null>";
            if (string.IsNullOrEmpty(method)) return "<Choose>";
            string type = obj.GetType().ToString();
            int period = type.LastIndexOf('.');
            if (period > 0) type = type.Substring(period + 1);
            return type + "." + method;
        }

        public static bool GUIDField<T>(Rect rect, string label, ref string guid) where T:Object {
            T o = null;
            string assetPath = !guid.IsEmpty()? AssetDatabase.GUIDToAssetPath(guid): null;
            if (!assetPath.IsEmpty()) {
                o = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            T newObj = EditorGUI.ObjectField(rect, label, o, typeof(T), false) as T;
            if (newObj != o) {
                guid = newObj!=null? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newObj)): null;
                return true;
            }
            return false;
        }
    }
}

