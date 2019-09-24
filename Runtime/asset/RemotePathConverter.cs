#if !UNITY_WEBGL
using System.Text.Ex;
using mulova.commons;
using System.Security.Cryptography;
using System.Text;
using System;
using mulova.unicore;

namespace mulova.comunity {
	internal class RemotePathConverter
	{
		/// <summary>
		/// Encode url to local file path
		/// </summary>
		public class PathEncoder : TextReplacer {
			public PathEncoder() {
				AddReplaceString("http://", "");
				AddReplaceString("https://", "");
				AddReplaceString("ftp://", "");
				AddReplaceString(":", "/");
				AddReplaceToken(@"\?", "/");
				AddReplaceString("&", "/");
				AddReplaceString("\"", "_");
				AddReplaceString("'", "_");
				AddReplaceString("=", "_");
				AddReplaceString(@"\.jpg|\.jpeg|\.JPG|\.JPEG", ".i1");
				AddReplaceString(@"\.png|\.PNG", ".i2");
			}
		}

		private readonly string cdnLocalDir;
		private readonly PathEncoder pathEncoder = new PathEncoder ();

		public RemotePathConverter()
		{
            cdnLocalDir = PathUtil.Combine(Platform.downloadPath, Cdn.DIR);
		}

		public string Convert(string remote)
		{
			if (remote.IsEmpty()) {
				return remote;
			}
			string url = DetachParams(remote);
			if (!Cdn.Path.IsEmpty() && url.StartsWith(Cdn.Path)) {
				return url.Replace(Cdn.Path, cdnLocalDir);
			} else {
				return PathUtil.Combine(cdnLocalDir, pathEncoder.Replace(url));
			}
		}

		// detach following '?'
		private string DetachParams(string u)
		{
			int index = u.IndexOf('?');
			if (index >= 0) {
				return u.Substring(0, index);
			} else {
				return u;
			}
		}
	}

	public class RemotePathHashConverter
	{
		public string Convert(string remote)
		{
            string filename = remote.ComputeHash();
            return PathUtil.Combine(Platform.downloadPath, filename.Substring(0, 2), filename);
		}
	}
}

#endif
