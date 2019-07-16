using System;
using System.IO;

namespace comunity
{
	public static class HashFunction
	{
		public static string Compute(Stream s)
		{
			return MD5.Digest(s);
		}

		public static string Compute(string str)
		{
			return MD5.Digest(str);
		}
	}
}

