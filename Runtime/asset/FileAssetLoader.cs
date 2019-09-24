//#define ASSETBUNDLE_MANAGER
#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.IO;
using System.Text;
using System.Text.Ex;
using mulova.commons;
using mulova.unicore;
using UnityEngine;
using Object = UnityEngine.Object;
#if ASSETBUNDLE_MANAGER
using AssetBundles;
#endif

namespace mulova.comunity
{
    public class FileAssetLoader : IAssetLoader
    {

        public class Entry
        {
            public readonly string url;
            public readonly string digest;
            public FileInfo file;

            public Entry(string url, string digest)
            {
                this.url = url;
                this.digest = digest;
            }

            public Entry(string line)
            {
                string[] tokens = line.Split(FileAssetLoader.VER_SEPARATOR); 
                this.url = tokens[0];
                if (tokens.Length == 2)
                {
                    this.digest = tokens[1];
                } else
                {
                    this.digest = null;
                }
            }

            public bool IsCached()
            {
                return file != null;
            }

            public bool Exists()
            {
                return file != null&&file.Exists;
            }
        }

        private ConvertedKeyMap<string, Entry> url2file = new ConvertedKeyMap<string, Entry>((k) => {
            return k.ToLower();}); // key: remote url   value: local file path
        private const string EXT = ".alias";
        public const char VER_SEPARATOR = '#';
        private const int DIR_SIZE = 255;
		private RemotePathHashConverter pathConv = new RemotePathHashConverter();
#if WWW_MODULE
		private MultiMap<string, Action<WWW>> callbacks = new MultiMap<string, Action<WWW>>();
#endif
        public static readonly Loggerx log = AssetCache.log;

        public FileAssetLoader()
        {
        }

        public void Remove(string url)
        {
            Entry e = GetCacheEntry(url);
            if (e != null)
            {
                url2file.Remove(url);
                DeleteLocalFile(e.file);
            }
            fallback.Remove(url);
        }

