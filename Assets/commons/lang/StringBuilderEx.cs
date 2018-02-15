//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System.Collections;
using System.Text;

namespace commons {
	public static class StringBuilderEx {
		public static StringBuilder Trim(this StringBuilder s) {
			int begin = 0;
			for (; begin<s.Length && char.IsWhiteSpace(s[begin]); begin++) {
			}
			int end = s.Length-1;
			for (; end>begin && char.IsWhiteSpace(s[end]); end--) {
			}
			if (end < begin) {
				s.Length = 0;
			} else {
				s.Remove(end+1, s.Length-(end+1));
				s.Remove(0, begin);
			}
			return s;
		}
		
		public static StringBuilder Unwrap(this StringBuilder s) {
			s.Replace("\r\n", " ");
			s.Replace('\n', ' ');
			s.Trim();
			return s;
		}

		public static StringBuilder ConcatWithSeparator(this StringBuilder s, IEnumerable e, string separator) {
			bool concat = false;
			foreach (object o in e) {
				s.Append(o.ToString());
				if (concat) {
					s.Append(separator);
				} else {
					concat = true;
				}
			}
			return s;
		}
	}
}

