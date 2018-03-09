//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Text;
using System.IO;

namespace commons
{
	public static class StringEx
	{
		public static readonly Loggerx log = LogManager.GetLogger(typeof(string));
		
		public static string FormatEx(this string str, object arg)
		{
			return string.Format(str, arg);
		}
		
		public static string FormatEx(this string str, object arg1, object arg2)
		{
			return string.Format(str, arg1, arg2);
		}
		
		public static string FormatEx(this string str, object arg1, object arg2, object arg3)
		{
			return string.Format(str, arg1, arg2, arg3);
		}
		
		public static string FormatEx(this string str, params object[] args)
		{
			return string.Format(str, args);
		}
		
		public static bool IsEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}
		
		public static bool IsNotEmpty(this string str)
		{
			return !string.IsNullOrEmpty(str);
		}
		
		public static bool IsURL(this string str)
		{
			if (str.IsEmpty())
			{
				return false;
			}
			return str.StartsWith("file:///")
				||str.StartsWith("http://")
				||str.StartsWith("https://")
				||str.StartsWith("ftp://");
		}
		
		public static bool Is(this string str, params FileType[] fileType)
		{
			FileType type = FileTypeEx.GetFileType(str);
			foreach (FileType t in fileType)
			{
				if (t == FileType.All)
				{
					return true;
				}
				if (t == type)
				{
					return true;
				}
			}
			return false;
		}

		public static string WrapWhitespacePath(this string str)
		{
			#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			return str.Replace(" ", @"\ ");
			#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			return str.Wrap("\"");
			#else
			return str;
			#endif
		}
		
		public static string ToUnixPath(this string str)
		{
			if (!str.IsEmpty()&&str.IndexOf('\\') >= 0)
			{
				return str.Replace('\\', '/');
			}
			return str;
		}
		
		public static StringBuilder ToUnixPath(this StringBuilder str)
		{
			return str.Replace('\\', '/');
		}
		
		public static T ParseEnum<T>(this string str) where T : struct, IComparable, IConvertible, IFormattable
		{
			if (str.IsEmpty())
			{
				return default(T);
			}
			return EnumUtil.Parse<T>(str);
		}
		
		public static T ParseEnum<T>(this string str, T defValue) where T : struct, IComparable, IConvertible, IFormattable
		{
			if (str.IsEmpty())
			{
				return defValue;
			}
			return EnumUtil.Parse<T>(str, defValue);
		}
		
		public static bool EqualsIgnoreWhiteSpace(this string str1, string str2)
		{
			if (str1.IsEmpty())
			{
				return str2.IsEmpty()||str2.Trim().IsEmpty();
			}
			int j = 0;
			for (int i = 0; i < str1.Length; ++i)
			{
				while (j < str2.Length&&char.IsWhiteSpace(str2[j]))
				{
					++j;
				}
				if (!char.IsWhiteSpace(str1[i]))
				{
					if (j >= str2.Length||str1[i] != str2[j])
					{
						return false;
					} else
					{
						++j;
					}
				}
			}
			while (j < str2.Length)
			{
				if (!char.IsWhiteSpace(str2[j]))
				{
					return false;
				}
				++j;
			}
			return true;
		}
		
		public static string Remove(this string str, Predicate<char> filter)
		{
			StringBuilder s = new StringBuilder(str.Length);
			foreach (char c in str)
			{
				if (filter(c))
				{
					s.Append(c);
				}
			}
			return s.ToString();
		}
		
		public static string RemoveWhiteSpace(this string str)
		{
			return str.Remove((c) =>
				{
					return !char.IsWhiteSpace(c);
				});
		}
		
		public static string OnlyLetterDigit(this string str)
		{
			return str.Remove((c) =>
				{
					return char.IsLetterOrDigit(c);
				});
		}
		
		public static bool IsResourcesPath(this string path)
		{
			return path != null&&(path.Contains("/Resources/")||path.StartsWith("Resources/"));
		}
		
