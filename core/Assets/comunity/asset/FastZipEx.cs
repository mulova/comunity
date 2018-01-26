using System.Collections.Generic;
using System.IO;
using System.Text;


#if SHARPZIPLIB
using crypto.ex;

namespace ICSharpCode.SharpZipLib.Zip {
	public static class FastZipEx {
		/// <summary>
		/// Support UTF8
		/// Create list files
		/// </summary>
		/// <param name="zip">Zip.</param>
		/// <param name="zipPath">Zip path.</param>
		/// <param name="srcDir">Source dir.</param>
		/// <param name="srcFiles">Source files.</param>
		/// <param name="deleteSrcFiles">If set to <c>true</c> delete source files.</param>
		/// <param name="removedFiles">remove files are added to the list file</param>
		public static void CreateZipAndList(this FastZip zip, string zipPath, string srcDir, IEnumerable<FileInfo> srcFiles, bool deleteSrcFiles, IEnumerable<string> removedFiles = null) {
			string zipName = PathUtil.GetFileNameWithoutExt(zipPath)+".zip";
			// Compress
			List<string> paths = new List<string>();
			foreach (FileInfo f in srcFiles) {
				paths.Add(f.FullName);
			}
			
			// Generate MD5 digest for assets
			StringBuilder zipFilter = new StringBuilder();
			StringBuilder fileListStr = new StringBuilder();
			fileListStr.Append(DownloadList.ZIP_HEADER).Append(zipName).Append("\n");

			// create zip filter and file list
			foreach (FileInfo f in srcFiles) {
				string fullPath = f.FullName;
				string relativePath = PathUtil.GetRelativePath(fullPath, srcDir);
				zipFilter.Append(relativePath).Append("$;");
				fileListStr.Append(DownloadList.ZIP_ENTRY_HEADER);
				AddDigest(fileListStr, relativePath, fullPath);
			}
			// add remove list
			foreach (string r in removedFiles) {
				fileListStr.Append(DownloadList.DELETE_HEADER).Append(r).Append("\n");
			}
			zipFilter.Replace(@"\", @"\\");
			
			// Zipping
			TextReplacer replace = new TextReplacer();
			replace.AddReplaceToken(@"\(", @"\(");
			replace.AddReplaceToken(@"\)", @"\)");
			replace.AddReplaceToken(@"\+", @"\+");
			replace.AddReplaceToken(@"\.", @"\.");
			ZipEntryFactory ef = zip.EntryFactory as ZipEntryFactory;
			ef.IsUnicodeText = true;
			string filter = replace.Replace(zipFilter.ToString());
			UnityEngine.Debug.Log("srcdir:"+srcDir);
			UnityEngine.Debug.Log("filter:"+filter);
			zip.CreateZip(zipPath, srcDir, true, filter);
			if (deleteSrcFiles) {
				foreach (string deletePath in paths) {
					File.Delete(PathUtil.Combine(PathUtil.Combine(srcDir, deletePath)));
				}
			}
			File.WriteAllText(PathUtil.ReplaceExtension(zipPath, ".bytes"), fileListStr.ToString());
		}

		private static void AddDigest(StringBuilder str, string assetPath, string fullPath) {
			str.Append(assetPath).Append(Downloader.VER_SEPARATOR).Append(MD5.Digest(new FileStream(fullPath, FileMode.Open, FileAccess.Read))).Append("\n");
		}
	}
}

#endif