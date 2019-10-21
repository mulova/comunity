//#define TEST
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Ex;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Ex;
using System.Threading;
using mulova.commons;
using mulova.unicore;

namespace mulova.comunity
{
    /// <summary>
    /// Download and unzip files
    /// 
    /// prerequisite: Threading
    /// 
    /// [Event In] begin download when event arrived. 
    /// [Event Out] send event when download ends or canceled.
    /// 
    /// 1. Check the version of the list (srcListUrl + ".ver")
    /// 2. if the version mismatch from the client, remove old entries and download new files.
    ///    each file can have its own version (divided by tab)
    /// 3. if downloaded file is 'zip' archive, it is unzipped in the directory named by zip file name
    /// 
    /// file list format
    /// - line: file_name(\t version)
    /// - comment: starts with '#'
    /// </summary>
    public class Downloader : LogBehaviour
    {
        public const string VER_SEPARATOR = "#";
        public int timeoutSec = 10;
        // in seconds
        public int retry = 2;
        private int retryLeft;
        public bool readOnly = true;
        // if true, assume that the all files downloaded are not changed after download
        public float retryDelay = 0.5f;
        public bool preserveProgress = true;
        // count already downloaded files in download progress

        private static string dstDir;
        // root folder where downloaded files are saved.
        private Action<Exception> onComplete;
        private DownloadStep step = DownloadStep.Null;
        private int initialDownloadSize;
        private int totalDownloadSize;
        // web variables
#if UNITY_WEBGL
		[SerializeField] WebGLDownloader _webGL;
        public WebGLDownloader webGL
        {
            get
            {
                if (_webGL == null)
                {
                    _webGL = go.FindComponent<WebGLDownloader>();
                }
                return _webGL;
            }
        }
#endif

        private Exception webException;
        private string srcRoot;
        // non-web variables
#if !UNITY_WEBGL
        private WebDownloader web;
        private Exception unzipException;
        private UnzipQueue unzipQueue;
        public UnzipQueue.UnzipMethod unzipMethod;
#endif

        private static GamePref _tags;
        private static GamePref tags
        {
            get
            {
                if (_tags == null)
                {
                    _tags = new GamePref("_dl_l", true);
                }
                return _tags;
            }
        }


        private Queue<string> filesToDownload = new Queue<string>();
        // updated list of files
        private List<string> filesDownloaded = new List<string>();

        class FileCallbackParam
        {
            public string src;
            public string dst;

            public FileCallbackParam(string src, string dst)
            {
                this.src = src;
                this.dst = dst;
            }
        }

        /// <summary>
        /// Download fileList from rootUrl.
        /// if fileList entry is downloaded once, it doesn't retryed even even if the rootUrl changes when readOnly flag is true
        /// </summary>
        /// <param name="srcListRoot">Source list root.</param>
        /// <param name="fileList">File list.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="retryCount">Retry count.</param>
        public void BeginDownload(string rootUrl, IEnumerable<string> fileList, Action<Exception> callback)
        {
            this.srcRoot = rootUrl;
            step = DownloadStep.Null;
#if UNITY_WEBGL
            if (webGL != null && webGL.IsBusy)
            {
                log.Warn("Download is already begun");
                return;
            }
#else
            if (web != null)
            {
                log.Warn("Download is already begun");
                return;
            }
            DirUtil2.CreateDirectory(GetCacheRoot());
#endif
            onComplete = callback;
            InitTags(fileList);
            SetFiles(fileList, preserveProgress);
            DownloadNext();
        }

        private void InitTags(IEnumerable<string> files)
        {
            foreach (string f in files)
            {
                if (readOnly && IsPathTagged(f))
                {
                    TagPath(f, false);
                }
            }
        }

        internal void TagPath(string cdnRelativePath, bool save = true)
        {
            string id = string.Concat(BuildConfig.RES_VERSION, "/", cdnRelativePath);
            tags.SetBool(id, true);
            if (save)
            {
                tags.Save();
            }
        }

        internal bool IsPathTagged(string cdnRelativePath)
        {
            string id = string.Concat(BuildConfig.RES_VERSION, "/", cdnRelativePath);
            return tags.GetBool(id, false);
        }