		/// <summary>
		/// Gets the resource path from assetPath
		/// </summary>
		/// <returns>The resource path.</returns>
		/// <param name="assetPath">Asset path contains 'Resources/'</param>
		public static string GetResourcesPath(this string path)
		{
			int startIndex = 0;
			if (path.StartsWith("Resources/"))
			{
				startIndex = "Resources/".Length;
			} else
			{
				int index = path.IndexOf("/Resources/");
				if (index >= 0)
				{
					startIndex = index+"/Resources/".Length;
				} else
				{
					return path;
				}
			}
			int dotIndex = path.LastIndexOf(".");
			int length = (dotIndex > 0? dotIndex : path.Length)-startIndex;
			return path.Substring(startIndex, length);
		}
		
		public static byte[] GetBytes(this string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
		
		public static string GetString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}
		
		public static bool EqualsIgnoreCase(this string str1, string str2)
		{
			if (str1 == null)
			{
				return false;
			}
			return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
		}
		
		public static bool StartsWithIgnoreCase(this string str1, string str2)
		{
			if (str1 == null)
			{
				return false;
			}
			return str1.StartsWith(str2, StringComparison.OrdinalIgnoreCase);
		}
		
		public static bool EndsWithIgnoreCase(this string str1, string str2)
		{
			if (str1 == null)
			{
				return false;
			}
			return str1.EndsWith(str2, StringComparison.OrdinalIgnoreCase);
		}
		
		public static string CamelCaseToLower(this string src)
		{
			if (src.IsEmpty())
			{
				return src;
			}
			StringBuilder str = new StringBuilder(src.Length * 2);
			bool skipSeparator = true;
			foreach (char c in src)
			{
				if (Char.IsWhiteSpace(c))
				{
					if (str.Length > 0&&str[str.Length-1] != '_')
					{
						str.Append('_');
					}
					skipSeparator = true;
				} else if (Char.IsLetterOrDigit(c))
				{
					if (Char.IsUpper(c))
					{
						if (!skipSeparator)
						{
							str.Append('_');
						}
						str.Append(Char.ToLower(c));
						skipSeparator = true;
					} else
					{
						skipSeparator = false;
						str.Append(c);
					}
				} else
				{
					skipSeparator = true;
					str.Append(c);
				}
			}
			return str.ToString();
			//		Regex regex = new Regex(@"((?<=.)[A-Z][a-zA-Z]*)|((?<=[a-zA-Z])\d+)");
			//		return regex.Replace(str, "_$1$2").ToLower();
		}
		
		public static string Fill(this string format, params object[] param)
		{
			try
			{
				return string.Format(format, param);
			} catch (Exception ex)
			{
				log.Error(ex);
				return format;
			}
		}
		
		public static string Encode(this string src, Encoding dstEnc)
		{
			return Encode(src, null, dstEnc);
		}
		
