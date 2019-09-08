//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System;

using UnityEngine;

using Object = UnityEngine.Object;
using System.Collections.Generic;
using mulova.commons;
using System.Text.Ex;
using System.Collections.Generic.Ex;

namespace mulova.comunity
{
    /// <summary>
    /// </summary>
    public class AssetCache
    {
        public static Loggerx log = LogManager.GetLogger(typeof(AssetCache));
        public const int TIMEOUT = 20 * 1000;
        private IAssetLoader loader = new DummyAssetLoader();
        private LRUCache<Object> assetCache = new LRUCache<Object>(false);
        private Dictionary<string, IdObject> exclusiveCache = new Dictionary<string, IdObject>();
        // key: alias, value: url
        private MultiMap<string, Delegate> callbacks = new MultiMap<string, Delegate>();

        public Func<string, string> pathConverter = s => s;

        public void Init(params IAssetLoader[] loaders)
        {
            for (int i = 0; i < loaders.Length; ++i)
            {
                log.Info("Set loader {0}: {1}", i, loaders[i].GetType().Name);
                if (i+1 < loaders.Length)
                {
                    loaders[i].SetFallback(loaders[i+1]);
                }
            }
            loader = loaders[0];
        }

        public void SetCaching(bool caching)
        {
            if (Application.isPlaying)
            {
                Init(new WebAssetLoader(caching));
            } else
            {
                Init(CreateEditorLoader());
            }
        }

        private IAssetLoader CreateEditorLoader()
        {
            return ReflectionUtil.NewInstance<IAssetLoader>("core.EditorAssetLoader");
        }

        #if !UNITY_WEBGL
        private IAssetLoader CreateLocalLoader()
        {
            if (Platform.isEditor)
            {
                return CreateEditorLoader();
            } else
            {
                return new FileAssetLoader();
            }
        }

        public void ToLocal()
        {
            if (StreamingAssetLoader.version > 0)
            {
                Init(CreateLocalLoader(), new StreamingAssetLoader());
            } else
            {
                Init(CreateLocalLoader());
            }
        }

        public void ToRemoteLocal()
        {
            if (StreamingAssetLoader.version > 0)
            {
                Init(new RemoteAssetLoader(), CreateLocalLoader(), new StreamingAssetLoader());
            } else
            {
                Init(new RemoteAssetLoader(), CreateLocalLoader());
            }
        }

        public void ToLocalRemote()
        {
            if (StreamingAssetLoader.version > 0)
            {
                Init(CreateLocalLoader(), new StreamingAssetLoader(), new RemoteAssetLoader());
            } else
            {
                Init(CreateLocalLoader(), new RemoteAssetLoader());
            }
        }
        #endif

        /// <summary>
        /// If alias is the same, destroy old asset and set new one
        /// </summary>
        /// <param name="alias">Alias.</param>
        /// <param name="url">URL.</param>
        /// <param name="asset">Asset.</param>
        private void AddToCache(string alias, string url, Object asset)
        {
            if (Application.isEditor)
            {
                return;
            }
            assetCache[url] = asset;
            if (!alias.IsEmpty())
            {
                // If the asset for the alias exists, destroy it and set new one
                IdObject old = exclusiveCache.Get(alias);
                if (old != null&&old.obj != asset)
                {
                    log.Info("Replacing asset cache: {0} -> {1}", old.id, url);
                    assetCache.Remove(old);
                    Object.Destroy(old.obj);
                }
                exclusiveCache[alias] = new IdObject(url, asset);
            }
        }

        public void GetAssetBundle(string url, bool unload, Action<AssetBundle> callback)
        {
            loader.GetAssetBundle(url, unload, callback);
        }

        public void GetAsset<T>(string url, Action<T> callback, bool asyncHint = false) where T: Object
        {
            GetAsset<T>(null, url, callback, asyncHint);
        }

