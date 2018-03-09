//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Reflection;
using commons;
using UnityEditor.SceneManagement;
using System.Collections;

namespace comunity
{
	
    /**
	 * Editor에서만 사용되는 Asset 관련 Util methods
	 */
    public static class EditorAssetUtil
    {

        /**
		 * @return animation clip이 없으면 Length 0 array를 반환한다.
		 */
        public static AnimationClip[] GetAnimationClips(Object obj)
        {
            if (obj is AnimationClip)
            {
                return new AnimationClip[] { (AnimationClip)Selection.activeObject };
            } else if (obj is GameObject&&((GameObject)obj).GetComponent<Animation>() != null)
            {
                Animation ani = ((GameObject)obj).GetComponent<Animation>();
                return AnimationUtility.GetAnimationClips(ani.gameObject);
            }
            return new AnimationClip[0];
        }

        /**
		 * object list 중에서 animation clip들을 찾아서 추가한다.
		 * 동일한 이름이 여러개 있을 경우 editable animation이 우선권을 갖는다.
		 * @param clips array의 element는 animation clip이거나 animation clip을 포함한 개체
		 */
        public static void AddAnimationClips(Animation anim, params Object[] clips)
        {
            foreach (Object o in clips)
            {
                if (IsFolder(o))
                {
                    AddFolderAnimations(anim, AssetDatabase.GetAssetPath(o));
                } else
                {
                    AddAnimationClip(anim, o);
                }
            }
        }

        public static bool IsFolder(Object o)
        {
            string path = AssetDatabase.GetAssetPath(o);
            return path.IsNotEmpty()&&Directory.Exists(GetAssetFileFullPath(path));
        }

