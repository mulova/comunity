//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons
{
    public enum LogLevel
    {
        DEBUG=100,
        INFO=200,
        WARN=300,
        ERROR=400,
        NONE=500,
    }

	public static class LogLevelEx {
		public static bool IsLoggable(this LogLevel srcLevel, LogLevel dstLevel) {
			return (int)srcLevel <= (int)dstLevel;
		}
	}

	public sealed class LogManager
	{
        public static bool usePackageName;
        private static LogManager mgr;
        private static LogLevel defaultLevel;
        private static Loggerx DUMMY_LOGGER = new LoggerImpl("", null);

        private Dictionary<string, Loggerx> loggers;
        private LogFormatter formatter;
        private List<LogAppender> appenders = new List<LogAppender>();
        
        static LogManager() {
        	mgr = new LogManager();
			SetLogFormatter(new SimpleLogFormatter());
			SetDefaultLevel(LogLevel.WARN);
		}

        private LogManager()
        {
            this.loggers = new Dictionary<string, Loggerx>();
        }

        private static LogManager GetInstance()
        {
            return mgr;
        }

        public static Loggerx GetLogger(Type type)
        {
			string name = null;
			if (type.IsGenericType)
			{
				Type g = type.GetGenericTypeDefinition();
                name = usePackageName? g.FullName: g.Name;
                name = name.Remove(name.IndexOf('`'));
            } else {
                name = usePackageName? type.FullName: type.Name;
			}
            return GetLogger(name);
        }
        public static Loggerx GetLogger(string name)
        {
        	if (name == null) {
        		return DUMMY_LOGGER;
			}
            LogManager mgr = GetInstance();
            if (mgr.loggers.ContainsKey(name))
            {
                return mgr.loggers[name];
            }
            else
            {
                LoggerImpl logger = new LoggerImpl(name, mgr.formatter);
                logger.SetLevel(defaultLevel);
                logger.SetAppenders(mgr.appenders);
                mgr.loggers.Add(name, logger);
                return logger;
            }
        }

		public static bool HasLogger(string name)
		{
			if (name.IsEmpty()) {
				return false;
			}
			LogManager mgr = GetInstance();
			return mgr.loggers.ContainsKey(name);
		}

		public static void SetDefaultLevel(LogLevel level)
		{
			defaultLevel = level;
		}

		public static LogLevel GetDefaultLevel() {
			return defaultLevel;
		}

		/// <summary>
		/// Set all loggers level
		/// </summary>
		/// <param name="level">Level.</param>
        public static void ResetLevel(LogLevel level)
        {
			SetDefaultLevel(level);
            foreach (Loggerx log in mgr.loggers.Values)
            {
                log.level = level;
            }
        }

        public static List<Loggerx> GetLoggers()
        {
            List<Loggerx> list = new List<Loggerx>();
            list.AddRange(mgr.loggers.Values);
            return list;
        }

        public static void AddAppender(LogAppender a)
        {
            mgr.appenders.Add(a);
            foreach (KeyValuePair<string, Loggerx> logger in mgr.loggers)
            {
                logger.Value.AddAppender(a);
            }
        }
		
		public static void RemoveAppender(LogAppender a)
        {
            mgr.appenders.Remove(a);
            foreach (KeyValuePair<string, Loggerx> logger in mgr.loggers)
            {
                logger.Value.RemoveAppender(a);
            }
        }
		
        public static void SetLogFormatter(LogFormatter formatter)
        {
            mgr.formatter = formatter;
            foreach (KeyValuePair<string, Loggerx> logger in mgr.loggers)
            {
                logger.Value.SetFormatter(formatter);
            }
        }

		public static LogFormatter GetLogFormatter()
		{
			return GetInstance().formatter;
		}
		
		public static void Cleanup() {
			foreach (LogAppender a in mgr.appenders)
            {
                a.Cleanup();
            }
		}
	}
}
