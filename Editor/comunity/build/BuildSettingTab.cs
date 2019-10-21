#define GIT
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.IO;
using System.Text;
using System.Text.Ex;
using mulova.commons;
using mulova.comunity;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using UnityEngine.Ex;

namespace mulova.build
{
    public class BuildSettingTab : EditorTab
    {
        private string version;
        private int versionCode;
        private string clientVersion;
        private string revisionStr;
        private string cdn;
        const string CONFIG_FILE = "Assets/Resources/"+BuildConfig.FILE_NAME+".bytes";
        const string PREBUILD_SCRIPT_PREF = "BuildSetting_PreBuildScript";
        const string PREBUILD_METHOD_PREF = "BuildSetting_PreBuildMethod";
        const string POSTBUILD_METHOD_PREF = "BuildSetting_PostBuildMethod";
        
        public BuildSettingTab(TabbedEditorWindow window) : base("Build Setting", window)
        {
        }
        
        public override void OnEnable()
        {
            if (Application.isPlaying)
            {
                return;
            }
            Reset();
            prebuildScript = EditorPrefs.GetString(PREBUILD_SCRIPT_PREF);
            prebuildMethod = EditorPrefs.GetString(PREBUILD_METHOD_PREF);
            postbuildMethod = EditorPrefs.GetString(POSTBUILD_METHOD_PREF);
            prebuildOutput = null;
        }
        
        public override void OnHeaderGUI()
        {
        }
        