        /**
		 * @return bool true if the method call succeeds
		 */
        private static bool AddAnimationClip(Animation anim, Object o)
        {
            if (o is AnimationClip)
            {
                if (anim.GetClip(o.name) == null)
                {
                    anim.AddClip((AnimationClip)o, o.name);
                    return true;
                }
            } else if (o is GameObject)
            {
                GameObject go = (GameObject)o;
                Animation a = go.GetComponent<Animation>();
                if (a != null)
                {
                    foreach (AnimationClip c in AnimationUtility.GetAnimationClips(a.gameObject))
                    {
                        if (anim.GetClip(c.name) == null)
                        {
                            anim.AddClip(c, c.name);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /**
		 * @return AssetDatabase.GetAssetPath()로 부터 얻어지는 path 혹은 Assets/ 아래의 relative path
		 */
        public static string GetAssetFileFullPath(string assetPath)
        {
            string relativePath = assetPath;
            if (assetPath == "Assets"
                ||assetPath == "Assets/")
            {
                relativePath = "";
            } else if (assetPath.StartsWith("Assets/"))
            {
                relativePath = assetPath.Substring("Assets/".Length);
            } else if (assetPath.StartsWith(Application.dataPath))
            {
                return assetPath;
            }
            return PathUtil.Combine(Application.dataPath, relativePath);
        }

        public static string GetProjectFileFullPath(string assetPath)
        {
            if (assetPath.IsURL())
            {
                return assetPath;
            }
            return PathUtil.Combine(GetProjPath(), assetPath);
        }

        public static string GetProjectRelativePath(string fullPath)
        {
            string path = fullPath.ToUnixPath();
            if (path.StartsWith(GetProjPath()))
            {
                return path.Substring(GetProjPath().Length);
            }
            return path;
        }

        private static string projPath;

        public static string GetProjPath()
        {
            DirectoryInfo dir = new DirectoryInfo(Application.dataPath);
            projPath = dir.Parent.FullName.ToUnixPath();
            if (!projPath.EndsWith("/", StringComparison.Ordinal))
            {
                projPath += "/";
            }
            return projPath;
        }

        /**
		 * assetPath 아래에서 wildcard filter를 거친 asset들을 반환한다.
		 * @assetPath AssetDatabase.GetAssetPath()로 부터 얻어지는 path 혹은 Assets/ 아래의 relative path
		 * @wildcard filtering정보. null이면 모두를 반환한다. 예) *.fbx
		 * @return Type에 해당되는 object들을 반환한다.
		 */
        public static T[] ListAssets<T>(string assetPath, FileType fileType) where T: Object
        {
            List<T> list = new List<T>();
            foreach (string p in ListAssetPaths(assetPath, fileType))
            {
                T t = AssetDatabase.LoadAssetAtPath(p, typeof(T)) as T;
                if (t != null)
                {
                    list.Add(t);
                }
            }
            return list.ToArray();
        }

        /**
		 * Folder가 선택될 경우 아래의 Asset들을 선택해서 반환한다.
		 */
        public static Object[] ListAssets(Object[] assets)
        {
            Object[] oldSel = Selection.objects;
			
            Selection.objects = assets;
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            Selection.objects = oldSel;
            return selection;
        }

		
        /**
		 * 특정 폴더 아래에 저장된 모든 animation을 저장한다.
		 * 저장시 동일한 이름의 animation은 .anim 만을 추가한다.
		 * @param folder Assets/ 디렉토리 아래의 상대경로
		 */
        public static void AddFolderAnimations(Animation anim, string folder)
        {
            string path = GetAssetFileFullPath(folder);
            Dictionary<string, FileInfo> selected = new Dictionary<string, FileInfo>();
            FileInfo[] info = AssetUtil.ListFiles(path, "*.anim");
            foreach (FileInfo f in info)
            {
                string key = f.Name.Substring(0, f.Name.Length-5);
                selected[key] = f;
            }
            info = AssetUtil.ListFiles(path, "*.fbx");
            foreach (FileInfo f in info)
            {
                int s = f.Name.IndexOf("@");
                if (s < 0)
                {
                    s = 0;
                } else
                {
                    s++;
                }
                string key = f.Name.Substring(s, f.Name.Length-4-s);
                if (selected.ContainsKey(key) == false)
                {
                    selected[key] = f;
                }
            }
			
            foreach (string name in selected.Keys)
            {
                FileInfo file = selected[name];
                Object o = AssetDatabase.LoadAssetAtPath(GetProjectRelativePath(file.FullName), typeof(Object)) as Object;
                if (o != null)
                {
                    AddAnimationClips(anim, o);
                }
            }
        }

        public static T LoadReference<T>(string guid) where T:Object
        {
            Object o = GetObject(guid);
            if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T)))
            {
                return ((GameObject)o).GetComponent(typeof(T)) as T;
            } else
            {
                return o as T;
            }

        }

        public static List<T> LoadReferencesFromFile<T>(string filePath) where T:Object
        {
            List<T> objList = new List<T>();
            if (File.Exists(filePath))
            {
                string[] guidList = File.ReadAllLines(filePath);
                if (guidList != null)
                {
                    foreach (string guid in guidList)
                    {
                        T obj = LoadReference<T>(guid);
                        if (obj != null)
                        {
                            objList.Add(obj);
                        }
                    }
                }
            }
            return objList;
        }

        public static Object GetObject(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                return null;
            }
            string assetPath = AssetDatabase.GUIDToAssetPath(objectId);
            if (!string.IsNullOrEmpty(assetPath))
            {
                return AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)) as Object;
            } else
            {
                Transform t = TransformUtil.Search(objectId);
                if (t != null)
                {
                    return t.gameObject;
                }
            }
            return null;
        }

