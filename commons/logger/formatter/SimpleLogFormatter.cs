//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Diagnostics;
using System.Text;

namespace commons {
	public class SimpleLogFormatter : LogFormatter
	{
		private string prefix;
		public bool ShowName { get; set; }
		public bool ShowTime { get; set; }
		public bool ShowMethod { get; set; }
		public bool ShowLevel { get; set; }
		public bool ShowTrace { get; set; }

		public void SetPrefix(string prefix) {
			this.prefix = prefix;
		}

		public string Format(Loggerx logger, LogLevel level, string message, Exception e)
		{
			StringBuilder str = new StringBuilder();
			if (prefix.IsNotEmpty()) {
				str.Append(prefix);
			}
			//		str.Append('\t');
			if (ShowTime) {
				str.Append(String.Format("{0:HH:mm:ss.fff} ", DateTime.Now));
			}
			if (ShowName) {
				str.Append(logger.ToString()).Append(" ");
			}
			if (ShowLevel) {
				str.Append(level.ToString()).Append(" ");
			}
			if (ShowMethod) {
				StackTrace trace = new System.Diagnostics.StackTrace();
				StackFrame frame = trace.GetFrame(3);
				//			int line = frame.GetFileLineNumber();
				str.Append("[");
				str.Append(frame.GetMethod().ReflectedType.Name);
				str.Append(".");
				str.Append(frame.GetMethod().Name);
				str.Append("()] ");
			}
			
			//		str.Append('\n');
			if (message != null)
			{
				str.Append(message);		
			}
			if(e!=null) {
                str.AppendLine();
                str.Append(e.ToString());
			}
			return str.ToString();
		}
	}
}
