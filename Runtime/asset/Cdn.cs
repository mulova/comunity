

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using mulova.commons;
using System.Text.Ex;
using System.Collections.Generic.Ex;
using mulova.unicore;
using mulova.i18n;

namespace mulova.comunity
{
    public static class Cdn
    {
        public const string DIR = "_dl";
        public const char VER_SEPARATOR = (char)0x1E;
        private static string rootPath;
        
        public static string Path
        {
            get
            {
                return rootPath;
            }
        }
        
        private static AssetCache _cache;
        
        public static AssetCache cache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new AssetCache();
                    _cache.pathConverter = ToFullPath;
#if UNITY_WEBGL
                    _cache.SetCaching(true);
#else
                    _cache.ToLocal();
#endif
                }
                return _cache;
            }
        }
        
        public static void SetCdnServer(string cdnServer)
        {
            if (cdnServer.IsEmpty())
            {
                return;
            }
            rootPath = GetPath(cdnServer, string.Empty, Platform.platform, TexFormatGroup.GetBest());
        }
        
        public static string GetPath(string root, string zone, RuntimePlatform platform, TexFormatGroup texFormat)
        {
            return PathUtil.Combine(root, BuildConfig.BUILD_ID, zone, texFormat.GetAbCategory(platform), BuildConfig.RES_VERSION);
        }
        
        public static string ToFullPath(string relativePath)
        {
            return PathUtil.Combine(rootPath, relativePath);
        }
        
        public static string ToFullPath(string relativePath, params object[] args)
        {
            return PathUtil.Combine(rootPath, string.Format(relativePath, args));
        }
        
        public static string ToRelativePath(string fullPath)
        {
            if (!rootPath.IsEmpty())
            {
                string relative = PathUtil.GetRelativePath(fullPath, rootPath);
                if (!relative.IsEmpty())
                {
                    return relative;
                } else
                {
                    return fullPath;
                }
            } else
            {
                return fullPath;
            }
        }
        
        private static Dictionary<string, int> versions = new Dictionary<string, int>();
        
        public static void LoadVersion(string cdnPath, Action<Exception> callback)
        {
            string url = Cdn.ToFullPath(cdnPath);
            Web.noCache.GetBytes(url, b =>
                                 {
                if (b != null)
                {
                    LineParser parser = new LineParser(false);
                    char[] separator = new char[] { VER_SEPARATOR };
                    foreach (string line in parser.Parse(b, Encoding.UTF8))
                    {
                        string[] tok = line.SplitEx(separator);
                        if (tok.Length >= 2)
                        {
                            int ver = 0;
                            int.TryParse(tok[1], out ver);
                            versions.Add(Cdn.ToFullPath(tok[0]), ver);
                        }
                    }
                    callback(null);
                } else
                {
                    callback(new Exception("Can't access "+cdnPath));
                }
            });
        }
        
        public static int GetFileVersion(string url)
        {
            return versions.Get(url);
        }
        
        public static void GetResourceVersion(Action<string> callback)
        {
            string verUrl = string.Format("{0}/version_{1}.txt", Path, TexFormatGroup.GetBest().GetAbCategory(Platform.platform));
            Web.noCache.GetBytes(verUrl, bytes =>
                                 {
                if (bytes != null)
                {
                    string ver = Encoding.ASCII.GetString(bytes); 
                    callback(ver);
                } else
                {
                    AssetCache.log.Error("Can't get cdn version from {0}", verUrl);
                    callback(null);
                }
            });
        }

        public static void Clear()
        {
            _cache = null;
        }
    }
}