        public static List<string> SaveReferences<T>(string path, IEnumerable<T> objList) where T:Object
        {
            List<string> strList = new List<string>();
            foreach (T o in objList)
            {
                if (o != null)
                {
                    strList.Add(GetObjectId(o));
                }
            }
            try
            {
                string dir = PathUtil.GetDirectory(path);
                if (dir.IsNotEmpty()&&!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllLines(path, strList.ToArray());
            } catch (Exception ex)
            {
                Debug.LogError(ex.Message+"\n"+ex.StackTrace);
                EditorUtility.DisplayDialog("Error", "Cannot save references", "OK");
            }
            return strList;
        }

        /// <summary>
        /// Get Object Id.
        /// </summary>
        /// <returns>
        /// GUID for the assets. Scene Path for the scene objects.
        /// </returns>
        /// <param name='o'>
        /// The Object
        /// </param>
        public static string GetObjectId(Object o, out bool asset)
        {
            string str = AssetDatabase.GetAssetPath(o);
            if (!string.IsNullOrEmpty(str))
            {
                asset = true;
                return AssetDatabase.AssetPathToGUID(str);
            } else
            {
                asset = false;
                if (o is GameObject)
                {
                    return (o as GameObject).transform.GetScenePath();
                } else if (o is Component)
                {
                    return (o as Component).transform.GetScenePath();
                }
                return o.ToString();
            }
        }

        public static string GetObjectId(Object o)
        {
            bool asset = false;
            return GetObjectId(o, out asset);
        }

		
        public static void SetTextureImportProperty(string assetPath, 
                                              TextureImporterType textureType, TextureImporterNPOTScale scale, bool mipmap, bool readable)
        {
            AssetDatabase.StartAssetEditing();
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = textureType;
                importer.isReadable = readable;
                importer.mipmapEnabled = mipmap;
                importer.npotScale = scale;
                AssetDatabase.StopAssetEditing();
                AssetDatabase.ImportAsset(assetPath);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }

        /**
		 * texture로부터 특정 color가 있는 좌표를 읽어온다.
		 */
        public static List<Vector2> GetColorPixels(string assetPath, Color color)
        {
            SetTextureImportProperty(assetPath, CompatibilityEditor.TEXTURE_IMPORTER_TYPE, TextureImporterNPOTScale.None, false, true);
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));
            Color[] colors = tex.GetPixels(0);
            List<Vector2> coord = new List<Vector2>();
			
