//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Text;
using System.IO;
using System.Text.Ex;

namespace commons
{
	public static class PathUtil
	{
		public static string Combine(params object[] paths)
		{
			StringBuilder str = new StringBuilder();
			foreach (object o in paths)
			{
				string path = o.ToText().Trim();
				if (path.IsNotEmpty())
				{
					if (str.Length == 0)
					{
						str.Append(path);
					} else
					{
						if (str[str.Length-1] != '/'&&str[str.Length-1] != '\\')
						{
							str.Append('/');
						}
						if (path.StartsWith("/")||path.StartsWith("\\"))
						{
							str.Append(path, 1, path.Length-1);
						} else
						{
							str.Append(path);
						}
					}
				}
			}
			str.ToUnixPath();
			
			return str.ToString();
		}

		/// <summary>
		/// Replaces the extension.
		/// </summary>
		/// <returns>The extension.</returns>
		/// <param name="src">Source.</param>
		/// <param name="ext">.(dot)을 포함한 확장자</param>
		public static string ReplaceExtension(string src, string ext)
		{
			if (src.EndsWithIgnoreCase(ext))
			{
				return src;
			}
			int dotIndex = src.LastIndexOf(".");
			if (dotIndex >= 0)
			{
				return src.Substring(0, dotIndex)+ext;
			} else
			{
				return src;
			}
		}

		/// <summary>
		/// Gets the directory.
		/// </summary>
		/// <returns>directory part of the 'src'. if 'src' is the directory as is, return as it is.</returns>
		/// <param name="src">Source.</param>
		public static string GetDirectory(string src)
		{
			src = src.ToUnixPath();
			if (src.EndsWith("/", StringComparison.Ordinal))
			{
				return src;
			}
			int slashIndex = src.LastIndexOf("/");
			if (slashIndex >= 0)
			{
				return src.Substring(0, slashIndex+1);
			}
			return "";
		}

		public static string GetSibling(string src, string siblingFileName)
		{
			return Combine(GetDirectory(src), siblingFileName);
		}

		public static string ChangeDirectory(string path, string dir)
		{
			string filename = Path.GetFileName(path);
			return Combine(dir, filename);
		}

		public static string GetParent(string src)
		{
			if (src.IsEmpty())
			{
				return string.Empty;
			}
			int endIndex = src.Length-1;
			if (src.EndsWith("/", StringComparison.Ordinal))
			{
				endIndex--;
			}
			while (endIndex >= 0&&src[endIndex] != '/')
			{
				endIndex--;
			}
			if (endIndex > 0)
			{
				return src.Substring(0, endIndex);
			}
			return string.Empty;
		}

		public static string GetLastDirectory(string src)
		{
			if (src.IsEmpty())
			{
				return string.Empty;
			}
			src = src.ToUnixPath();
			int endIndex = src.Length-1;
			if (!src.EndsWith("/", StringComparison.Ordinal))
			{
				endIndex = src.LastIndexOf('/');
			}
			int slashIndex = src.LastIndexOf("/", endIndex-1);
			if (slashIndex >= 0)
			{
				return src.Substring(slashIndex+1, endIndex-slashIndex-1);
			}
			return src;
		}

		public static string GetFileNameWithoutExt(string src)
		{
			int slashIndex = src.LastIndexOf("/")+1;
			int dotIndex = src.LastIndexOf(".");
			if (dotIndex >= 0)
			{
				return src.Substring(slashIndex, dotIndex-slashIndex);
			} else
			{
				return src.Substring(slashIndex, src.Length-slashIndex);
			}
		}

		/**
		 * 확장자 바로 앞에 suffix를 붙인다.
		 */
		public static string AddFileSuffix(string path, string suffix)
		{
			string dir = Path.GetDirectoryName(path);
			string filename = Path.GetFileNameWithoutExtension(path);
			string ext = Path.GetExtension(path);
			if (dir.IsNotEmpty())
			{
				return string.Concat(dir, "/", filename, suffix, ext);
			} else
			{
				return string.Concat(filename, suffix, ext);
			}

		}

		public static string GetCommonParent(params string[] paths)
		{
			string common = StringUtil.GetCommonPrefix(paths);
			return GetDirectory(common);
		}

		public static string DetachExt(string path)
		{
			int dotIndex = path.LastIndexOf('.');
			if (dotIndex < 0)
			{
				return path;
			}
			int sepIndex = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
			if (sepIndex >= 0&&dotIndex < sepIndex)
			{
				return path;
			}
			return path.Substring(0, dotIndex);
		}

		public static string GetRelativePath(string fullPath, string parent)
		{
			fullPath = fullPath.ToUnixPath();
			parent = parent.ToUnixPath();

			if (fullPath.StartsWith(parent))
			{
				if (fullPath[parent.Length] == '/')
				{
					return fullPath.Substring(parent.Length+1);
				} else
				{
					return fullPath.Substring(parent.Length);
				}
			} else
			{
				return string.Empty;
			}
		}

		#if !UNITY_WEBGL
		public static string GetUniquePath(string path)
		{
			string p = path;
			int suffix = 1;
			if (File.Exists(p))
			{
				p = PathUtil.AddFileSuffix(path, suffix.ToString("D2"));
				suffix++;
			}
			return p;
		}
		#endif

		public static string WrapQuotation(string path)
		{
			if (path.IsNotEmpty()&&path.IndexOf(' ') >= 0)
			{
				return string.Concat("\"", path, "\"");
			}
			return path;
		}
	}
}