        private bool autoIncrement;
        private static string prebuildScript;
        private static string prebuildMethod;
        private static string postbuildMethod;
        private static ExecOutput prebuildOutput;
        
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Not working in play mode", MessageType.Warning);
                return;
            }
            DrawVersion();
            DrawServer();
            DrawPrebuildScript(); 
            //      DrawPatch();
            
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                string pass = PlayerSettings.keystorePass;
                if (DrawPassword("Keystore password", ref pass))
                {
                    PlayerSettings.keystorePass = pass;
                }
            }
        }
        
        private void DrawVersion()
        {
            EditorUI.BeginContents();
            EditorGUILayout.LabelField("Revision: "+revisionStr, EditorStyles.boldLabel);
            DrawString("Bundle Version", BuildConfig.VERSION, ref version);
            GUI.enabled = !autoIncrement;
            DrawInt("Bundle Version Code", BuildConfig.VERSION_CODE, ref versionCode);
            EditorGUI.indentLevel++;
            if (EditorGUILayoutUtil.Toggle("Auto Increment", ref autoIncrement))
            {
                if (autoIncrement)
                {
                    versionCode++;
                } else
                {
                    versionCode--;
                }
            }
            EditorGUI.indentLevel--;
            GUI.enabled = true;
            DrawString("Client Version", BuildConfig.RES_VERSION, ref clientVersion);
            EditorUI.EndContents();
        }
        
        private void DrawServer()
        {
            EditorUI.BeginContents();
            DrawString("CDN", Cdn.Path, ref cdn);
            EditorUI.EndContents();
        }
        
        private void DrawPrebuildScript()
        {
            if (EditorUI.DrawHeader("Build Scripts"))
            {
                EditorUI.BeginContents();
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayoutUtil.TextField("Prebuild Method", ref prebuildMethod))
                {
                    EditorPrefs.SetString(PREBUILD_METHOD_PREF, prebuildMethod);
                }
                if (GUILayout.Button("Run", GUILayout.ExpandWidth(false)))
                {
                    try
                    {
                        ReflectionUtil.ExecuteMethod(prebuildMethod);
                        EditorUtility.DisplayDialog("Done", "Run Prebuild Method success", "OK");
                    } catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                        EditorUtility.DisplayDialog("Error", ex.Message, "OK");
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUILayoutUtil.TextArea("Prebuild Script", ref prebuildScript, GUILayout.Height(50)))
                {
                    EditorPrefs.SetString(PREBUILD_SCRIPT_PREF, prebuildScript);
                }
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayoutUtil.TextField("Postbuild Method", ref postbuildMethod))
                {
                    EditorPrefs.SetString(POSTBUILD_METHOD_PREF, postbuildMethod);
                }
                if (GUILayout.Button("Run", GUILayout.ExpandWidth(false)))
                {
                    ReflectionUtil.ExecuteMethod(postbuildMethod);
                    EditorUtility.DisplayDialog("Done", "Run PostBuild Method success", "OK");
                }
                EditorGUILayout.EndHorizontal();
                if (prebuildOutput != null)
                {
                    if (prebuildOutput.IsError())
                    {
                        MarkError();
                    }
                    string result = prebuildOutput.GetResult();
                    EditorGUILayoutUtil.TextArea("Prebuild Result", ref result, GUILayout.Height(100));
                    if (prebuildOutput.IsError())
                    {
                        ClearError();
                    }
                }
                EditorUI.EndContents();
            }
        }
        
        
        public override void OnFooterGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply", GUILayout.Height(40)))
            {
                Apply();
            }
            if (GUILayout.Button("Reset", GUILayout.Height(40)))
            {
                Reset();
            }
            EditorGUILayout.BeginVertical();
            if (EditorUserBuildSettings.development)
            {
                MarkError();
            }
            EditorUserBuildSettings.development = EditorGUILayout.Toggle("Development", EditorUserBuildSettings.development);
            ClearError();
            EditorUserBuildSettings.androidBuildSubtarget = (MobileTextureSubtarget)EditorGUILayout.EnumPopup("Texture Compression", EditorUserBuildSettings.androidBuildSubtarget);
            EditorUserBuildSettings.allowDebugging = EditorGUILayout.Toggle("Script Debugging", EditorUserBuildSettings.allowDebugging);
            
            EditorGUILayout.BeginHorizontal();
            string buildPath = EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget);
            if (File.Exists(buildPath)||Directory.Exists(buildPath))
            {
                MarkError();
            }
            EditorGUILayoutUtil.TextField("Build Path", ref buildPath);
            ClearError();
            
            if (GUILayout.Button("Browse", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.iOS:
                    case BuildTarget.StandaloneOSX:
                        buildPath = EditorUtility.OpenFolderPanel("Build Folder", PathUtil.GetDirectory(buildPath), PathUtil.GetLastDirectory(buildPath));
                        break;
                    default:
                        buildPath = EditorUtility.OpenFilePanel("Build Path", PathUtil.GetDirectory(buildPath), "apk");
                        break;
                        
                }
                EditorUserBuildSettings.SetBuildLocation(EditorUserBuildSettings.activeBuildTarget, buildPath);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            bool build = GUILayout.Button("Build", GUILayout.Height(40));
            bool run = GUILayout.Button("Build & Run", GUILayout.Height(40));
            if (build||run)
            {
                BuildOptions buildOptions = BuildOptions.ShowBuiltPlayer|BuildOptions.InstallInBuildFolder;
                if (run)
                {
                    buildOptions |= BuildOptions.AutoRunPlayer;
                }
                if (EditorUserBuildSettings.development)
                {
                    buildOptions |= BuildOptions.Development;
                }
                if (EditorUserBuildSettings.allowDebugging)
                {
                    buildOptions |= BuildOptions.AllowDebugging;
                }
                ReflectionUtil.ExecuteMethod(prebuildMethod);
                AssetDatabase.SaveAssets();
                
                List<string> scenes = new List<string>();
                foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
                {
                    scenes.Add(s.path);
                }
                BuildPipeline.BuildPlayer(scenes.ToArray(), buildPath, EditorUserBuildSettings.activeBuildTarget, buildOptions);
                ReflectionUtil.ExecuteMethod(postbuildMethod);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        public const string PATCH_SRC_FOLDER = "PatchSourceFolder";
        public const string PATCH_DST_FOLDER = "PatchDestinationFolder";
        public const string PATCH_ZIP_PATH = "PatchZipPath";
        
        private void DrawPatch()
        {
            if (EditorUI.DrawHeader("Patch"))
            {
                EditorUI.BeginContents();
                EditorGUILayout.BeginHorizontal();
                string patchSrcDir = EditorPrefs.GetString(PATCH_SRC_FOLDER, "Assets/");
                string patchDstDir = EditorPrefs.GetString(PATCH_DST_FOLDER, "Assets/");
                if (EditorGUILayoutUtil.TextField("Src", ref patchSrcDir))
                {
                    EditorPrefs.SetString(PATCH_SRC_FOLDER, patchSrcDir);
                }
                if (GUILayout.Button("Browse"))
                {
                    patchSrcDir = EditorUtility.OpenFolderPanel("Source Folder", patchSrcDir, "");
                    EditorPrefs.SetString(PATCH_SRC_FOLDER, patchSrcDir);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayoutUtil.TextField("Dst", ref patchDstDir))
                {
                    EditorPrefs.SetString(PATCH_DST_FOLDER, patchDstDir);
                }
                if (GUILayout.Button("Browse"))
                {
                    patchDstDir = EditorUtility.OpenFolderPanel("Destination Folder", patchDstDir, "");
                    EditorPrefs.SetString(PATCH_DST_FOLDER, patchDstDir);
                }
                EditorGUILayout.EndHorizontal();
                string zipPath = EditorPrefs.GetString(PATCH_ZIP_PATH, "patch.zip");
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayoutUtil.TextField("Patch ZipFile", ref zipPath))
                {
                    EditorPrefs.SetString(PATCH_ZIP_PATH, zipPath);
                }
                GUI.enabled = Directory.Exists(patchSrcDir)&&Directory.Exists(patchDstDir)&&!zipPath.IsEmpty();
                if (GUILayout.Button("Generate"))
                {
                    Dictionary<string, FileInfo> srcFiles = GetFiles(patchSrcDir);
                    Dictionary<string, FileInfo> dstFiles = GetFiles(patchDstDir);
                    Dictionary<string, FileInfo> added = new Dictionary<string, FileInfo>(dstFiles);
                    Dictionary<string, FileInfo> removed = new Dictionary<string, FileInfo>(srcFiles);
                    Dictionary<string, FileInfo> changed = new Dictionary<string, FileInfo>();
                    foreach (KeyValuePair<string, FileInfo> pair in dstFiles)
                    {
                        if (removed.ContainsKey(pair.Key))
                        {
                            using (var s1 = srcFiles.Get(pair.Key).Open(FileMode.Open))
                            {
                                using (var s2 = pair.Value.Open(FileMode.Open))
                                {
                                    string srcDigest = s1.ComputeHash();
                                    string dstDigest = s2.ComputeHash();
                                    if (srcDigest != dstDigest)
                                    {
                                        changed.Add(pair.Key, pair.Value);
                                    }
                                }
                            }
                            removed.Remove(pair.Key);
                        }
                    }
                    foreach (string key in srcFiles.Keys)
                    {
                        added.Remove(key);
                    }
                    List<FileInfo> zipFiles = new List<FileInfo>(changed.Values);
                    zipFiles.AddRange(added.Values);
                    if (!zipFiles.IsEmpty())
                    {
                        List<string> removedRelative = new List<string>();
                        foreach (FileInfo f in removed.Values)
                        {
                            removedRelative.Add(PathUtil.GetRelativePath(f.FullName, patchSrcDir));
                        }
                        //                  zip.CreateZipAndList(zipPath, patchDstDir, zipFiles, false, removedRelative);
                        EditorUtil.OpenExplorer(zipPath);
                    } else
                    {
                        EditorUtility.DisplayDialog("Warning", "No difference", "OK");
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorUI.EndContents();
                GUI.enabled = true;
            }
        }
        
        private static Dictionary<string, FileInfo> GetFiles(string dir)
        {
            Dictionary<string, FileInfo> files = new Dictionary<string, FileInfo>();
            foreach (FileInfo f in AssetUtil.ListFiles(dir))
            {
                string path = f.FullName.ToUnixPath();
                if (path.StartsWith(dir))
                {
                    files.Add(PathUtil.GetRelativePath(path, dir), f);
                } else
                {
                    Debug.LogError("not Reachable");
                }
            }
            return files;
        }
        
        public override void OnDisable()
        {
        }
        
        public override void OnChangePlayMode(PlayModeStateChange stateChange)
        {
        }
        
        public override void OnChangeScene(string sceneName)
        {
        }
        
        public override void OnFocus(bool focus)
        {
        }
        
        public override void OnSelected(bool sel)
        {
        }
        
        private bool DrawPassword(string title, ref string pass)
        {
            if (string.IsNullOrEmpty(pass))
            {
                EditorGUILayout.HelpBox(title+" is not set", MessageType.Error);
                SetBackgroundColor(Color.red);
            }
            string pass2 = EditorGUILayout.PasswordField(title, pass);
            ResetBackgroundColor();
            if (pass != pass2)
            {
                pass = pass2;
                return true;
            }
            return false;
        }
        
        private void DrawEnum<T>(string title, T e1, ref T e2) where T:struct, IComparable, IConvertible, IFormattable
        {
            if (!e1.Equals(e2))
            {
                SetBackgroundColor(Color.red);
                EditorGUILayoutUtil.PopupEnum(title, ref e2);
                ResetBackgroundColor();
            } else
            {
                EditorGUILayoutUtil.PopupEnum(title, ref e2);
            }
        }
        
        private void DrawInt(string title, int i1, ref int i2)
        {
            if (i1 != i2)
            {
                MarkError();
                EditorGUILayoutUtil.IntField(title, ref i2);
                ClearError();
            } else
            {
                EditorGUILayoutUtil.IntField(title, ref i2, EditorStyles.toolbarTextField);
            }
        }
        
        private void DrawString(string title, string s1, ref string s2)
        {
            if (s1 != s2)
            {
                MarkError();
                EditorGUILayoutUtil.TextField(title, ref s2, GUILayout.ExpandWidth(true));
                ClearError();
            } else
            {
                EditorGUILayoutUtil.TextField(title, ref s2, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
            }
        }
        
        private void DrawPopup(string title, string s1, ref string s2, string[] list)
        {
            if (list == null)
            {
                DrawString(title, s1, ref s2);
            } else
            {
                if (s1 != s2)
                {
                    SetBackgroundColor(Color.red);
                    EditorGUILayoutUtil.Popup<string>(title, ref s2, list);
                    ResetBackgroundColor();
                } else
                {
                    EditorGUILayoutUtil.Popup<string>(title, ref s2, list);
                }
            }
        }
        
        private Color backupBgColor;
        
        private void SetBackgroundColor(Color color)
        {
            backupBgColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }
        
        private void ResetBackgroundColor()
        {
            GUI.backgroundColor = backupBgColor;
        }
        
        private void MarkError()
        {
            SetBackgroundColor(Color.red);
        }
        
        private void ClearError()
        {
            ResetBackgroundColor();
        }
        
        private void Reset()
        {
            BuildConfig.Reset();
            version = PlayerSettings.bundleVersion;
            versionCode = PlayerSettings.Android.bundleVersionCode;
            clientVersion = BuildConfig.RES_VERSION;
            revisionStr = GetRevision();
            cdn = Cdn.Path;
        }
        
        private void Apply()
        {
            PlayerSettings.Android.bundleVersionCode = versionCode;
            PlayerSettings.bundleVersion = version;
            try
            {
                string code = GenerateCode();
                string dir = PathUtil.GetDirectory(CONFIG_FILE);
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }
                using (StreamWriter writer = new StreamWriter(CONFIG_FILE, false))
                {
                    writer.WriteLine("{0}", code);
                }
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Reset();
            } catch (System.Exception ex)
            {
                string msg = " threw:\n"+ex.ToString();
                Debug.LogError(msg);
                EditorUtility.DisplayDialog("Error when trying to regenerate class", msg, "OK");
            }
        }
        
        private void Append(StringBuilder str, string key, object val)
        {
            str.Append(key).Append("=").Append(val.ToString()).Append("\n");
        }
        
        private string GetRevision()
        {
            ExecOutput result = null;
            #if GIT
            if (Platform.platform.IsWindows())
            {
                result = EditorUtil.ExecuteCommand("sh", "get_revision.sh "+PathUtil.Combine(Application.dataPath, ".."), Encoding.UTF8);
            } else
            {
                result = EditorUtil.ExecuteCommand("sh", "get_revision.sh "+PathUtil.Combine(Application.dataPath, ".."), Encoding.UTF8);
            }
            #else
            if (EditorPlatform.platform.IsWindows()) {
            result = EditorUtil.ExecuteCommand ("svnversion", "-n " + PathUtil.Combine (Application.dataPath, "..\\..\\..\\rainbowchaser"), Encoding.UTF8);
            } else {
            result = EditorUtil.ExecuteCommand("cd", Application.dataPath + " && svnversion", Encoding.UTF8);
            }
            #endif
            return result.stdout;
        }
        
        private string GenerateCode()
        {
            StringBuilder str = new StringBuilder();
            Append(str, nameof(BuildConfig.VERSION), version);
            Append(str, nameof(BuildConfig.VERSION_CODE), versionCode);
            Append(str, nameof(BuildConfig.RES_VERSION), clientVersion);
            Append(str, nameof(BuildConfig.BUILD_TIME), System.DateTime.Now.Ticks);
            Append(str, nameof(BuildConfig.CDN), cdn);
            return str.ToString();
        }
        
        //  [PostProcessScene]
        //  public static void OnPostprocessScene() {
        //      if (prebuildScript.IsNotEmpty()) {
        //          prebuildOutput = EditorUtil.ExecuteCommand(prebuildScript);
        //      } else {
        //          prebuildOutput = null;
        //      }
        //  }
    }
}