        public void GetAsset<T>(string alias, string url, Action<T> callback, bool asyncHint = false) where T: Object
        {
            VerifyUrl(url);
            if (!url.IsEmpty())
            {
                T o = assetCache.Get(url) as T;
                if (o != null)
                {
                    callback(o);
                } else
                {
                    callbacks.Add(url, callback);
                    if (callbacks.GetCount(url) == 1)
                    {
                        log.Debug("Loading {0}", url);
                        loader.GetAsset<T>(pathConverter(url), a =>
                        {
                            if (a != null)
                            {
                                AddToCache(alias, url, a);
                            }
                            List<Delegate> slot = callbacks.GetSlot(url);
                            callbacks.Remove(url);
                            foreach (Delegate c in slot)
                            {
                                ((Action<T>)c)(a);
                            }
                        }, asyncHint);
                    }
                }
            } else
            {
                callback(null);
            }
        }

        public void GetBytes(string url, Action<byte[]> callback)
        {
            VerifyUrl(url);
            if (!url.IsEmpty())
            {
                log.Debug("Loading {0}", url);
                loader.GetBytes(pathConverter(url), callback);
            } else
            {
                callback(null);
            }
        }


        public void GetAudio(string url, AudioClipLoadType loadType, Action<AudioClip> callback, string alias = null)
        {
            VerifyUrl(url);
            if (!url.IsEmpty())
            {
                AudioClip c = assetCache.Get(url) as AudioClip;
                if (c != null)
                {
                    callback(c);
                } else
                {
                    if (url.Is(FileType.Asset))
                    {
                        GetAsset<AudioClip>(url, callback, false);
                    } else
                    {
                        callbacks.Add(url, callback);
                        if (callbacks.GetCount(url) == 1)
                        {
                            log.Debug("Loading {0}", url);
                            loader.GetAudio(pathConverter(url), loadType, clip =>
                            {
                                if (clip != null)
                                {
                                    AddToCache(alias, url, clip);
                                }
                                List<Delegate> slot = callbacks.GetSlot(url);
                                foreach (Delegate d in slot)
                                {
                                    ((Action<AudioClip>)d)(clip);
                                }
                                callbacks.Remove(url);
                            });
                        }
                    }
                }
            } else
            {
                callback(null);
            }
        }

        public void GetTexture(string url, Action<Texture> callback, string alias = null)
        {
            VerifyUrl(url);
            if (!url.IsEmpty())
            {
                Texture t = assetCache.Get(url) as Texture;
                if (t != null)
                {
                    callback(t);
                } else
                {
                    if (url.Is(FileType.Asset))
                    {
                        GetAsset<Texture>(alias, url, callback, false);
                    } else
                    {
                        callbacks.Add(url, callback);
                        if (callbacks.GetCount(url) == 1)
                        {
                            log.Debug("Loading {0}", url);
                            loader.GetTexture(pathConverter(url), tex =>
                            {
                                if (tex != null)
                                {
                                    AddToCache(alias, url, tex);
                                }
                                List<Delegate> slot = callbacks.GetSlot(url);
                                foreach (Delegate c in slot)
                                {
                                    ((Action<Texture>)c)(tex);
                                }
                                callbacks.Remove(url);
                            });
                        }
                    }
                }
            } else
            {
                callback(null);
            }
        }

        public static string VerifyUrl(string url)
        {
            if (Platform.isEditor && !url.IsEmpty())
            {
                for (int i = url.Length-1; i >= 0; i--)
                {
                    char c = url[i];
                    if (c == '/' || c == '\\')
                    {
                        return null;
                    }
                    if (char.IsUpper(c))
                    {
                        return url+" has UpperCase.";
                    }
                }
            }
            return null;
        }

        public void Remove(string url)
        {
            loader.Remove(url);
        }

        public Uri GetCachePath(string url)
        {
            return loader.GetCachePath(pathConverter(url));
        }

        public IAssetLoader GetImpl()
        {
            return loader;
        }

        public static string GetAssetCategory(RuntimePlatform platform, TexFormatGroup group)
        {
            string dir = platform.IsStandalone()? "pc" : platform.GetPlatformName();
            if (group != TexFormatGroup.AUTO)
            {
                dir = string.Concat(dir, "_", group.id);
            }
            return dir;
        }

        public static string GetAssetCategory()
        {
            return GetAssetCategory(Platform.platform, TexFormatGroup.GetBest());
        }
    }
}
