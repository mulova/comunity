#if UNITY_IOS && PBX_PROJECT
using System;
using System.IO;
using UnityEditor.iOS.Xcode;
using System.Collections.Generic;
using mulova.commons;
using UnityEngine;
using UnityEditor;

public class IosPostprocessor
{
    private string rootPath;
    private string pbxprojPath;
    private string target;
    private object projObj;
    // workaround for TypeLoadException
    private object plistObj;
    // workaround for TypeLoadException

    public PBXProject proj
    {
        get
        {
            return projObj as PBXProject;
        }
        set
        {
            projObj = value;
        }
    }

    public PlistDocument plist
    {
        get
        {
            return plistObj as PlistDocument;
        }
        set
        {
            plistObj = value;
        }
    }

    public IosPostprocessor(string projPath)
    {
        this.rootPath = projPath;
        this.pbxprojPath = PBXProject.GetPBXProjectPath(projPath);

        proj = new PBXProject();
        proj.ReadFromFile(pbxprojPath);
        target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
    }


    /// <summary>
    /// Add user packages to project.
    /// Most other source or resource files and packages can be added the same way.
    /// (except system library)
    /// </summary>
    /// <param name="srcPath">Source path.</param>
    public void AddUserPackage(string physicalPath)
    {
        AddUserPackage(physicalPath, "Frameworks/");
    }

    public void AddUserPackage(string physicalPath, string dstDir)
    {
        string packageName = Path.GetFileName(physicalPath);
        string packagePath = PathUtil.Combine(dstDir, packageName);
        DirUtil.Copy(physicalPath, Path.Combine(rootPath, packagePath), true);
        proj.AddFileToBuild(target, proj.AddFile(packagePath, packagePath, PBXSourceTree.Source));
    }

    public void AddFile(string path)
    {
        string absPath = PathUtil.Combine(Application.dataPath, path);
        string dir = PathUtil.GetDirectory(path);
        AddFile(absPath, dir);
    }

    public void AddFile(string srcPath, string dstDir)
    {
        string dstName = Path.GetFileName(srcPath);
        string dstPath = PathUtil.Combine(dstDir, dstName);
        string dstAbsPath = Path.Combine(rootPath, dstPath);
        string dstAbsDir = PathUtil.GetDirectory(dstAbsPath);
        if (!Directory.Exists(dstAbsDir))
        {
            Directory.CreateDirectory(dstAbsDir);
        }
        File.Copy(srcPath, dstAbsPath, true);
        proj.AddFileToBuild(target, proj.AddFile(dstPath, dstPath, PBXSourceTree.Source));
    }

    // Add custom system frameworks. Duplicate frameworks are ignored.
    public void AddSystemFramework(string name, bool weak = false)
    {
        proj.AddFrameworkToProject(target, name, weak);
    }

    public void AddSystemLibrary(string name, bool weak = false)
    {
//          string fileGuid = proj.AddFile("usr/lib/" + name, "Frameworks/" + name);
        string fileGuid = proj.AddFile("usr/lib/"+name, "Frameworks/"+name, PBXSourceTree.Sdk);
        proj.AddFileToBuild(target, fileGuid);
    }

    // example: proj.SetBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
    public void SetBuildProperty(string key, string value)
    {
        proj.SetBuildProperty(target, key, value);
    }

    // example: proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");
    //          proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
    public void AddBuildProperty(string key, string value)
    {
        proj.AddBuildProperty(target, key, value);
    }

    public void AddLdFlag(string value)
    {
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", value);
    }

    public void AddCFlag(string value)
    {
        proj.AddBuildProperty(target, "OTHER_CFLAGS", value);
    }

    public void AddFrameworkSearchPath(string value)
    {
        proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", PathUtil.Combine("$(PROJECT_DIR)/", value));
    }

    public void AddHeaderPath(string value)
    {
        proj.AddBuildProperty(target, "HEADER_SEARCH_PATHS", PathUtil.Combine("$(SRCROOT)/", value));
    }