        /// <summary>
        /// Remove already downloaded files from file list if files are read-only
        /// </summary>
        /// <param name="files">Source list relative to cdn</param>
        private void SetFiles(IEnumerable<string> files, bool preserveFileCount)
        {
            this.webException = null;
#if !UNITY_WEBGL
            this.unzipException = null;
#endif
            filesDownloaded = new List<string>();
            filesToDownload = new Queue<string>();
            foreach (string f in files)
            {
                string file = f.Trim();
                if (readOnly && IsPathTagged(file))
                {
                    filesDownloaded.Add(file);
                }
                else if (!file.IsEmpty())
                {
                    filesToDownload.Enqueue(file);
                }
            }
            tags.Save();

            if (!preserveFileCount)
            {
                filesDownloaded.Clear();
            }
            initialDownloadSize = filesDownloaded.Count;
            totalDownloadSize = filesToDownload.Count + filesDownloaded.Count;

#if UNITY_WEBGL
            webGL.DownloadFileCompleted += OnWWWFileCallback;
            webGL.DownloadProgressChanged += OnWWWProgressCallback;
#else
            web = new WebDownloader();
            web.DownloadFileCompleted += OnDownloadFileCallback;
            web.DownloadProgressChanged += OnProgress;
            web.Timeout = timeoutSec * 1000;

            if (unzipMethod != null)
            {
                this.unzipQueue = new UnzipQueue(unzipMethod);
            }
            else
            {
                this.unzipQueue = null;
            }
#endif
        }

        public DownloadStep GetStep()
        {
            return step;
        }

        public Exception GetException()
        {
#if UNITY_WEBGL
            return webException;
#else
            if (webException != null)
            {
                return webException;
            }
            return unzipException;
#endif
        }

        public int GetTotalFileCount()
        {
            return totalDownloadSize;
        }

        /// <summary>
        /// Gets the downloaded file count
        /// </summary>
        /// <returns>The downloaded file count. includes currently file in progress.</returns>
        public int GetDownloadFileCount()
        {
            return totalDownloadSize - filesToDownload.Count;
        }

        private float fileProgress;

        /// <summary>
        /// Gets the download progress of the current file.
        /// </summary>
        /// <returns>the download progress of the current file</returns>
        public float GetFileProgress()
        {
            if (filesToDownload.Count > 0)
            {
                return fileProgress;
            }
#if !UNITY_WEBGL
            else if (unzipQueue != null)
            {
                return unzipQueue.GetCurrentProgress();
            }
#endif
            else if (step == DownloadStep.Canceled)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public float GetTotalProgress()
        {
            if (step == DownloadStep.Null)
            {
                return 0;
            }
            else if (step == DownloadStep.Done)
            {
                return 1;
            }
            else
            {
                if (totalDownloadSize == 0)
                {
                    return 1;
                }
#if !UNITY_WEBGL
                if (unzipQueue != null)
                {
                    return (unzipQueue.GetCompleteCount() + initialDownloadSize) / (float)totalDownloadSize;
                }
                else
#endif
                    if (GetDownloadFileCount() < totalDownloadSize)
                {
                    return (GetDownloadFileCount() + fileProgress) / totalDownloadSize;
                }
                else
                {
                    return 1f;
                }
            }
        }

#if !UNITY_WEBGL
        public static string GetCacheRoot()
        {
            if (dstDir.IsEmpty())
            {
                dstDir = PathUtil.Combine(Platform.downloadPath, Cdn.DIR);
            }
            return dstDir;
        }
#endif

        void OnDisable()
        {
            Cleanup();
            onComplete = null;
            step = DownloadStep.Canceled;
        }

        /// <summary>
        /// Cleanup connections.
        /// </summary>
        public void Cleanup()
        {
            filesToDownload.Clear();
            filesDownloaded.Clear();
#if UNITY_WEBGL
			if (_webGL != null)
            {
                _webGL.Dispose();
            }
#else
            if (unzipQueue != null)
            {
                unzipQueue.Stop();
                unzipQueue = null;
            }
            if (web != null)
            {
                web.Dispose();
                web = null;
            }
#endif
        }

#if TEST
        private System.Random rand = new System.Random();
#endif
#if UNITY_WEBGL
        private void OnWWWProgressCallback(WWW www, float progress)
        {
            fileProgress = progress;
        }
        
        private void OnWWWFileCallback(WWW www, object userState)
        {
#if TEST
            if (rand.Next(10) < 5) {
                WebDownloadFails (new System.Exception (param.dst));
            } else
#endif
                if (www.error.IsNotEmpty())
            {
                //          DeleteFile (param.dst);
                Exception ex = new Exception(string.Format("{0}: {1}", www.url, www.error));
                log.Error(ex);
                WebDownloadFails(ex);
            } else
            {
                log.Debug("Download complete: {0} ", www.url);
                try
                {
                    PlatformMethods.inst.SetNoBackupFlag(www.url);
                    filesDownloaded.Add(filesToDownload.Dequeue());
                    fileProgress = 0;
                    retryLeft = retry;
                    DownloadNext();
                } catch (Exception ex)
                {
                    WebDownloadFails(ex);
                }
            }
        }
#endif
        private void OnDownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            FileCallbackParam param = e.UserState as FileCallbackParam;
            //      LogDebug ("FileCallback {0}", param.src);
#if TEST
            if (rand.Next(10) < 5) {
                //          DeleteFile (param.dst);
                WebDownloadFails (new System.Exception (param.dst));
            } else
#endif
            if (e.Error != null)
            {
                //          DeleteFile (param.dst);
                log.Error(e.Error, param.src);
                WebDownloadFails(e.Error);
            }
            else if (e.Cancelled)
            {
                //          DeleteFile (param.dst);
                WebDownloadFails(new InvalidOperationException("Download is canceled"));
            }
#if !UNITY_WEBGL
            else if (unzipException != null)
            {
                log.Error(param.src);
                UnzipFails(unzipException);
            }
#endif
            else
            {
                LogDebug("Download complete: {0} ", param.dst);
                try
                {
                    SetNoBackUpFlag(param.dst);
                    string[] split = SplitVersion(param.dst);
#if !UNITY_WEBGL
                    if (split[0].Is(FileType.Zip))
                    {
                        unzipTargetDir = GetCacheRoot();
                        DirUtil2.CreateDirectory(unzipTargetDir);
                        if (unzipQueue != null)
                        {
                            unzipQueue.Add(param.dst, unzipTargetDir, OnUnzipComplete);
                        }
                    }
                    else
                    {
                        File.Move(param.dst, split[0]);
                    }
#endif
                    filesDownloaded.Add(filesToDownload.Dequeue());
                    fileProgress = 0;
                    retryLeft = retry;
                    DownloadNext();
                }
                catch (Exception ex)
                {
                    WebDownloadFails(ex);
                }
            }
        }

