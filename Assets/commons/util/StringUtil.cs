//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Text;
using System.Globalization;
using System.Text.Ex;

namespace commons
{
    public static class StringUtil
    {
        public static string Assemble(params object[] objs)
        {
            StringBuilder str = new StringBuilder();
            foreach (object o in objs)
            {
                if (o != null)
                {
                    str.Append(o.ToString());
                }
            }
            return str.ToString();
        }

        public static string ToCurrencyFormat(long n, bool sign)
        {
            StringBuilder str = new StringBuilder();
            if (n == 0)
            {
                return "0";
            }
            if (sign)
            {
                if (n > 0)
                {
                    str.Append("+");
//				} else {
//					str.Append("-");
                }
            }
            str.Append(n.ToString("N0"));
            return str.ToString();
        }

        /**
		 * extract contents from text without texts such as [url]...[/url]
		 * @return string[0] : contents without enclosing texts
		 *         string[1] : url
		 */
        public static string[] Extract(string text, string head, string tail)
        {
            int i1 = text.IndexOf(head);
            int i2 = text.IndexOf(tail);
            if (i1 < 0||i2 < 0||i1 > i2)
            {
                return new string[] { text, "" };
            }
            StringBuilder str = new StringBuilder(text.Length);
            if (i1 > 0)
            {
                str.Append(text.Substring(0, i1));
            }
            if (i2+tail.Length < text.Length)
            {
                str.Append(text.Substring(i2+tail.Length, text.Length-(i2+tail.Length)));
            }
            string url = text.Substring(i1+head.Length, i2-(i1+head.Length));
            return new string[] { str.ToString(), url };
        }

        public static string GetCommonPrefix(params string[] paths)
        {
            if (paths == null||paths.Length < 2)
            {
                return string.Empty;
            }
            foreach (string s in paths)
            {
                if (s.IsEmpty())
                {
                    return string.Empty;
                }
            }
            int index = int.MaxValue;
            string cur = paths[0];

            for (int i = 1; i < paths.Length; ++i)
            {
                for (int j = 0; j <= index&&j < cur.Length&&j < paths[i].Length; ++j)
                {
                    if (cur[j] != paths[i][j])
                    {
                        index = j;
                        break;
                    }
                }
            }
            if (index == 0)
            {
                return string.Empty;
            }
            return paths[0].Substring(0, index);
        }

        public static string ToOrdinal(int num)
        {
            if( num <= 0 ) return num.ToString();
            
            switch(num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }
            
            switch(num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
            
        }

		public static bool ContainsIgnoreCase(this string s1, string s2)
		{
			return s1.IndexOfIgnoreCase(s2) >= 0;
		}

		public static int IndexOfIgnoreCase(this string s1, string s2)
		{
			if (s1 == null)
			{
				if (s2 == null)
				{
					return 0;
				} else
				{
					return 1;
				}
			} else if (s2 == null)
			{
				return -1;
			}
			return CultureInfo.CurrentCulture.CompareInfo.IndexOf(s1, s2, CompareOptions.IgnoreCase);
		}
    }
}