            int i = 0;
            for (int h = tex.height-1; h >= 0; h--)
            {
                for (int w = 0; w < tex.width; w++)
                {
                    if (colors[i].Equals(color))
                    {
                        coord.Add(new Vector2(w, h));
                    }
                    i++;
                }
            }
            return coord;
        }

        public static ScriptableObject CreateScriptableObject(string typeName, string assetName, Object parent)
        {
            string path = "";
            if (parent != null)
            {
                path = AssetDatabase.GetAssetPath(parent);
            }
            if (path.Length > 0)
            {
                path = path+"/"+assetName+".asset";
            } else
            {
                path = "Assets/"+assetName+".asset";
            }
            return CreateScriptableObject(typeName, path);
        }

        public static ScriptableObject CreateScriptableObject(string typeName, string path)
        {
            string dir = PathUtil.GetDirectory(GetAssetFileFullPath(path));
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
			
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            Debug.Log("Create Asset: "+path);
            ScriptableObject obj = ScriptableObject.CreateInstance(typeName);
            AssetDatabase.CreateAsset(obj, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            obj = AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject)) as ScriptableObject;
            EditorGUIUtility.PingObject(obj);
            return obj;
        }

        /// <summary>
        /// Creates the scriptable object.
        /// </summary>
        /// <returns>The scriptable object.</returns>
        /// <param name="path">project relative path. Starts with Assets/</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T CreateScriptableObject<T>(string path) where T:ScriptableObject
        {
            string dir = PathUtil.GetDirectory(GetAssetFileFullPath(path));
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
			
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            Debug.Log("Create Asset: "+path);
            T obj = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(obj, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            obj = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
            EditorGUIUtility.PingObject(obj);
            return obj;
        }

        /**
		 * @param root directory object in Asset folder
		 */
        public static List<T> DepthFirstTraversal<T>(string root, FileType fileType, Func<Object, T> func) where T: Object
        {
            List<T> result = new List<T>();
            string[] paths = ListAssetPaths(root, fileType);
            foreach (string childPath in paths)
            {
                Object childObj = AssetDatabase.LoadAssetAtPath(childPath, typeof(Object));
                if (childObj != null)
                {
                    T res = func(childObj);
                    if (res != null)
                    {
                        result.Add(res);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Creates the asset.
        /// </summary>
        /// <returns>The asset.</returns>
        /// <param name="path">path Application.dataPath의 하위 경로. Windows의 경우 Assets/ 아래의 경로</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T CreateAsset<T>(string path) where T: UnityEngine.Object
        {
            string fullPath = PathUtil.Combine(Application.dataPath, path);
            FileStream output = File.Create(fullPath);
            output.Close();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            T asset = AssetDatabase.LoadAssetAtPath(GetProjectRelativePath(fullPath), typeof(T)) as T;
            return asset;
        }

        /**
		 * Asset을 Build하여 rootPath/Target명/path 에 AssetBundle을 저장한다.
		 */
        public static void BuildAssetBundle(string basePath, string relativePath, BuildTarget target, Object obj, string name)
        {
            BuildAssetBundle(basePath, relativePath, target, new Object[] { obj }, new string[] { name });
        }

        public static void BuildAssetBundle(string basePath, string relativePath, BuildTarget target, Object[] objs, string[] names)
        {
            string fullPath = PathUtil.Combine(basePath, target.ToRuntimePlatform(), relativePath);
            BuildAssetBundle(fullPath, target, objs, names);
        }

        public static void BuildAssetBundle(string fullPath, BuildTarget buildTarget, Object obj, string name)
        {
            BuildAssetBundle(fullPath, buildTarget, new Object[] { obj }, new string[] { name });
        }

        /**
		 * Asset을 Build하여 rootPath/Target명/path 에 AssetBundle을 저장한다.
		 */
        public static void BuildAssetBundle(string fullPath, BuildTarget buildTarget, Object[] objs, string[] names)
        {
            string dir = PathUtil.GetDirectory(fullPath);
            string filename = Path.GetFileName(fullPath);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
			
            List<GameObject> editorOnly = new List<GameObject>();
            List<Object> newObjs = new List<Object>();
            List<string> newNames = new List<string>();
            List<string> removePaths = new List<string>();
            for (int i = 0; i < objs.Length; i++)
            {
                Object o = objs[i];
                newObjs.Add(o);
                newNames.Add(names[i]);
                if (o is GameObject)
                {
                    // erase EditorOnly tag
                    GameObject go = (GameObject)o;
                    if (go.tag == "EditorOnly")
                    {
                        go.tag = null;
                        editorOnly.Add(go);
                    }
                }
            }
            #pragma warning disable 0618
            BuildAssetBundleOptions option = BuildAssetBundleOptions.CollectDependencies;
            string exportPath = PathUtil.Combine(dir, filename);

            StringBuilder str = new StringBuilder();
            objs = newObjs.ToArray();
            BuildPipeline.BuildAssetBundle(objs[0], objs, exportPath, option, buildTarget);
            str.Append("AssetBundle '").Append(exportPath).Append("[").Append(buildTarget.ToRuntimePlatform().ToString()).Append("]\n");
            str.Append("\tBuild List (Key-Value) - ");
            for (int i = 0; i < objs.Length; i++)
            {
                str.Append("\t").Append(names[i]).Append(": ").Append(objs[i].ToString()).Append("\n");
            }
            Debug.Log(str.ToString());
            #pragma warning restore 0618
            // Restore 'EditorOnly' tag
            foreach (GameObject go in editorOnly)
            {
                go.tag = "EditorOnly";
            }
            foreach (string temp in removePaths)
            {
                AssetDatabase.DeleteAsset(temp);
            }
        }

        /// <summary>
        /// Lists the asset paths.
        /// </summary>
        /// <returns>The asset paths. Project path에 상대적인 path를 반환한다.</returns>
        /// <param name="assetPath">AssetDatabase.GetAssetPath()로 부터 얻어지는 path 혹은 Assets/ 아래의 relative path</param>
        public static string[] ListAssetPaths(string assetPath, FileType fileType, bool assetsRelative = false)
        {
            string fullPath = GetAssetFileFullPath(assetPath);
            List<string> paths = new List<string>();

            foreach (string ext in fileType.GetExt())
            {
                FileInfo[] files = AssetUtil.ListFiles(fullPath, "*"+ext);
                for (int i = 0; i < files.Length; i++)
                {
                    if (fileType == FileType.All && files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    string srcPath = files[i].FullName.ToUnixPath();
                    int index = srcPath.IndexOf(Application.dataPath);
                    if (assetsRelative)
                    {
                        paths.Add(srcPath.Substring(index+Application.dataPath.Length+1));
                    } else
                    {
                        paths.Add(Path.Combine("Assets/", srcPath.Substring(index+Application.dataPath.Length+1)));
                    }
                }
            }
            return paths.ToArray();
        }

        /// <summary>
        /// Lists the asset paths.
        /// </summary>
        /// <returns>The asset relative paths. Application.dataPath에 상대적인 path를 반환한다.</returns>
        /// <param name="assetPath">AssetDatabase.GetAssetPath()로 부터 얻어지는 path 혹은 Assets/ 아래의 relative path</param>
        /// <param name="wildcard">file type</param>
        public static List<FileInfo> ListAssetFiles(string assetPath, FileType fileType)
        {
            string fullPath = GetAssetFileFullPath(assetPath);
            List<FileInfo> files = new List<FileInfo>();
            foreach (string ext in fileType.GetExt())
            {
                files.AddRange(AssetUtil.ListFiles(fullPath, "*"+ext));
            }
            return files;
        }

		
        /// <summary>
        /// Finds the scene components.
        /// </summary>
        /// <returns>
        /// The scene components.
        /// </returns>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        public static List<T> FindSceneComponents<T>() where T: Component
        {
            List<T> list = new List<T>();
            foreach (GameObject go in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            {
                Transform root = go.transform;
                foreach (T t in root.GetComponentsInChildren<T>(true))
                {
                    list.Add(t);
                }
            }
            return list;
        }

        /// <summary>
        /// Get components under the specified assetPath and add them to the list
        /// </summary>
        /// <param name='assetPath'>
        /// Asset path.
        /// </param>
        /// <param name='type'>
        /// Type.
        /// </param>
        public static List<Object> SearchTypeInAsset(string assetPath, Type type)
        {
            List<Object> list = new List<Object>();
            Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            if (obj != null)
            {
                if (type.IsAssignableFrom(obj.GetType()))
                {
                    list.Add(obj);
                } else if (obj is GameObject&&typeof(Component).IsAssignableFrom(type))
                {
                    foreach (Component c in (obj as GameObject).GetComponentsInChildren(type, true))
                    {
                        list.Add(c);
                    }
                }
            }
            return list;
        }

        public static IEnumerable<T> SearchAssetObjects<T>(Object root, FileType fileType) where T: Object
        {
            foreach (var o in  SearchAssetObjects(root, typeof(T), fileType))
            {
                yield return o as T;
            }
        }

        /// <summary>
        /// Load specified types under assets (scene objects are excluded)
        /// </summary>
        /// <returns>
        /// Type instance list under root
        /// </returns>
        /// <param name='type'>
        /// Object Type
        /// </param>
        /// <param name='wildcard'>
        /// Wildcard to filter assets
        /// </param>
        public static IEnumerable<Object> SearchAssetObjects(Object root, Type type, FileType fileType)
        {
            try {
                int count = 0;
                string rootPath = ".";
                if (root != null)
                {
                    rootPath = AssetDatabase.GetAssetPath(root);
                }
                if (!string.IsNullOrEmpty(rootPath))
                {
                    string[] assetFiles = EditorAssetUtil.ListAssetPaths(rootPath, fileType);
                    foreach (string path in assetFiles)
                    {
                        foreach (var o in SearchTypeInAsset(path, type))
                        {
                            count++;
                            if (EditorUtility.DisplayCancelableProgressBar("Search", "Searching asset "+path, (count%100)/100f))
                            {
                                yield break;
                            }
                            yield return o;
                        }
                    }
                }
            } finally {
                EditorUtility.ClearProgressBar();
            }
        }

        public static string GetCurrentScene()
        {
            string scene = Application.isPlaying? SceneBridge.loadedLevelName : EditorSceneBridge.currentScene;
            return Path.GetFileNameWithoutExtension(scene);
        }

        public static List<string> GetPlayingAnimationList(Animation anim)
        {
            List<string> playing = new List<string>();
            foreach (AnimationClip c in UnityEditor.AnimationUtility.GetAnimationClips(anim.gameObject))
            {
                if (anim.IsPlaying(c.name))
                {
                    playing.Add(c.name);
                }
            }
            return playing;
        }

        public static AnimationClip CloneAnimationClip(AnimationClip src)
        {
            AnimationClip newClip = new AnimationClip();
            #pragma warning disable 0618
            UnityEditor.AnimationClipCurveData[] curveDatas = UnityEditor.AnimationUtility.GetAllCurves(src, true);
            for (int j = 0; j < curveDatas.Length; j++)
            {
                // TODOM fix warning
                UnityEditor.AnimationUtility.SetEditorCurve(
                    newClip,
                    curveDatas[j].path,
                    curveDatas[j].type,
                    curveDatas[j].propertyName,
                    curveDatas[j].curve
                );
            }
            #pragma warning restore 0618
            AnimationEvent[] events = UnityEditor.AnimationUtility.GetAnimationEvents(src);
            UnityEditor.AnimationUtility.SetAnimationEvents(newClip, events);
            return newClip;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Get the relative path under Asset/ </returns>
        /// <param name="src">Source.</param>
        public static string GetAssetRelativePath(string src)
        {
            string relative = null;
            src = src.ToUnixPath();
            if (src.StartsWith("Assets/"))
            {
                relative = src.Substring("Assets/".Length);
            } else if (src.StartsWith(Application.dataPath))
            {
                relative = src.Substring(Application.dataPath.Length);
            } else
            {
                relative = src;
            }
            if (relative.StartsWith("/"))
            {
                relative = relative.Substring(1);
            }
            return relative;
        }

        public static string GetAssetRelativePath(Object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return GetAssetRelativePath(AssetDatabase.GetAssetPath(obj));
        }

        public static string Obj2GUID(Object o)
        {
            if (o is MonoBehaviour)
            {
                o = (o as MonoBehaviour).gameObject;
            }
            string path = AssetDatabase.GetAssetPath(o);
            if (path.IsNotEmpty())
            {
                return AssetDatabase.AssetPathToGUID(path);
            }
            return path;
        }

        public static T GUID2Obj<T>(string guid) where T: Object
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.IsNotEmpty())
            {
                return AssetDatabase.LoadAssetAtPath(path, typeof(Object)) as T;
            }
            return null;
        }

        public static void ScanFolder(string id, FileType fileType, Action<List<FileInfo>> callback)
        {
            string folder = EditorPrefs.GetString(id, "Assets/");
            folder = EditorUtility.OpenFolderPanel("Scan Folder", folder, "");
			
            if (!string.IsNullOrEmpty(folder))
            {
                folder = EditorAssetUtil.GetProjectRelativePath(folder);
                EditorPrefs.SetString(id, folder);
                callback.Call(ListAssetFiles(folder, fileType));
            }
        }

        public static bool IsModified(Object o, string md5)
        {
            return IsModified(AssetDatabase.GetAssetPath(o), md5);
        }

        public static bool IsModified(string path, string hash)
        {
            string absPath = PathUtil.Combine(PathUtil.GetParent(Application.dataPath), path);
            if (!System.IO.File.Exists(absPath))
            {
                return false;
            }
            return HashFunction.Compute(new System.IO.FileStream(absPath, System.IO.FileMode.Open)) != hash;
        }

        public static void ExportToPNG(Texture2D tex, string path, TextureImporterFormat format)
        {
            File.WriteAllBytes(path, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            SetTextureFormat(path, format);
        }

        public static void ExportToJPG(Texture2D tex, string path, TextureImporterFormat format)
        {
            File.WriteAllBytes(path, tex.EncodeToJPG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            SetTextureFormat(path, format);
        }

        public static void SetTextureFormat(string path, TextureImporterFormat format)
        {
            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            importer.SetFormat(format);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }


        public static Texture2D[] SplitTexAlpha(Texture2D src)
        {
            string srcPath = AssetDatabase.GetAssetPath(src);
            TextureImporter importer = TextureImporter.GetAtPath(srcPath) as TextureImporter;
            if (!importer.isReadable)
            {
                importer.isReadable = true;
                AssetDatabase.ImportAsset(srcPath, ImportAssetOptions.ForceUpdate);
                src = AssetDatabase.LoadAssetAtPath<Texture2D>(srcPath);
            }
            Texture2D[] tex = new Texture2D[2];
            tex[0] = new Texture2D(src.width, src.height, TextureFormat.RGB24, false);
            tex[1] = new Texture2D(src.width, src.height, TextureFormat.Alpha8, false);
            for (int w = 0; w < src.width; w++)
            {
                for (int h = 0; h < src.height; h++)
                {
                    Color c = src.GetPixel(w, h);
                    tex[0].SetPixel(w, h, c);
                    tex[1].SetPixel(w, h, new Color(c.a, c.a, c.a, c.a));
                }
            }
            return tex;
        }

        public static void SplitTexAlpha(string path)
        {
            string rgbPath = PathUtil.ReplaceExtension(PathUtil.AddFileSuffix(path, "_rgb"), ".jpg");
            string aPath = PathUtil.ReplaceExtension(PathUtil.AddFileSuffix(path, "_a"), ".png");
            SplitTexAlpha(path, rgbPath, aPath);
        }

        public static void SplitTexAlpha(string path, string rgbPath, string aPath)
        {
            Texture2D src = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Texture2D[] channels = SplitTexAlpha(src);
            ExportToJPG(channels[0], rgbPath, TextureImporterFormat.Automatic);
            ExportToPNG(channels[1], aPath, TextureImporterFormat.Alpha8);
//			ReduceImageImportSize(aPath, 2);
        }

        /// <summary>
        /// Reduces the size of the image import.
        /// </summary>
        /// <param name="assetPath">Asset path.</param>
        /// <param name="scaleDown">Scale down.</param>
        public static void ReduceImageImportSize(string assetPath, int scaleDown)
        {
            // make alpha texture size as half
            TextureImporter aImporter = TextureImporter.GetAtPath(assetPath) as TextureImporter;
            int[] size = aImporter.GetOriginalTextureSize();
            aImporter.maxTextureSize = Math.Max(size[0], size[1]) / scaleDown;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        private delegate void GetWidthAndHeight(TextureImporter importer,ref int width,ref int height);

        private static GetWidthAndHeight getWidthAndHeightDelegate;

        public static int[] GetOriginalTextureSize(this TextureImporter importer)
        {
            if (getWidthAndHeightDelegate == null)
            {
                var method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic|BindingFlags.Instance);
                getWidthAndHeightDelegate = Delegate.CreateDelegate(typeof(GetWidthAndHeight), null, method) as GetWidthAndHeight;
            }
            int width = 1024;
            int height = 1024;
            getWidthAndHeightDelegate(importer, ref width, ref height);
            return new int[] { width, height };
        }

        public static IEnumerable<GameObject> SceneRoots()
        {
            var prop = new HierarchyProperty(HierarchyType.GameObjects);
            var expanded = new int[0];
            while (prop.Next(expanded))
            {
                yield return prop.pptrValue as GameObject;
            }
        }

        public static void MoveDirectory(string src, string dst)
        {
            string[] assets = ListAssetPaths(src, FileType.All);
            try 
            {
                for (int i=0; i<assets.Length; ++i)
                {
                    var path = assets[i];
                    if (EditorUtility.DisplayCancelableProgressBar("Move Directory", path, i/(float)assets.Length))
                    {
                        break;
                    }
                    string srcPath = path.Substring(src.Length+1); // skip '/' character
                    string dir = Path.GetDirectoryName(srcPath);
                    string dstDir = string.Concat(dst,"/",dir);
                    string dstPath = string.Concat(dst,"/",srcPath);
                    if (!Directory.Exists(dstDir))
                    {
                        Directory.CreateDirectory(dstDir);
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    }
                    AssetDatabase.MoveAsset(path, dstPath);
                }
                Directory.Delete(src);
                Debug.LogFormat("Moving asset {0} to {1}", src, dst);
            } finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void CopyAssets(string src, string dst, bool move = false)
        {
            string[] assets = ListAssetPaths(src, FileType.All);
            try 
            {
                for (int i=0; i<assets.Length; ++i)
                {
                    var path = assets[i];
                    if (EditorUtility.DisplayCancelableProgressBar("Move Directory", path, i/(float)assets.Length))
                    {
                        break;
                    }
                    string srcPath = path.Substring(src.Length+1); // skip '/' character
                    string dir = Path.GetDirectoryName(srcPath);
                    string dstDir = string.Concat(dst,"/",dir);
                    string dstPath = string.Concat(dst,"/",srcPath);
                    if (!Directory.Exists(dstDir))
                    {
                        Directory.CreateDirectory(dstDir);
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    }
                    if (File.Exists(dstPath))
                    {
                        AssetDatabase.DeleteAsset(dstPath);
                    }
                    if (move)
                    {
                        AssetDatabase.MoveAsset(path, dstPath);
                    } else
                    {
                        AssetDatabase.CopyAsset(path, dstPath);
                    }
                }
                if (move && Directory.Exists(src))
                {
                    Directory.Delete(src, true);
                }
                Debug.LogFormat("Moving asset {0} to {1}", src, dst);
            } finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