        private string unzipTargetDir;


        private void WebDownloadFails(Exception ex)
        {
            if (webException != ex)
            {
                LogError(ex, "Download fails");
            }
            webException = ex;
            SetStep(DownloadStep.Canceled);

#if UNITY_WEBGL
            bool over = true;
#else
            bool over = unzipQueue == null || unzipQueue.IsOver();
#endif
            if (over)
            {
                if (retryLeft > 0)
                {
                    retryLeft--;
                    LogWarn("Retry {0}", downloadSrcPath);
                    List<string> fileList = new List<string>();

#if !UNITY_WEBGL
                    Thread.Sleep((int)(retryDelay * 1000));
                    if (unzipMethod != null)
                    {
                        // redownload all files because files may not yet decompressed.
                        fileList.AddRange(filesDownloaded);
                    }
#endif
                    fileList.AddRange(filesToDownload);
                    Cleanup();
                    SetFiles(fileList, true);
                    DownloadNext();
                }
                else
                {
                    Complete(DownloadStep.Canceled);
                }
            }

        }

        private void UnzipFails(Exception ex)
        {
#if UNITY_WEBGL
            Complete(DownloadStep.Canceled);
#else
            if (unzipException != ex)
            {
                LogError(ex, "Unzip fails");
            }
            unzipException = ex;
            SetStep(DownloadStep.Canceled);
            if (!web.IsBusy)
            {
                Complete(DownloadStep.Canceled);
            }
#endif
        }


        private void Complete(DownloadStep s)
        {
            SetStep(s);
            Threading.InvokeLater(() =>
                                  {
                                      Cleanup();

                                      if (log.IsLoggable(LogType.Log) && filesDownloaded.Count > 0)
                                      {
                                          StringBuilder str = new StringBuilder(1024);
                                          str.Append("ChangeSet\n");
                                          foreach (string path in filesDownloaded)
                                          {
                                              str.Append("\t").Append(path).Append("\n");
                                          }
                                          log.Debug(str.ToString());
                                      }
                                      ActionEx.CallAfterRelease(ref onComplete, GetException());
                                  });
        }


