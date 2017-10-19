using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// Animation target renamer.
/// This script allows animation curves to be moved from one target to another.
/// 
/// Usage:
///     1) Select the gameobject whose curves you wish to move.
///     2) Ensure that the animation to be modified is the default animation on the game object.
///     3) Open the Animation Target Renamer from the Window menu in the Unity UI.
///     4) Change the names in the textboxes on the right side of the window to the names of the objects you wish to move the animations to.
///         NOTE: if the objects do not exist, the original objects will be duplicated and renamed.     
///     5) Select whether or not the old object should be deleted.
///     6) Press Apply.
/// </summary>
using commons;


namespace core
{
    public class AnimationTargetRenamer : EditorWindow
    {
        /// <summary>
        /// The curve data for the animation.
        /// </summary>
        private static AnimationClipCurveData[] curveDatas;
        
        /// <summary>
        /// The names of the original GameObjects.
        /// </summary>
        private static List<string> origObjectPaths;
        private static List<CurveInfo> origCurveInfo;
        
        
        /// <summary>
        /// The names of the target GameObjects.
        /// </summary>
        private static List<string> targetObjectPaths = new List<string> ();
        private static List<CurveInfo> targetCurveInfo = new List<CurveInfo>();
        
        [MenuItem("Window/Animation Target Renamer")]
        public static void OpenWindow ()
        {
            AnimationTargetRenamer window = (AnimationTargetRenamer)GetWindow<AnimationTargetRenamer> ("AnimRenamer");
            window.Show();
        }
        
        private static Object obj;
        
        private AnimationClip clip;
        private void InitClip(AnimationClip clip) {
            this.clip = clip;
            #pragma warning disable 0618
            curveDatas = AnimationUtility.GetAllCurves (clip, true);
            #pragma warning restore 0618
            
            origObjectPaths = new List<string> ();
            origCurveInfo = new List<CurveInfo> ();
            foreach (AnimationClipCurveData curveData in curveDatas) {
                if (!origObjectPaths.Contains (curveData.path)) {
                    origObjectPaths.Add (curveData.path);
                }
                CurveInfo c = new CurveInfo(curveData);
                if (!origCurveInfo.Contains(c)) {
                    origCurveInfo.Add(c);
                }
            }
            
            // if we got here, we have all the data we need to work with,
            // so we should be able to build the UI.
            
            targetObjectPaths.Clear();
            targetCurveInfo.Clear();
            
            foreach (AnimationClipCurveData curveData in curveDatas) {
                if (!targetObjectPaths.Contains (curveData.path)) {
                    // if we haven't already added a target, add it to the list.
                    targetObjectPaths.Add (curveData.path);
                }
                CurveInfo c = new CurveInfo(curveData);
                if (!targetCurveInfo.Contains(c)) {
                    targetCurveInfo.Add(c);
                }
            }
            
        }
        
        Vector2 scrollPos;
        private AnimationClip[] clips;
        
        void OnGUI ()
        {
            // find the game object we're working on.
            Object o = Selection.activeObject;
            GameObject go = o as GameObject;
            if (o != null && obj != o) {
                obj = o;
                if (obj is GameObject) {
                    // If the object isn't set or doesn't have an animation,
                    /// we can't initialize the cuve data, so do nothing.
                    if (go.GetComponent<Animation>() != null || go.GetComponent<Animator>() != null) {
                        // if we haven't looked at the curve data, do so now, 
                        // and initialize the list of original object paths.
                        clips = AnimationUtility.GetAnimationClips(go);
                        if (clips.Length > 0) {
                            InitClip(clips[0]);
                        }
                    }
                } else if (obj is AnimationClip) {
                    InitClip(obj as AnimationClip);
                    clips = new AnimationClip[] { clip };
                }
            }
            
            scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
            if (clips.IsNotEmpty()) {
                if (EditorGUIUtil.Popup("Clip", ref clip, clips)) {
                    InitClip(clip);
                }
                
                if (clip != null) {
                    for (int t=0; t<targetObjectPaths.Count; t++) {
                        targetObjectPaths[t] = EditorGUILayout.TextField (new GUIContent(origObjectPaths[t], origObjectPaths[t]), targetObjectPaths [t]);
                    }
                    
                    if (GUILayout.Button ("Apply")) {
                        
                        // get the actual gameobjects we're working on.
                        List<GameObject> originalObjects = new List<GameObject>();
                        List<GameObject> targetObjects = new List<GameObject>();
                        
                        // rename scene GameObject
                        if (go != null) {
                            for(int i =0; i < origObjectPaths.Count; i++) {
                                Transform t = go.transform.Find(origObjectPaths[i]);
                                if (t != null) {
                                    originalObjects.Add(t.gameObject);
                                    Transform targetTrans = go.transform.Find(targetObjectPaths[i]);
                                    GameObject target = targetTrans != null? targetTrans.gameObject: null;
                                    if(target == null)
                                    {
                                        // If the target object doesn't exist, duplicate the source object,
                                        // and rename it.
                                        target = t.gameObject;
                                        Rename(target, origObjectPaths[i], targetObjectPaths[i]);  
                                    }
                                    targetObjects.Add(target);
                                }
                            }
                        }
                        
                        // Rename
                        Dictionary<string, string> renameMap = new Dictionary<string, string>();
                        for (int i=0; i<origObjectPaths.Count; ++i) {
                            if (origObjectPaths[i] != targetObjectPaths[i]) {
                                renameMap[origObjectPaths[i]] = targetObjectPaths[i];
                            }
                        }
                        foreach (var curveData in curveDatas) {
                            // change type and propertyName
                            for (int i=0; i<origCurveInfo.Count; ++i) {
                                if (!origCurveInfo[i].Equals(targetCurveInfo[i]) && origCurveInfo[i].Is(curveData)) {
                                    targetCurveInfo[i].ApplyProperty(curveData);
                                }
                            }
                            // rename
                            if (renameMap.ContainsKey(curveData.path)) {
                                curveData.path = renameMap[curveData.path];
                            }
                        }
                        
                        // set up the curves based on the new names.
                        clip.ClearCurves ();
                        foreach (var curveData in curveDatas) {
                            clip.SetCurve (curveData.path, curveData.type, curveData.propertyName, curveData.curve);
                        }
                        origObjectPaths.Clear();
                        foreach (AnimationClipCurveData curveData in curveDatas) {
                            if (!origObjectPaths.Contains (curveData.path)) {
                                origObjectPaths.Add (curveData.path);
                            }
                            
                        }
                    }
                    
                    
                    EditorGUI.indentLevel++;
                    for (int t=0; t<targetCurveInfo.Count; t++) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(origCurveInfo[t].path);
                        EditorGUILayout.LabelField(origCurveInfo[t].type);
                        EditorGUILayout.LabelField(origCurveInfo[t].propertyName);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("==>");
                        Color oldColor = GUI.backgroundColor;
                        if (ReflectionUtil.GetType(targetCurveInfo[t].type) == null) {
                            GUI.backgroundColor = Color.red;
                        }
                        targetCurveInfo[t].type = EditorGUILayout.TextField (targetCurveInfo[t].type);
                        GUI.backgroundColor = oldColor;
                        targetCurveInfo[t].propertyName = EditorGUILayout.TextField (targetCurveInfo[t].propertyName);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }
                    EditorGUI.indentLevel--;
                    
                    
                }
            }
            EditorGUILayout.EndScrollView();
            
        }
        