    public void AddLibraryPath(string value)
    {
        proj.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", PathUtil.Combine("$(SRCROOT)/", value));
    }

    public void SetBitCode(bool enable)
    {
        proj.SetBuildProperty(target, "ENABLE_BITCODE", enable? "YES" : "NO");
    }

    public void SetProductName(string value)
    {
        proj.SetBuildProperty(target, "PRODUCT_NAME", value);
    }

    public void SetDevelopmentTeam(string value)
    {
        proj.SetBuildProperty(target, "DEVELOPMENT_TEAM", value);
    }

    public void AddConfig(string key, Dictionary<string, object> values)
    {
        if (values == null)
            return;
        var dic = plist.root.CreateDict(key);
        AddConfig(dic, values);
    }

    public void AddConfig(string key, List<object> values)
    {
        if (values == null)
            return;
        var ary = plist.root.CreateArray(key);
        AddConfig(ary, values);
    }

    public void AddConfig(string key, List<string> values)
    {
        if (values == null)
            return;
        AddConfig(key, values.ConvertAll(x => (object)x));
    }

    public void AddConfig(string key, string value)
    {
        plist.root.SetString(key, value);
    }

    public void AddConfig(string key, bool value)
    {
        plist.root.SetBoolean(key, value);
    }

    public void AddConfig(string key, int value)
    {
        plist.root.SetInteger(key, value);
    }

    public void MergeConfig(string key, Dictionary<string, object> values)
    {
        if (values == null)
            return;

        if (plist.root.values.ContainsKey(key) == false)
            AddConfig(key, values);
        else
        {
            var dic = plist.root.values[key].AsDict();
            AddConfig(dic, values);
        }
    }

    public void MergeConfig(string key, List<object> values)
    {
        if (values == null)
            return;

        if (plist.root.values.ContainsKey(key) == false)
            AddConfig(key, values);
        else
        {
            var ary = plist.root.values[key].AsArray();
            AddConfig(ary, values);
        }
    }

    public void AddQueryScheme(string scheme)
    {
        MergeConfig("LSApplicationQueriesSchemes", new List<object>() {
            scheme
        });
    }

    public void AddQueryScheme(List<object> schemes)
    {
        MergeConfig("LSApplicationQueriesSchemes", schemes);
    }

    public void AddURLScheme(string id, string scheme)
    {
        MergeConfig("CFBundleURLTypes",
            new List<object>() {
                new Dictionary<string, object>() {
                    { "CFBundleURLName", id }, {"CFBundleURLSchemes", new List<object>() {
                            scheme
                        }
                    }
                }
            });
    }

    private void AddConfig(object d, Dictionary<string, object> values)
    {
        PlistElementDict dic = d as PlistElementDict;
        foreach (var pair in values)
        {
            if (pair.Value is string)
                dic.SetString(pair.Key, (string)pair.Value);
            else if (pair.Value is int)
                dic.SetInteger(pair.Key, (int)pair.Value);
            else if (pair.Value is bool)
                dic.SetBoolean(pair.Key, (bool)pair.Value);
            else if (pair.Value is Dictionary<string, object>)
            {
                var parent = dic.CreateDict(pair.Key);
                AddConfig(parent, (Dictionary<string, object>)pair.Value);
            } else if (pair.Value is List<object>)
            {
                var parent = dic.CreateArray(pair.Key);
                AddConfig(parent, (List<object>)pair.Value);
            }
        }
    }

    private void AddConfig(object a, List<object> values)
    {
        PlistElementArray ary = a as PlistElementArray;
        foreach (var v in values)
        {
            if (v is int)
                ary.AddInteger((int)v);
            else if (v is string)
                ary.AddString((string)v);
            else if (v is bool)
                ary.AddBoolean((bool)v);
            else if (v is Dictionary<string, object>)
            {
                var parent = ary.AddDict();
                AddConfig(parent, (Dictionary<string, object>)v);
            }
        }
    }

    public void Save()
    {
        proj.WriteToFile(pbxprojPath);
    }
}
#endif