		public static string Encode(this string src, Encoding srcEnc, Encoding dstEnc)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = srcEnc == null? new StreamWriter(stream) : new StreamWriter(stream, srcEnc);
			writer.Write(src);
			writer.Flush();
			stream.Position = 0;
			StreamReader reader = new StreamReader(stream, dstEnc);
			string str = reader.ReadToEnd();
			reader.Close();
			stream.Close();
			return str;
		}
		
		private static char[] chars = new char[] { ' ', ',', '\n', '\r', '\t' };
		
		public static string[] SplitCSV(this string str)
		{
			if (str.IsEmpty())
			{
				return new string[0];
			}
			return str.Split(chars, StringSplitOptions.RemoveEmptyEntries);
		}
		
		public static string[] SplitEx(this string str, params char[] separator)
		{
			return str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		}
		
		private static StringBuilder _builder = new StringBuilder(1024);
		public static StringBuilder sharedBuilder
		{
			get
			{
				_builder.Length = 0;
				return _builder;
			}
		}
		
		public static string Concat(params string[] strs)
		{
			var builder = sharedBuilder;
			foreach (var s in strs)
			{
				builder.Append(s);
			}
			return builder.ToString();
		}
		
		public static string Wrap(this string s, string c)
		{
			var builder = sharedBuilder;
			builder.Append(c);
			builder.Append(s);
			builder.Append(c);
			return builder.ToString();
		}
		
		/// <summary>
		/// </summary>
		/// <returns> return XXX if ".../XXX/" or ".../XXX"
		public static string GetLastFolderName(this string folderPath)
		{
			if (folderPath == null)
			{
				return null;
			}
			string path = folderPath.Replace("\\", "/");
			if (!path.EndsWith("/", StringComparison.Ordinal))
			{
				path = path+"/";
			}
			int index = path.LastIndexOf("/", path.Length-2);
			path = path.Substring(index+1, path.Length-index-1);
			return path;
		}
		
		public static bool IsNumber(this string s)
		{
			foreach (char c in s)
			{
				if (!char.IsNumber(c)&&c != ','&&c != '.')
				{
					return false;
				}
			}
			return true;
		}
		
		public static string InsertSuffix(this string path, object suffix)
		{
			string dir = PathUtil.GetDirectory(path);
			string filename = PathUtil.GetFileNameWithoutExt(path);
			string suffixStr = suffix.ToString();
			string ext = Path.GetExtension(path);
			StringBuilder str = new StringBuilder(filename.Length+suffixStr.Length+ext.Length+1);
			str.Append(dir).Append('/').Append(filename).Append(suffixStr).Append(ext);
			return str.ToString();
		}
		
		/**
		* Parase float value from string ended width 'f'
		*/
		public static float ParseFloatF(this string str)
		{
			if (str.EndsWithIgnoreCase("f"))
			{
				return float.Parse(str.Substring(0, str.Length-1));
			}
			return float.Parse(str);
		}
		
		public static string AddSuffix(this string baseName, int suf)
		{
			return AddSuffix(baseName, null, suf);
		}
		
		/**
		* parse string which may contains divide formula
		*/
		public static float ParseFloat(this string str)
		{
			int divideIndex = str.IndexOf('/');
			if (divideIndex > 0)
			{
				float dividend = ParseFloatF(str.Substring(0, divideIndex));
				float divisor = ParseFloatF(str.Substring(divideIndex+1));
				return dividend / divisor;
			}
			return ParseFloatF(str);
		}
		
		/**
		* 두자리로 가정한다.
		*/
		public static string AddSuffix(this string baseName, string separator, int suf)
		{
			StringBuilder str = new StringBuilder(baseName, baseName.Length+4);
			if (!string.IsNullOrEmpty(separator)&&!baseName.EndsWith(separator, StringComparison.Ordinal))
			{
				str.Append(separator);
			}
			if (suf < 10)
			{
				str.Append("0");
			}
			str.Append(suf);
			return str.ToString();
		}
		
		/**
		* underscore로 구분되고 마지막 문자열이 지정된 suffix로 되어있으면 떼어낸다.
		*/
		public static string DetachSpecifiedSuffix(this string src, string suffix)
		{
			int index = src.Length-1;
			if (src.EndsWithIgnoreCase(suffix))
			{
				index -= suffix.Length;
				if (src[index] == '_')
				{
					index--;
				}
			} else
			{
				return src;
			}
			return src.Substring(0, index+1);
		}
		
		public static int GetSuffix(this string src)
		{
			int val = 0;
			int digit = 1;
			int index = src.Length-1;
			while (index >= 0&&char.IsDigit(src, index))
			{
				val += digit * (src[index]-'0');
				digit *= 10;
				index--;
			}
			if (index >= 0&&index < src.Length-1)
			{
				return val;
			}
			return -1;
		}
		
		public static string DetachSuffix(this string src)
		{
			return DetachSuffix(src, "_");
		}
		
		/**
		* 뒤에 숫자 suffix가 있으면 제거하고 '_' 도 함께 제거한다.
		* underscore 외에 지정된 separator로 되어있어도 떼어낸다.
		*/
		public static string DetachSuffix(this string src, string separator)
		{
			int index = src.Length-1;
			while (index >= 0&&char.IsDigit(src, index))
			{
				index--;
			}
			if (index >= 0&&src[index] == '_')
			{
				index--;
			} else if (index >= separator.Length-1)
			{
				bool match = true;
				for (int i = 0; i < separator.Length&&match; i++)
				{
					if (src[index-i] != separator[separator.Length-1-i])
					{
						match = false;
					}
				}
				if (match)
				{
					index -= separator.Length;
				}
			}
			if (index < 0)
			{
				return src;
			}
			return src.Substring(0, index+1);
		}
	}
	
}


