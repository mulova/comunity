//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.Collections;

namespace commons
{
	/**
	 * text asset을 row 단위로 읽어 parsing한다.
	 */
	public abstract class TextRowParser<T>
	{
		public Func<string, string> preprocessor;
		private bool includeEmptyLine;

		public TextRowParser(bool includeEmptyLine)
		{
			this.includeEmptyLine = includeEmptyLine;
		}

		public List<T> Parse(string content)
		{
			return Parse(new StringReader(content));
		}

		public List<T> Parse(Stream s, Encoding encoding)
		{
			return Parse(new StreamReader(s, encoding));
		}

		public List<T> Parse(byte[] b, Encoding encoding)
		{
			return Parse(new StreamReader(new MemoryStream(b), encoding));
		}

		public virtual List<T> Parse(TextReader r)
		{
			List<T> rows = new List<T>();
			StringBuilder str = new StringBuilder();
			
			string line;
			while ((line = r.ReadLine()) != null)
			{
				string l = line.Trim();
				if (!includeEmptyLine&&l.Length == 0)
				{
					continue;
				}
				if (l.StartsWith("#")||l.StartsWith("//"))
				{
					continue;
				}
				if (preprocessor != null)
				{
					l = preprocessor(l);
				}
				/*  다음 줄로 이어진다. */
				if (l.EndsWithIgnoreCase("\\"))
				{
					str.Append(l.Substring(0, l.Length-2));
				} else
				{
					str.Append(l);
					T row = ParseLine(str.ToString());
					if (row != null)
					{
						rows.Add(row);
					}
					str.Length = 0;
				}
			}
			return rows;
		}

		protected abstract T ParseLine(string line);
	}
}
