using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.IO;
using commons;

namespace comunity
{
    public class CdnDownloader : Script
    {
        public const string PREF_CDN_RES_VER = "cdn_res_ver";
        public const string PREF_CLIENT_RES_VER = "client_res_ver";
        public Downloader listDownloader;
        public Downloader fileDownloader;
#if !UNITY_WEBGL
        public UnzipQueue.UnzipMethod unzipMethod;
#endif
        
        private int newResVer;
        private DownloadStep step = DownloadStep.Null;
        private GamePref _pref;
        private GamePref pref {
            get {
                if (_pref == null)
                {
                    _pref = new GamePref("_cdn_pref_", true);
                }
                return _pref;
            }
        }
        
        private void InitDownloader()
        {
            if (listDownloader == null)
            {
                listDownloader = go.AddComponent<Downloader>();
            }
#if UNITY_WEBGL
            listDownloader.webGL.caching = false;
#endif
            listDownloader.readOnly = false;
            if (fileDownloader == null)
            {
                fileDownloader = go.AddComponent<Downloader>();
            }
#if UNITY_WEBGL
            fileDownloader.webGL.caching = true;
#endif
            fileDownloader.readOnly = true;
        }
        
        /// <summary>
        /// Download files by version number
        // 1. if app is launched the very first, use "[version]/filelist_[version].txt"
        // 2. else if client resource version is the same as server's, get "[version]/patch_[version].txt"
        // 3. else use "filelist_[client_version].txt"
        /// </summary>
        /// <param name="resVer">Res ver.</param>
        /// <param name="callback">Callback.</param>
        public void Download(string verStr, Action<Exception> callback)
        {
            log.Info("Downloading version {0}", verStr);
            
            step = DownloadStep.Null;
            
            newResVer = 0;
            if (int.TryParse(verStr.Trim(), out newResVer))
            {
                int savedResVer = pref.GetInt(PREF_CDN_RES_VER, 0); 
                string savedClientVer = pref.GetString(PREF_CLIENT_RES_VER, string.Empty); 
#if !UNITY_WEBGL
                if (savedResVer == 0&&StreamingAssetLoader.version > 0)
                {
                    savedResVer = StreamingAssetLoader.version;
                }
#endif
                if (savedClientVer != BuildConfig.RES_VERSION || savedResVer != newResVer)
                {
                    // 1. if app is launched the very first, use "filelist_[version].txt"
                    // 2. if client resource version is the same as server's, get "patch_[version].txt"
                    // 3. else use "filelist_c[client_version].txt"
#if UNITY_WEBGL
                    DownloadPatch(savedResVer, newResVer, callback);
#else
                    if (savedClientVer == BuildConfig.RES_VERSION && savedResVer != 0)
                    {
                        // download patch list files
                        DownloadPatch(savedResVer, newResVer, callback);
                    } else
                    {
                        DownloadList(string.Format("{0:D3}/filelist_{0:D3}.txt", newResVer), callback);
                    }
#endif
                } else
                {
                    step = DownloadStep.Done;
                    callback(null);
                }
            } else
            {
                callback(new Exception("No resource version "+verStr));
            }
        }
        
        private void DownloadFiles(IEnumerable<string> srcList, Action<Exception> callback)
        {
            step = DownloadStep.Downloading;
#if !UNITY_WEBGL
            fileDownloader.unzipMethod = unzipMethod;
#endif
            fileDownloader.BeginDownload(Cdn.Path, srcList, e =>
                                         {
                if (e == null)
                {
                    step = DownloadStep.Done;
                    pref.SetInt(PREF_CDN_RES_VER, newResVer);
                    pref.SetString(PREF_CLIENT_RES_VER, BuildConfig.RES_VERSION);
                } else
                {
                    step = DownloadStep.Canceled;
                }
                callback(e);
            });
        }
        
        public void DownloadList(string listFile, Action<Exception> callback)
        {
            DownloadList(new string[] { listFile }, callback);
        }
        
        public void DownloadList(IList<string> listFiles, Action<Exception> callback)
        {
            InitDownloader();
            step = DownloadStep.Null;
#if !UNITY_WEBGL
            listDownloader.unzipMethod = unzipMethod;
#endif
            log.Info("Downloading list files {0}", listFiles.Join(","));
            listDownloader.BeginDownload(Cdn.Path, listFiles, e =>
                                         {
                if (e == null)
                {
                    Exception ex = null;
                    // merge list files
                    List<string> files = new List<string>();
                    int count = 0;
                    foreach (string f in listFiles)
                    {
                        Web.noCache.GetBytes(Cdn.ToFullPath(f), b=> {
                            count++;
                            if (b != null)
                            {
                                StreamReader r = new StreamReader(new MemoryStream(b));
                                while (!r.EndOfStream)
                                {
                                    string line = r.ReadLine();
                                    //                              #if UNITY_WEBGL
                                    //                              line = PathUtil.ReplaceExtension(line, FileTypeEx.ASSET_BUNDLE);
                                    //                              #endif
                                    files.Add(line);
                                }
                            } else
                            {
                                ex = new Exception(f);
                            }
                            if (count == listFiles.Count)
                            {
                                if (ex == null)
                                {
                                    DownloadFiles(files, callback);
                                } else
                                {
                                    step = DownloadStep.Canceled;
                                    callback(ex);
                                }
                            }
                        });
                    }
                } else
                {
                    step = DownloadStep.Canceled;
                    callback(e);
                }
            });
        }
        
        // download list files from 'patch_${savedResVer}' ~ 'patch_${serverResVer}'
        // and merge list files.
        private void DownloadPatch(int savedResVer, int serverResVer, Action<Exception> callback)
        {
            List<string> listFiles = new List<string>();
            for (int no = savedResVer+1; no <= serverResVer; ++no)
            {
#if UNITY_WEBGL         
                string file = string.Format("{0:D3}/modlist_{0:D3}.txt", no);
#else
                string file = string.Format("{0:D3}/patch_{0:D3}.txt", no);
#endif
                listFiles.Add(file);
                log.Info("Listing patch file {0}", file);
            }
            DownloadList(listFiles, callback);
        }
        
        public float GetTotalProgress()
        {
            if (fileDownloader == null)
            {
                return 0;
            }
            return fileDownloader.GetTotalProgress();
        }
        
        public float GetFileProgress()
        {
            return fileDownloader != null? fileDownloader.GetFileProgress() : 0;
        }
        
        public DownloadStep GetStep()
        {
            return step;
        }
    }
}
