using UnityEngine;
using System;
using mulova.commons;


namespace mulova.comunity
{
    public class LogInitializer : Lifecycle
    {
        static LogInitializer()
        {
            LogManager.AddAppender(new UnityConsoleAppender());
        }

        public LogSnapshot snapshot = new LogSnapshot();
#if BACKUP
        private LogSnapshot backup;
#endif
        protected override void AwakeImpl()
        {
#if BACKUP
            backup = new LogSnapshot(snapshot);
            BackUp(backup);
#endif
            Apply();
        }

        public void Apply()
        {
            snapshot.Apply();
        }
        
#if BACKUP
        void OnDestroy()
        {
            if (backup != null)
            {
                Restore(backup);
            }
        }

        private void BackUp(LogSnapshot s)
        {
            s.logLevel = LogManager.GetDefaultLevel();
            foreach (LoggerData d in s.data)
            {
                d.level = LogManager.GetLogger(d.name).GetLevel();
            }
            SimpleLogFormatter f = LogManager.GetLogFormatter() as SimpleLogFormatter;
            if (f != null)
            {
                s.showName = f.ShowName;
                s.showTime = f.ShowTime;
                s.showMethod = f.ShowMethod;
                s.showLevel = f.ShowLevel;
            }
        }
#endif
    }
}

