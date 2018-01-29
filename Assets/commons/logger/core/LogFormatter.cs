//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Diagnostics;

namespace commons
{
	public interface LogFormatter
	{
        string Format(Loggerx logger, LogLevel level, string message, Exception e);
	}
}
