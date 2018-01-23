//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Text;
using UnityEngine;


namespace core {
	public class DebugUtil {
		/**
		 * @trim x, y, z 각 요소의 string 길이
		 */ 
		public static string ToString(Vector3 v, int trim) {
			StringBuilder str = new StringBuilder(128);
			str.Append("(");
			str.Append(ToString(v.x, trim));
			str.Append(", ");
			str.Append(ToString(v.y, trim));
			str.Append(", ");
			str.Append(ToString(v.z, trim));
			str.Append(")");
			return str.ToString();
		}
		
		public static string ToString(Vector3 v) {
			return ToString(v, 100);
		}
		
		public static string ToString(float f, int length) {
			string s = f.ToString();
			return s.Substring(0, Math.Min(length, s.Length));
		}
		
		public static string ExtractTrace(Exception e) {
			string src = e.StackTrace;
			StringBuilder str = new StringBuilder(300);
			foreach (string line in src.Split('\n')) {
				int last = line.LastIndexOf("\\");
				if (last < 0) {
					line.LastIndexOf("/");
				}
				if (last >= 0) {
					str.Append(line.Substring(last+1));
					str.Append("\n");
				}
			}
			return str.ToString();
		}
	}
}

