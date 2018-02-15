//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using commons;

namespace comunity
{
	public class ExecOutput {
		public string stdout;
		public string stderr;
		public Exception ex;
		
		public ExecOutput(string stdout, string stderr) {
			this.stdout = stdout;
			this.stderr = stderr;
		}
		
		public ExecOutput(Exception ex) {
			this.ex = ex;
		}
		
		public bool IsError() {
			return stderr.IsNotEmpty() || ex != null;
		}
		
		public string GetResult() {
			if (ex != null) {
				return ex.StackTrace;
			} else if (stderr.IsNotEmpty()) {
				return stderr;
			} else {
				return stdout;
			}
		}
	}
}


