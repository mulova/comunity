#if !UNITY_WEBGL
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.ComponentModel;
using System.Collections;
using mulova.commons;


namespace mulova.comunity
{

    /// <summary>
    /// Get file from remote and store at the local file
    /// </summary>
    public class RemoteAssetLoader : IAssetLoader
    {

        public static readonly ILog log = AssetCache.log;
        private RemotePathHashConverter pathConv = new RemotePathHashConverter();
        private MultiMap<string, Action<FileInfo>> callbacks = new MultiMap<string, Action<FileInfo>>();

        public RemoteAssetLoader()
        {
            
        }

        public void Remove(string url)
        {
            string localPath = pathConv.Convert(url);
            if (File.Exists(localPath))
            {
                try
                {
                    File.Delete(localPath);
                } catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            fallback.Remove(url);
        }


        /// <summary>
        /// </summary>
        /// <returns> return the local cache path if asset already exists in local cache, or null</returns>
        /// <param name="url">URL.</param>
        public Uri GetCachePath(string url)
        {
            return new Uri(url);
        }

        public void GetAssetBundle(string url, bool unload, Action<AssetBundle> callback)
        {
            GetRemote(url, f =>
            {
                if (f != null)
                {
                    FileAssetLoader.GetAssetBundleFromFile(f.FullName, callback);
                } else
                {
                    fallback.GetAssetBundle(url, unload, callback);
                }
            });
        }

        /// <summary>
        /// Gets the bytes. Support Web Platform
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void GetBytes(string url, Action<byte[]> callback)
        {
            GetRemote(url, f => {
                if (f != null)
                {
                    FileAssetLoader.GetBytesFromFile(f, callback);
                } else
                {
                    fallback.GetBytes(url, callback);
                }
            });
		}

#if WWW_MODULE
		/// <summary>
		/// Gets the WWW.
		/// www is disposed after when the callback ends.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="callback">Callback.</param>
		private void GetWWW(string url, bool dispose, Action<WWW> callback)
        {
            GetRemote(url, f => {
                if (f == null)
                {
                    callback(null);
                } else
                {
                    Threading.inst.StartCoroutine(LoadWWWAsync(f, dispose, www => callback(www)));
                }
            });
        }

        private IEnumerator LoadWWWAsync(FileInfo f, bool dispose, Action<WWW> callback)
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
        }
#endif

        public void GetAsset<T>(string url, Action<T> callback, bool asyncHint) where T: Object
        {
            GetRemote(url, f => {
                FileAssetLoader.GetAssetFromFile<T>(f, asset=> {
                    if (asset != null)
                    {
                        callback(asset);
                    } else
                    {
                        fallback.GetAsset<T>(url, callback, asyncHint);
                    }
                });
            });
        }

        public void GetAssets<T>(string url, Action<IEnumerable<T>> callback, bool asyncHint) where T: Object
        {
            GetRemote(url, f => {
                FileAssetLoader.GetAssetsFromFile<T>(f, assets=> {
                    if (assets != null)
                    {
                        callback(assets);
                    } else
                    {
                        fallback.GetAssets<T>(url, callback, asyncHint);
                    }
                });
            });
        }

        public void GetTexture(string url, Action<Texture2D> callback)
        {
            GetRemote(url, f => {
                if (f != null)
                {
                    FileAssetLoader.GetTextureFromFile(url, f, tex=> {
                        if (tex != null)
                        {
                            callback(tex);
                        } else
                        {
                            fallback.GetTexture(url, callback);
                        }
                    });
                } else
                {
                    fallback.GetTexture(url, callback);
                }
            });
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

        /// <summary>
        /// Gets the file from remote
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="callback">Callback. fileinfo is null if file doesn't exist locally</param>
        private void GetRemote(string url, Action<FileInfo> callback)
        {
            lock(callbacks)
            {
                callbacks.Add(url, callback);
                if (callbacks.GetCount(url) > 1)
                {
                    return;
                }
            }
            string localPath = pathConv.Convert(url);
            FileInfo localFile = new FileInfo(localPath);
            if (localFile.Exists)
            {
                List<Action<FileInfo>> clist = null;
                lock(callbacks)
                {
                    clist = callbacks.GetSlot(url);
                    callbacks.Remove(url);
                }
                foreach (Action<FileInfo> c in clist)
                {
                    c(localFile);
                }
            } else
            {
                log.Debug("Access remote asset {0}", url);
                string tmpPath = localPath+".tmp";
                WebDownloader web = new WebDownloader();
                web.Timeout = AssetCache.TIMEOUT;
                web.DownloadFileCompleted += OnDownloadFileCallback;
                web.DownloadFileAsyncEx(new Uri(url), tmpPath, new DownloadCallbackParam(web, url, localPath, tmpPath));
            }
        }

        private void OnDownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            DownloadCallbackParam param = e.UserState as DownloadCallbackParam;
            List<Action<FileInfo>> c = null;
            lock(callbacks)
            {
                c = callbacks.GetSlot(param.url);
                callbacks.Remove(param.url);
            }
            param.Invoke(e, c);
        }

        private IAssetLoader fallback = new DummyAssetLoader();

        public void SetFallback(IAssetLoader fallback)
        {
            this.fallback = fallback;
        }

        public void UnloadAsset(string url)
        {
            this.fallback.UnloadAsset(url);
        }

        internal class DownloadCallbackParam
        {
            public WebDownloader web;
            public string url;
            public string localPath;
            public string tmpPath;

            public DownloadCallbackParam(WebDownloader web, string url, string localPath, string tmpPath)
            {
                this.web = web;
                this.url = url;
                this.localPath = localPath;
                this.tmpPath = tmpPath;
            }

            public void Invoke(AsyncCompletedEventArgs e, List<Action<FileInfo>> callbacks)
            {
                web.Dispose();
                FileInfo file = null;
                if (e.Cancelled||e.Error != null)
                {
                    log.Error(e.Error, "{0} download fails", url);
                    try
                    {
                        if (File.Exists(tmpPath))
                        {
                            File.Delete(tmpPath);
                        }
                    } catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                } else
                {
                    try
                    {
                        if (File.Exists(localPath))
                        {
                            File.Delete(localPath);
                        }
                        File.Move(tmpPath, localPath);
                        file = new FileInfo(localPath);
                    } catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
                foreach (Action<FileInfo> c in callbacks)
                {
                    c.Delay(file);
                }
            }
        }
    }
}
#endif