        private void Rename(GameObject gameObject, string srcPath, string dstPath)
        {
            string[] srcNodes = srcPath.Split('/');
            string[] dstNodes = dstPath.Split('/');
            Transform parent = (obj as GameObject).transform;
            for (int i=0; i<dstNodes.Length-1; ++i) {
                Transform dstNode = parent.Find(dstNodes[i]);
                if (dstNode == null) {
                    GameObject go = null;
                    Transform srcNode = parent.Find(srcNodes[i]);
                    if (srcNode != null) {
                        srcNode.name = dstNodes[i];
                        go = srcNode.gameObject;
                    } else {
                        go = new GameObject(dstNodes[i]);
                        go.transform.SetParent(parent, false);
                    }
                    parent = go.transform;
                } else {
                    parent = dstNode;
                }
            }
            gameObject.name = dstNodes[dstNodes.Length - 1];
            gameObject.transform.parent = parent;
        }
        
        private GameObject Instantiate(GameObject obj, string targetPath) {
            GameObject target = GameObject.Instantiate(obj) as GameObject;  
            //      Transform parent = GetParent(obj, targetPath);
            string[] paths = targetPath.Split('/');
            target.name = paths[paths.Length-1];
            target.transform.parent = obj.transform.parent;
            target.transform.SetLocal(obj.transform);
            return target;
        }
        
        private Transform GetParent(GameObject obj, string targetPath) {
            string[] path = targetPath.Split('/');
            Transform parent = obj.transform.parent;
            for (int i=0; i<path.Length-1; ++i) {
                GameObject child = new GameObject(path[i]);
                child.transform.SetParent(parent, false);
                parent = child.transform;
            }
            return parent;
        }
        
        class CurveInfo {
            public string path;
            public string type;
            public string propertyName;
            
            public CurveInfo(AnimationClipCurveData curveData) : this(curveData.path, curveData.type.FullName, curveData.propertyName) {
            }
            public CurveInfo(string path, string type, string propertyName) {
                this.path = path;
                this.type = type;
                int dot = propertyName.LastIndexOf('.');
                if (dot >= 0) {
                    this.propertyName = propertyName.Substring(0, dot);
                } else {
                    this.propertyName = propertyName;
                }
            }
            
            public bool Is(AnimationClipCurveData c) {
                return c.path == path && c.propertyName.StartsWith(propertyName) && c.type.FullName == type;
            }
            
            public void ApplyProperty(AnimationClipCurveData c) {
                c.type = ReflectionUtil.GetType(type);
                int dot = c.propertyName.LastIndexOf('.');
                if (dot >= 0) {
                    c.propertyName = propertyName+c.propertyName.Substring(dot);
                } else {
                    c.propertyName = propertyName;
                }
            }
            
            public override int GetHashCode()
            {
                int code = path.GetHashCode();
                if (type != null) {
                    code += type.GetHashCode();
                }
                if (propertyName != null) {
                    code += propertyName.GetHashCode();
                }
                return code;
            }
            
            public override bool Equals(object obj)
            {
                if (obj == null) {
                    return false;
                }
                if (obj is CurveInfo) {
                    CurveInfo that = obj as CurveInfo;
                    return this.path == that.path && this.type == that.type && this.propertyName == that.propertyName;
                }
                return false;
            }
        }
        
    }
}
