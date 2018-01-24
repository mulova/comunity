//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace commons
{
	class LoggerImpl : Loggerx
	{
        public object context { get; set; }
        public LogLevel level { get; set; }
        private LogFormatter formatter;
        private List<LogAppender> appenders = new List<LogAppender>();
		private string name;
		
        public LoggerImpl(string name, LogFormatter formatter){
			this.name = name;
            this.formatter = formatter;
        }

        public void SetFormatter(LogFormatter f)
        {
            formatter = f;
		}
        
        public void AddAppender(LogAppender a) {
        	appenders.Add(a);
		}
		
		public void RemoveAppender(LogAppender a) {
			appenders.Remove(a);
		}
		
		public void SetAppenders(IEnumerable<LogAppender> a) {
			appenders.Clear();
			appenders.AddRange(a);
		}

        public void SetContext(object ctx)
        {
        }

        public void Debug(string format, object arg1)
        {
            WriteContext(LogLevel.DEBUG, null, format, arg1);
        }
        public void Debug(string format, object arg1, object arg2)
        {
            WriteContext(LogLevel.DEBUG, null, format, arg1, arg2);
        }
        public void Debug(string format, object arg1, object arg2, object arg3)
        {
            WriteContext(LogLevel.DEBUG, null, format, arg1, arg2, arg3);
        }
        public void Debug(string format, params object[] data)
        {
            WriteContext(LogLevel.DEBUG, null, format, data);
        }

        public void Info(string format, object arg1)
        {
            WriteContext(LogLevel.INFO, null, format, arg1);
        }
        public void Info(string format, object arg1, object arg2)
        {
            WriteContext(LogLevel.INFO, null, format, arg1, arg2);
        }
        public void Info(string format, object arg1, object arg2, object arg3)
        {
            WriteContext(LogLevel.INFO, null, format, arg1, arg2, arg3);
        }
        public void Info(string format, params object[] data)
        {
			WriteContext(LogLevel.INFO, null, format, data);
        }

        public void Warn(string format, object arg1)
        {
            WriteContext(LogLevel.WARN, null, format, arg1);
        }
        public void Warn(string format, object arg1, object arg2)
        {
            WriteContext(LogLevel.WARN, null, format, arg1, arg2);
        }
        public void Warn(string format, object arg1, object arg2, object arg3)
        {
            WriteContext(LogLevel.WARN, null, format, arg1, arg2, arg3);
        }
        public void Warn(string format, params object[] data)
        {
            WriteContext(LogLevel.WARN, null, format, data);
        }
		public void Warn(Exception e, string format = null, params object[] data)
		{
			WriteContext(LogLevel.WARN, e, format, data);
		}

        public void Error(string format, object arg1)
        {
            WriteContext(LogLevel.ERROR, null, format, arg1);
        }
        public void Error(string format, object arg1, object arg2)
        {
            WriteContext(LogLevel.ERROR, null, format, arg1, arg2);
        }
        public void Error(string format, object arg1, object arg2, object arg3)
        {
            WriteContext(LogLevel.ERROR, null, format, arg1, arg2, arg3);
        }
        public void Error(string format, params object[] data)
        {
            WriteContext(LogLevel.ERROR, null, format, data);
        }
		public void Error(Exception e, string format = null, params object[] data)
		{
			WriteContext(LogLevel.ERROR, e, format, data);
		}

        public void SetLevel(LogLevel level)
        {
            this.level = level;
        }
		public LogLevel GetLevel() {
			return level;
		}
		public bool IsLoggable(LogLevel l) {
			return level.IsLoggable(l);
		}

        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.DebuggerStepThrough]
        private void WriteContext(LogLevel level, Exception e, string format, object param1)
        {
            if (this.level > level) return;
            WriteContext(level, e, format, new object[]{param1});
        }

        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.DebuggerStepThrough]
        private void WriteContext(LogLevel level, Exception e, string format, object param1, object param2)
        {
            if (this.level > level) return;
            WriteContext(level, e, format, new object[]{param1, param2});
        }

        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.DebuggerStepThrough]
        private void WriteContext(LogLevel level, Exception e, string format, object param1, object param2, object param3)
        {
            if (this.level > level) return;
            WriteContext(level, e, format, new object[]{param1, param2, param3});
        }

        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.DebuggerStepThrough]
        private void WriteContext(LogLevel level, Exception e, string format, params object[] data)
        {
            if (this.level > level) return;
            
            string message = string.Empty;
			if (format.IsNotEmpty()) {
				try {
					if (!data.IsEmpty()) {
						message = string.Format(format, data);
					} else {
						message = format;
					}
				} catch (Exception ex) {
					foreach (LogAppender a in appenders) {
						a.Write(this, LogLevel.ERROR, string.Join("\n", new string[] {format, ex.Message, ex.StackTrace}));
					}
				}
			}
            if (formatter != null) {
				message = formatter.Format(this, level, message, e);
			}
			
			foreach (LogAppender a in appenders) {
				a.Write(this, level, message);
			}
        }
		
		public string Name {
			get { return name; }
		}
		
		public override string ToString() {
			return name;
		}
        
	}
}
