using UnityEngine;
using System.Collections;
using UnityEditor;
using mulova.comunity;
using mulova.commons;
using System.IO;
using System.Collections.Generic;
using System.Text;

public static class AssetBundleBuilder
{
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
}
