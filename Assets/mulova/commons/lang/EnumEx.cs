#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

namespace commons
{

	using System;
	using System.Collections.Generic;
	
	using System.Text;
	
	public static class EnumEx {
		private static Dictionary<Enum, string> names;
		private static Dictionary<Enum, string> fullNames;
		
		private static void Init()
		{
			if (names == null)
			{
				names = new Dictionary<Enum, string>();
				fullNames = new Dictionary<Enum, string>();
			}
		}
		
		public static string GetName(this Enum e) {
			Init();
			string s = null;
			if (!names.TryGetValue(e, out s)) {
				s = e.ToString();
				names[e] = s;
			}
			return s;
		}
		
		public static string GetFullName(this Enum e) {
			Init();
			string s = null;
			if (!fullNames.TryGetValue(e, out s)) {
				StringBuilder str = new StringBuilder(64);
				str.Append(e.GetType().FullName);
				str.Append(".");
				str.Append(e.ToString());
				fullNames[e] = str.ToString();
			}
			return s;
		}
	}
}

#endif