        internal static string[] SplitVersion(string path)
        {
            path = path.Trim();
            if (path.Length == 0)
            {
                return null;
            }
            string[] ret = new string[2];
            int index = path.LastIndexOf(VER_SEPARATOR);
            if (index > 0)
            {
                ret[0] = path.Substring(0, index);
                ret[1] = path.Substring(index + 1, path.Length - index - 1);
            }
            else
            {
                ret[0] = path;
            }
            return ret;
        }

        private void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    log.Debug("Removed {0}", path);
                }
            }
            catch (Exception ex)
            {
                log.context = this;
                log.Error(ex);
            }
        }

        private string downloadSrcPath;
#if !UNITY_WEBGL
        private string downloadDstPath;
#endif

        private void DownloadNext()
        {
            webException = null;
#if !UNITY_WEBGL
            unzipException = null;
#endif
            if (filesToDownload.Count > 0)
            {
                string filename = filesToDownload.Peek();
                downloadSrcPath = PathUtil.Combine(srcRoot, filename);
                fileProgress = 0;
                SetStep(DownloadStep.Downloading);
                try
                {
#if UNITY_WEBGL
                    log.Debug("Download {0}", downloadSrcPath);
                    webGL.DownloadFile(downloadSrcPath, new FileCallbackParam(downloadSrcPath, null));
#else
                    LogDebug("Download {0}\n\tAt {1}", downloadSrcPath, downloadDstPath);
                    downloadDstPath = GetAbsolutePath(filename);
                    SetNoBackUpFlag(downloadDstPath);
                    web.DownloadFileAsyncEx(new Uri(downloadSrcPath), downloadDstPath, new FileCallbackParam(downloadSrcPath, downloadDstPath));
#endif
                }
                catch (Exception ex)
                {
                    WebDownloadFails(ex);
                }
            }
#if UNITY_WEBGL
            else {
                Complete(DownloadStep.Done);
            }
#else
            else if (unzipQueue == null || unzipQueue.IsEmpty())
            {
                Complete(DownloadStep.Done);
            }
            else
            {
                SetStep(DownloadStep.Unzip);
            }
#endif
        }

        private void SetNoBackUpFlag(string path)
        {
#if !UNITY_5_5_OR_NEWER
            Threading.InvokeLater(() =>
                                  {
#endif
                                      PlatformMethods.inst.SetNoBackupFlag(path);
#if !UNITY_5_5_OR_NEWER
                                  });
#endif
        }

        private void OnProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            fileProgress = e.BytesReceived / (float)e.TotalBytesToReceive;
        }

#if !UNITY_WEBGL
        internal bool Exists(string path)
        {
            string absPath = GetAbsolutePath(path);
            if (absPath.Is(FileType.Zip))
            {
                string dir = PathUtil.ReplaceExtension(absPath, "");
                return Directory.Exists(dir);
            }
            else
            {
                return File.Exists(absPath);
            }
        }

        public static string GetAbsolutePath(string path)
        {
            return PathUtil.Combine(GetCacheRoot(), path);
        }

        private void OnUnzipComplete(UnzipQueue.UnzipResult r)
        {
            if (unzipQueue == null)
            { // if unzip is cancelled
                return;
            }
            if (r.ex != null)
            {
                UnzipFails(r.ex);
            }
            else if (webException != null)
            {
                WebDownloadFails(webException);
            }
            else
            {
                LogDebug("Unzipped {0}", r.zipFile);
                if (readOnly)
                {
                    string key = PathUtil.GetRelativePath(r.zipFile, GetCacheRoot());
                    TagPath(key);
                }
                if ((unzipQueue == null || unzipQueue.IsEmpty()) && filesToDownload.Count == 0)
                {
                    Complete(DownloadStep.Done);
                }
            }
        }
#endif

        private void SetStep(DownloadStep step)
        {
            this.step = step;
        }

        private void LogError(Exception e, string format, params object[] param)
        {
            if (log.IsLoggable(LogType.Error))
            {
                Threading.InvokeLater(() =>
                                      {
                                          log.context = this;
                                          log.Error(e, format, param);
                                      });
            }
        }

        private void LogWarn(string format, params object[] param)
        {
            if (log.IsLoggable(LogType.Warning))
            {
                Threading.InvokeLater(() =>
                                      {
                                          log.context = this;
                                          log.Warn(format, param);
                                      });
            }
        }

        private void LogDebug(string format, params object[] param)
        {
            if (log.IsLoggable(LogType.Log))
            {
                Threading.InvokeLater(() =>
                                      {
                                          log.Debug(format, param);
                                      });
            }
        }
    }
}
