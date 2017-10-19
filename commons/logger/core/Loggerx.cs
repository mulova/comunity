//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace commons
{
	public interface Loggerx
	{
        object context { get; set; }
        LogLevel level { get; set; }

        void Debug(string format, object arg1);
        void Debug(string format, object arg1, object arg2);
        void Debug(string format, object arg1, object arg2, object arg3);
        void Debug(string format, params object[] args);

        void Info(string format, object arg1);
        void Info(string format, object arg1, object arg2);
        void Info(string format, object arg1, object arg2, object arg3);
        void Info(string format, params object[] args);

        void Warn(string format, object arg1);
        void Warn(string format, object arg1, object arg2);
        void Warn(string format, object arg1, object arg2, object arg3);
        void Warn(string format, params object[] args);
		void Warn(Exception e, string format = null, params object[] args);

        void Error(string format, object arg1);
        void Error(string format, object arg1, object arg2);
        void Error(string format, object arg1, object arg2, object arg3);
        void Error(string format, params object[] args);
		void Error(Exception e, string format = null, params object[] args);

        void AddAppender(LogAppender a);
		void RemoveAppender(LogAppender a);
		void SetFormatter(LogFormatter f);
		bool IsLoggable(LogLevel level);
	}
}
