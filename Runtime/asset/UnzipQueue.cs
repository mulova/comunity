#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Threading;
using mulova.commons;

namespace mulova.comunity
{
    public class UnzipQueue
    {
        public static readonly ILogger log = LogManager.GetLogger(typeof(UnzipQueue));
        public delegate void UnzipMethod(UnzipEntry e, int[] fileProcessed, out int fileCount);
        public UnzipMethod unzip;
        private int completeCount;
        private Queue<UnzipEntry> queue = new Queue<UnzipEntry>();
        private Exception exception;
        private volatile Thread thread;
        
        public UnzipQueue(UnzipMethod func) {
            this.unzip = func;
        }
        
        public void Stop() {
            lock (this) {
                thread = null;
                queue.Clear();
            }
        }
        
        public void Add(string srcZip, string dstFolder, Action<UnzipResult> callback) {
            lock (this) {
                queue.Enqueue(new UnzipEntry(srcZip, dstFolder, callback));
                if (thread == null) {
                    //          ThreadPool.QueueUserWorkItem(Update, null);
                    thread = new Thread(Update);
                    thread.Priority = System.Threading.ThreadPriority.AboveNormal;
                    thread.Start();
                }
            }
        }
        
        private void Update() {
            while (!IsOver()) {
                if (queue.Count == 0) {
                    Thread.Sleep (100);
                } else {
                    Unzip();
                }
            }
        }
        
        private int totalProgress;
        private int[] progress = new int[1];
        private void Unzip() {
            UnzipEntry current = queue.Peek();
            log.Debug("Extracting {0} at {1} ...", current.srcZip, current.targetDir);
            progress[0] = 0;
            try {
                totalProgress = 1;
                unzip(current, progress, out totalProgress);
                progress[0] = 0;
                completeCount++;
            } catch (Exception ex) {
                exception = ex;
                LogError(ex, "Unzipping {0} fails.", current.srcZip);
            }
            lock (this) {
                queue.Dequeue();
            }
            current.Callback(exception);
        }
        
        public int GetCompleteCount() {
            return completeCount;
        }
        
        public float GetCurrentProgress() {
            if (totalProgress == 0) {
                return 1;
            }
            return progress[0]/(float)totalProgress;
        }
        
        private void LogError(Exception e, string format, params object[] param) {
            if (log.IsLoggable(LogType.Error))
            {
                Threading.InvokeLater(()=> {
                    log.Error(e, format, param);
                });
            }
        }
        
        private void LogWarn(string format, params object[] param) {
            LogWarn(null, format, param);
        }
        
        private void LogDebug(string format, params object[] param)
        {
            if (log.IsLoggable(LogType.Log))
            {
                Threading.InvokeLater(()=> {
                    log.Debug(format, param);
                });
            }
        }
        
        public bool IsEmpty() {
            return queue.Count == 0;
        }
        
        public bool IsOver() {
            lock (this) {
                return thread == null || exception != null;
            }
        }
        
        public class UnzipEntry {
            public readonly string srcZip;
            public readonly string targetDir;
            public Action<UnzipResult> callback;
            
            public UnzipEntry(string srcZip, string targetDir, Action<UnzipResult> callback) {
                this.srcZip = srcZip;
                this.targetDir = targetDir;
                this.callback = callback;
            }
            
            public void Callback(Exception ex) {
                callback(new UnzipResult(srcZip, ex));
            }
        }
        
        public class UnzipResult {
            public readonly string zipFile;
            public readonly Exception ex;
            
            public UnzipResult(string zipFile, Exception ex) {
                this.zipFile = zipFile;
                this.ex = ex;
            }
        }
    }
}

#endif