        /// <summary>
        /// List the files and generate url to local path map
        /// </summary>
        public void ClearCache(string dir)
        {
            log.Info("Clear Cache Directory: {0}", dir);
            if (dir.Is(FileType.Zip))
            {
                dir = PathUtil.DetachExt(dir);
            }
            if (Directory.Exists(dir))
            {
                foreach (FileInfo f in AssetUtil.ListFiles(dir, "*"+EXT))
                {
                    string path = f.FullName;
                    if (File.Exists(path))
                    {
                        Entry entry = new Entry(File.ReadAllText(path, Encoding.UTF8));
                        url2file.Remove(entry.url);
                        DeleteLocalFile(new FileInfo(path.Substring(0, path.Length-EXT.Length)));
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether this url is cached
        /// </summary>
        /// <returns><c>true</c> if the url is downloaded locally and valid; otherwise, <c>false</c>.</returns>
        /// <param name="url">URL.</param>
        public bool IsCached(string url)
        {
            Entry e = GetCacheEntry(url);
            return e != null&&e.IsCached();
        }

        /// <summary>
        /// </summary>
        /// <returns> return the local cache path if asset already exists in local cache, or null</returns>
        /// <param name="url">URL.</param>
        public Uri GetCachePath(string url)
        {
            Entry e = GetCacheEntry(url);
            if (e.Exists())
            {
                return new Uri(e.file.FullName, UriKind.RelativeOrAbsolute);
            }
            return null;
        }

        private Entry GetCacheEntry(string url)
        {
            Entry e = url2file.Get(url);
            if (e != null)
            {
                return e;
            }
            // find local cdn first
            e = new Entry(url, string.Empty);
            string localPath = pathConv.Convert(url);
            if (!File.Exists(localPath)&&!localPath.Is(FileType.Asset))
            {
                localPath = PathUtil.ReplaceExtension(localPath, FileTypeEx.ASSET_BUNDLE);
            }
            if (File.Exists(localPath))
            {
                e.file = new FileInfo(localPath);
                url2file.Add(url, e);
            }
            if (Platform.isEditor)
            {
                // check upper case filename
                int i = localPath.Length-1;
                while (i >= 0 && localPath[i] != '/')
                {
                    if (char.IsUpper(localPath[i]))
                    {
                        log.Warn("UPPER CASE file name {0}", localPath);
                    }
                    i--;
                }
            }
            return e;
        }

        public void GetCdnCache(string cdnRelativeUrl, Action<FileInfo> callback)
        {
            string url = PathUtil.Combine(Cdn.Path, cdnRelativeUrl);
            GetFileCache(url, callback);
        }

        public void GetAssetBundle(string url, bool unload, Action<AssetBundle> callback)
        {
            Entry e = GetCacheEntry(url);
            if (e != null)
            {
                string filePath = e.file.FullName;
                log.Debug("Access {0}", filePath);
#if UNITY_5_3_OR_NEWER
                AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
#else
                AssetBundle bundle = AssetBundle.CreateFromFile(filePath);
#endif
                callback(bundle);
                if (unload) 
                {
                    bundle.Unload(false);
                }
            } else
            {
                fallback.GetAssetBundle(url, unload, callback);
            }
        }

        /// <summary>
        /// Gets the bytes. Support Web Platform
        /// </summary>
        /// <param name="cdnRelativeUrl">Cdn relative URL.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="timeout">Timeout.</param>
        public void GetBytes(string url, Action<byte[]> callback)
        {
            Entry e = GetCacheEntry(url);
            if (e != null&&e.file != null)
            {
                GetBytesFromFile(e.file, callback);
            } else
            {
                fallback.GetBytes(url, callback);
            }
        }

        public static void GetBytesFromFile(FileInfo f, Action<byte[]> callback)
        {
            if (f == null||!f.Exists)
            {
                callback(null);
            }
            if (f.Name.Is(FileType.Asset))
            {
                GetAssetFromFile<TextAsset>(f, a=> {
                    if (a != null)
                    {
                        callback(a.bytes);
                    } else
                    {
                        log.Warn("{0} is not text asset", f.FullName);
                        callback(null);
                    }
                });
            } else
            {
                callback(File.ReadAllBytes(f.FullName));
            }
        }

        public static void GetAssetBundleFromFile(string filePath, Action<AssetBundle> callback)
        {
            log.Debug("Access {0}", filePath);
            var ab = AssetBundle.LoadFromFile(filePath);
            callback(ab);
        }

        public static void GetAssetFromFile<T>(FileInfo f, Action<T> callback) where T: Object
        {
            if (f == null)
            {
                callback(null);
            }
            GetAssetFromFile<T>(f.FullName, callback);
        }

#if ASSETBUNDLE_MANAGER
        public static void GetAssetFromFile<T>(string filePath, Action<T> callback) where T: Object
        {
            AssetBundleLoadAssetOperation op = AssetBundleManager.LoadAssetAsync(filePath, "mainAsset", typeof(T));
            callback(op.GetAsset<T>());
        }

        private static IEnumerator GetAssetFromFileCo<T>(AssetBundleLoadAssetOperation op, Action<T> callback) where T: Object
        {
            yield return op;
            callback(op.GetAsset<T>());
        }
#else
        public static void GetAssetFromFile<T>(string filePath, Action<T> callback) where T: Object
        {
            GetAssetBundleFromFile(filePath, bundle=> {
                if (bundle != null)
                {
                    T asset = bundle.mainAsset as T;
                    bundle.Unload(false);
                    if (asset == null)
                    {
                        log.Warn("{0} is not type of {1}", filePath, typeof(T).FullName);
                    }
                    callback(asset);
                } else
                {
                    callback(null);
                }
            });
        }
#endif

        public static void GetAssetsFromFile<T>(FileInfo f, Action<T[]> callback) where T: Object
        {
            if (f == null)
            {
                callback(null);
            }
            GetAssetsFromFile<T>(f.FullName, callback);
        }

        public static void GetAssetsFromFile<T>(string filePath, Action<T[]> callback) where T: Object
        {
            GetAssetBundleFromFile(filePath, bundle=> {
                if (bundle != null)
                {
                    T[] assets = bundle.LoadAllAssets<T>();
                    bundle.Unload(false);
                    if (assets.IsEmpty())
                    {
                        log.Warn("{0} is not type of {1}", filePath, typeof(T).FullName);
                    }
                    callback(assets);
                } else
                {
                    callback(null);
                }
            });
        }

#if WWW_MODULE
		private void GetWWW(string url, bool dispose, Action<WWW> callback)
        {
            callbacks.Add(url, callback);
            if (callbacks.GetCount(url) > 1)
            {
                return;
            }
            GetFileCache(url, f => {
                if (f != null)
                {
                    Threading.inst.StartCoroutine(LoadWWWAsync(f, dispose, www => {
                        List<Action<WWW>> slot = callbacks.GetSlot(url);
                        callbacks.Remove(url);
                        slot.ForEach(s => s(www));
                    }));
                } else
                {
                    List<Action<WWW>> slot = callbacks.GetSlot(url);
                    callbacks.Remove(url);
                    slot.ForEach(s => s(null));
                }
            });
        }
#endif

		public void GetAsset<T>(string url, Action<T> callback, bool asyncHint) where T: Object
        {
            Entry e = GetCacheEntry(url);
            if (e.file != null&&!asyncHint)
            {
                GetAssetFromFile<T>(e.file.FullName, asset=> {
                    if (asset != null)
                    {
                        callback(asset);
                    } else
                    {
                        log.Warn("{0} is not type of {1}", url, typeof(T).FullName);
                        callback(null);
                    }
                });
            } else
            {
#if WWW_MODULE
                GetWWW(url, true, www => {
                    if (www != null)
                    {
                        T asset = www.GetAsset<T>();
                        callback(asset);
                    } else
                    {
                        fallback.GetAsset<T>(url, callback, asyncHint);
                    }
                });
#else
				throw new Exception("Not implemented");
#endif
			}
        }

        public void GetAssets<T>(string url, Action<IEnumerable<T>> callback, bool asyncHint) where T: Object
        {
            Entry e = GetCacheEntry(url);
            if (e.file != null&&!asyncHint)
            {
                GetAssetsFromFile<T>(e.file.FullName, assets=> {
                    if (assets != null)
                    {
                        callback(assets);
                    } else
                    {
                        log.Warn("{0} is not type of {1}", url, typeof(T).FullName);
                        callback(null);
                    }
                });
            } else
            {
#if WWW_MODULE
				GetWWW(url, true, www => {
                    if (www != null)
                    {
                        T[] assets = www.GetAssets<T>();
                        if (www.assetBundle != null)
                        {
                            www.assetBundle.Unload(false);
                        }
                        if (assets.IsNotEmpty())
                        {
                            callback(assets);
                        } else
                        {
                            log.Warn("{0} is not type of {1}", url, typeof(T).FullName);
                            callback(null);
                        }
                    } else
                    {
                        fallback.GetAssets<T>(url, callback, asyncHint);
                    }
                });
#else
				throw new Exception("Not implemented");
#endif
            }
        }

        public void GetTexture(string url, Action<Texture2D> callback)
        {
            GetFileCache(url, f => {
                if (f != null)
                {
                    GetTextureFromFile(url, f, callback);
                } else
                {
                    fallback.GetTexture(url, callback);
                }
            });
        }

        public static void GetTextureFromFile(string u, FileInfo f, Action<Texture2D> callback)
        {
            if (f == null)
            {
                callback(null);
            } else
            {
                if (f.Name.Is(FileType.Asset))
                {
                    GetAssetFromFile<Texture2D>(f, callback);
                } else
                {
                    string texPath = f.FullName;
                    var texData = TexData.Load(texPath);
                    var tex = new Texture2D(4, 4, TextureFormatEx.Get(u), texData.mipmap, texData.linear);
                    tex.wrapMode = texData.wrapMode;
                    tex.filterMode = texData.filterMode;
                    bool success = tex.LoadImage(File.ReadAllBytes(texPath), true);
                    if (success)
                    {
                        if (texData.crunch)
                        {
                            tex.Compress(true);
                        }
                    } else
                    {
                        log.Warn("Can't load textures {0}", u);
                    }
                    callback(tex);
                }
            }
		}

#if WWW_MODULE
		private IEnumerator LoadWWWAsync(FileInfo f, bool dispose, Action<WWW> callback)
        {
            if (f != null)
            {
                WWW www = new WWW(new Uri(f.FullName, UriKind.RelativeOrAbsolute).AbsoluteUri);
                yield return www;
                if (www.error.IsEmpty())
                {
                    callback(www);
                } else
                {
                    log.Warn(www.error);
                    callback(null);
                }
                if (dispose)
                {
                    www.DisposeEx();
                }
            } else
            {
                callback(null);
            }
        }

        private void LoadFile(FileInfo f, Action<WWW> callback)
        {
            if (f != null)
            {
                byte[] bytes = File.ReadAllBytes(f.FullName);
                WWW www = new WWW(new Uri(f.FullName, UriKind.RelativeOrAbsolute).AbsoluteUri, bytes);
                if (www.error.IsEmpty())
                {
                    callback(www);
                } else
                {
                    log.Warn(www.error);
                    callback(null);
                }
                www.DisposeEx();
            } else
            {
                callback(null);
            }
        }
#endif


        /// <summary>
        /// Gets the file cached locally or download new one.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="callback">Callback. fileinfo is null if file doesn't exist locally</param>
        /// <param name="timeout">Timeout in millisec</param>
        private void GetFileCache(string url, Action<FileInfo> callback)
        {
            Entry e = GetCacheEntry(url);
            if (e.Exists())
            {
                callback(e.file);
            } else
            {
                callback(null);
            }
        }

        public void GetAudio(string url, AudioClipLoadType loadType, Action<AudioClip> callback)
        {
            bool streaming = (loadType & AudioClipLoadType.Streaming) != 0;
            bool threeD = (loadType & AudioClipLoadType.ThreeD) != 0;
            bool compressed = (loadType & AudioClipLoadType.Compressed) != 0;
#if WWW_MODULE
			GetWWW(url, !streaming, www => {
                if (www != null)
                {
                    AudioClip a = compressed? www.GetAudioClipCompressed(threeD): www.GetAudioClip(threeD, streaming);
                    if (a == null)
                    {
                        log.Warn("{0} is not AudioClip", url);
                    }
                    callback(a);
                } else
                {
                    fallback.GetAudio(url, loadType, callback);
                }
            });
#else
			throw new Exception("Not implemented");
#endif
        }

        private void DeleteLocalFile(FileInfo file)
        {
            if (file == null)
            {
                return;
            }
            if (file.Exists)
            {
                file.Delete();
            }
            string alias = file.FullName+EXT;
            if (File.Exists(alias))
            {
                File.Delete(alias);
            }
        }

        public void Clear()
        {
            foreach (KeyValuePair<string, Entry> pair in url2file)
            {
                try
                {
                    Entry entry = pair.Value;
                    DeleteLocalFile(entry.file);
                } catch (System.Exception e)
                {
                    log.Warn(null, e, "Cant' clear cache {0}", pair.Value.file);
                }
            }
            url2file.Clear();
        }

        private IAssetLoader fallback = new DummyAssetLoader();

        public void SetFallback(IAssetLoader f)
        {
            this.fallback = f;
        }

        public void UnloadAsset(string url)
        {
#if ASSETBUNDLE_MANAGER
            AssetBundleManager.UnloadAssetBundle(url);
#endif
        }
    }
}
#endif