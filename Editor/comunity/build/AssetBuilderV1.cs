using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.IO;
using System.Text;
using System.Text.Ex;
using mulova.commons;
using mulova.comunity;
using mulova.preprocess;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.build.v1
{
    public class AssetBuilderV1
	{
		public const string OUTPUT_ROOT = "_asset";
		public readonly string zone;
		public readonly string oldVersion;
		public readonly string newVersion;
		private string outputDir;
		private string rootDir;
		private TexFormatGroup texFormat;
		private List<string> modList = new List<string>();
		public static readonly Loggerx log = LogManager.GetLogger(typeof(AssetBuilderV1));
		public readonly BuildTarget buildTarget;

        private AssetBundlePath abPath = AssetBundlePath.inst;

		public string targetRes
		{
			get
			{
				return texFormat.GetAbCategory(buildTarget.ToRuntimePlatform());
			}
		}

		public string versionFile
		{
			get
			{
				return PathUtil.Combine(rootDir, string.Format("version_{0}.txt", targetRes));
			}
		}

		public AssetBuilderV1(string zone, BuildTarget buildTarget, TexFormatGroup format)
		{
			this.zone = zone;
			this.buildTarget = buildTarget;
			this.texFormat = format;
			int version = GetLatestVersion(zone, buildTarget, format);
			this.oldVersion = version.ToString("D3");
			this.newVersion = (version+1).ToString("D3");
			Init();
		}

		public AssetBuilderV1(string zone, BuildTarget buildTarget, TexFormatGroup format, string oldVer, string newVer)
		{
			this.zone = zone;
			this.buildTarget = buildTarget;
			this.texFormat = format;
			this.oldVersion = oldVer;
			this.newVersion = newVer;
			Init();
		}

		private void Init()
		{
			this.rootDir = GetRootDir(zone, buildTarget, texFormat);
			this.outputDir = PathUtil.Combine(rootDir, newVersion);
		}

		public static string GetRootDir(string zone, BuildTarget buildTarget, TexFormatGroup texFormat)
		{
			return Cdn.GetPath(OUTPUT_ROOT, zone, buildTarget.ToRuntimePlatform(), texFormat);
		}

		public string GetOutputDir()
		{
			return outputDir;
		}

		private AssetSnapshot GetPrevSnapshot()
		{
			string path = PathUtil.Combine(rootDir, string.Format("{0}/digest_{0}.txt", oldVersion));
			return new AssetSnapshot(path);
		}

		public static int GetLatestVersion(string zone, BuildTarget buildTarget, TexFormatGroup texGroup)
		{
			string rootDir = GetRootDir(zone, buildTarget, texGroup);
			if (!Directory.Exists(rootDir))
			{
				return 0;
			}
			int ver = 0;
			foreach (string d in Directory.GetDirectories(rootDir))
			{
				string lastDir = Path.GetFileName(d);

				int v = 0;
				if (int.TryParse(lastDir, out v))
				{
					ver = Math.Max(ver, v);
				}
			}
			return ver;
		}

		public static bool IsModified(string filePath, ref DateTime time)
		{
			DateTime modTime = File.GetLastWriteTime(filePath);
			if (modTime != time)
			{
				time = modTime;
				return true;
			} else
			{
				return false;
			}
		}


		/// <summary>
		/// Build the specified snapshotPath and outputDir.
		/// </summary>
		/// <param name="snapshotPath">Snapshot path. null if no previous snapshot exists</param>
		/// <param name="outputDir">Output dir.</param>
		public void Build()
		{
			BuildConfig.Reset();
            List<ObjRef> dirRefs = abPath.dirs;
            List<ObjRef> rawDirRefs = abPath.rawDirs;

			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
			{
				// WWW Caching only allows asset bundle.
				dirRefs.AddRange(rawDirRefs);
				rawDirRefs.Clear();
			}

			if (Directory.Exists(outputDir)&&!Directory.GetFiles(outputDir).IsEmpty())
			{
				throw new Exception(string.Format("Output directory {0} is not empty", outputDir));
			}
			AssetSnapshot snapshot = GetPrevSnapshot();
			try
			{
				List<string> assetMods = new List<string>();
				List<string> rawAssetMods = new List<string>();
				// get mods
                foreach (ObjRef dir in rawDirRefs)
				{
                    rawAssetMods.AddRange(GetRawAssetMods(snapshot, dir.path));
				}
                foreach (ObjRef dir in dirRefs)
				{
                    assetMods.AddRange(GetAssetMods(snapshot, dir.path));
				}
				string err1 = VerifyLowerCase(assetMods);
				string err2 = VerifyLowerCase(rawAssetMods);
				if (!err1.IsEmpty() || !err2.IsEmpty())
				{
					throw new Exception($"{err1}\n{err2}");

                }

				assetMods.RemoveAll(m=> rawAssetMods.Contains(m));
				log.Info("Modified raw assets ({0:D0})", rawAssetMods.Count);
				log.Info("Modified assets ({0:D0})", assetMods.Count);
				string[] srcList = assetMods.ConvertAll(p => "Assets/"+p).ToArray();
				// preprocess
				var buildLog = BuildScript.PrebuildAll(ProcessStage.Verify);
				if (!buildLog.isEmpty)
				{
                    throw new Exception(buildLog.ToString());
                }
                buildLog = BuildScript.PrebuildAssets(srcList);
                if (!buildLog.isEmpty)
                {
                    Debug.LogError(buildLog.ToString());
                    EditorUtility.DisplayDialog("Verify Fails", buildLog.ToString(), "OK");
                }
                // change texture formats
                texFormat.ConvertDependencies(srcList);
				// build
				GenerateRawAsset(rawAssetMods, outputDir);
				GenerateAsset(assetMods, outputDir);
				assetMods.AddRange(GenerateStreamingScene(outputDir));
				AssetDatabase.SaveAssets();
				modList.Clear();
				modList.AddRange(assetMods);
				modList.AddRange(rawAssetMods);

			} catch (Exception ex)
			{
				log.Error(ex);
				throw ex;
			}

			if (!modList.IsEmpty())
			{
				SaveSnapshot(snapshot);
			} else
			{
				if (File.Exists(versionFile))
				{
					File.Delete(versionFile);
				}
				Debug.LogError("No resource change found");
			}
		}

		private void SaveSnapshot(AssetSnapshot snapshot)
		{
			string savePath = PathUtil.Combine(outputDir, string.Format("digest_{0}.txt", newVersion));
			string modPath = PathUtil.Combine(outputDir, string.Format("modlist_{0}.txt", newVersion));
			string verPath = PathUtil.Combine(outputDir, string.Format("modver_{0}.txt", newVersion));
			snapshot.SaveDigest(savePath, verPath);
			// save mod list
			File.WriteAllText(modPath, modList.Join("\n"));
			// save version at file
			if (!oldVersion.IsEmpty())
			{
				File.WriteAllText(versionFile, string.Format("{0} {1}\n", oldVersion, newVersion));
			}
		}

		private string VerifyLowerCase(List<string> mods)
		{
			StringBuilder str = new StringBuilder();
			foreach (var m in mods)
			{
				string err = AssetCache.VerifyUrl(m);
				if (!err.IsEmpty())
				{
					str.Append(err).AppendLine();
				}
			}
			return str.ToString();
		}

		private List<string> GetAssetMods(AssetSnapshot snapshot, string srcDir)
		{
			List<string> mods = new List<string>();
			string[] assetPaths = EditorAssetUtil.ListAssetPaths(srcDir, FileType.All, true);
			foreach (string assetPath in assetPaths)
			{
				if (assetPath.EndsWithIgnoreCase(".meta")
					||Path.GetFileName(assetPath).StartsWith(".", StringComparison.Ordinal)
					||assetPath.EndsWithIgnoreCase(".unity"))
				{
					continue;
				}

				AssetDigest digest = snapshot.GetAssetDigest(assetPath);
				bool modified = IsDepModified(snapshot, digest);

				if (modified)
				{
					mods.Add(assetPath);
				}
			}
			return mods;
		}

		private void GenerateAsset(List<string> modList, string outputDir)
		{
			if (modList.IsEmpty())
			{
				return;
			}
			foreach (string assetPath in modList)
			{
				string dstPath = GetOutputPath(outputDir, assetPath);
				dstPath = PathUtil.ReplaceExtension(dstPath, FileTypeEx.ASSET_BUNDLE);
				Object obj = AssetDatabase.LoadAssetAtPath("Assets/"+assetPath, typeof(Object));
				if (obj != null)
				{
					AssetBundleBuilder.BuildAssetBundle(dstPath, buildTarget, obj, Path.GetFileNameWithoutExtension(assetPath));
					log.Debug("Create AssetBundle '{0}'", dstPath);
				} else
				{
					throw new Exception("Invalid asset "+assetPath);
				}
			}
		}

		private List<string> GenerateStreamingScene(string outputDir)
		{
			List<string> paths = new List<string>();
			if (BuildConfig.STREAMING_SCENE_FROM <= 0)
			{
				return paths;
			}
			BuildOptions option = BuildOptions.BuildAdditionalStreamedScenes | BuildOptions.StrictMode;
			for (int i=BuildConfig.STREAMING_SCENE_FROM; i<EditorBuildSettings.scenes.Length; ++i)
			{
				EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
				if (scene.enabled)
				{
					string dstPath = GetOutputPath(outputDir, Path.GetFileNameWithoutExtension(scene.path)+FileTypeEx.ASSET_BUNDLE);
					paths.Add(dstPath);
					BuildPipeline.BuildStreamedSceneAssetBundle(new string[] { scene.path }, dstPath, buildTarget, option);
				}
			}
			return paths;
		}

		private List<string> GetRawAssetMods(AssetSnapshot snapshot, string srcDir)
		{
			List<string> mod = new List<string>();
			foreach (string assetPath in EditorAssetUtil.ListAssetPaths(srcDir, FileType.All, true))
			{
				if (assetPath.EndsWithIgnoreCase(".meta")||Path.GetFileName(assetPath).StartsWith(".", StringComparison.Ordinal))
				{
					continue;
				}

				AssetDigest digest = snapshot.GetAssetDigest(assetPath);
				if (digest.IsModified())
				{
					mod.Add(assetPath);
				}
			}
			return mod;
		}

		private void GenerateRawAsset(List<string> modList, string outputDir)
		{
			if (modList.IsEmpty())
			{
				return;
			}

			foreach (string assetPath in modList)
			{
				if (assetPath.Is(FileType.Prefab, FileType.Asset))
				{
					throw new Exception(assetPath+" is not raw asset");
				}
				string srcPath = "Assets/"+assetPath;
				string dstPath = GetOutputPath(outputDir, assetPath);
				DirectoryInfo dstDir = new DirectoryInfo(PathUtil.GetDirectory(dstPath));
				if (!dstDir.Exists)
				{
					dstDir.Create();
				}
				log.Debug("Copy {0} to {1}", srcPath, dstPath);
				File.Copy(srcPath, dstPath, true);

				// add texture property
				if (srcPath.Is(FileType.Image))
				{
					TextureImporter im = AssetImporter.GetAtPath(srcPath) as TextureImporter;
					Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(srcPath);
					if (im != null)
					{
						TexData data = new TexData();
						data.crunch = im.crunchedCompression;
						data.filterMode = tex.filterMode;
						data.linear = !im.sRGBTexture;
						data.mipmap = im.mipmapEnabled;
						data.wrapMode = tex.wrapMode;
						TexData.Save(dstPath, data);
					}
				}
			}
		}

		private string GetOutputPath(string rootPath, string assetPath)
		{
			string fullPath = PathUtil.Combine(rootPath, assetPath);
			string filename = Path.GetFileName(fullPath);
			string dir = PathUtil.GetDirectory(fullPath);
			return PathUtil.Combine(dir, filename.ToLower());
		}

		static bool IsDepModified(AssetSnapshot snapshot, AssetDigest digest)
		{
			string[] dependPaths = AssetDatabase.GetDependencies(new string[] { "Assets/"+digest.path });
			bool modified = digest.IsModified();
			// check dependencies midified
			for (int i = 0; i < dependPaths.Length; ++i)
			{
				dependPaths[i] = EditorAssetUtil.GetAssetRelativePath(dependPaths[i]);
				if (digest.path == dependPaths[i])
				{
					continue;
				}
				AssetDigest depDigest = snapshot.GetDependDigest(dependPaths[i]);
				modified |= depDigest.IsModified();
				if (depDigest.path.Is(FileType.Asset, FileType.Prefab))
				{
					modified |= IsDepModified(snapshot, depDigest);
				}
			}
			return modified;
		}

		public override string ToString()
		{
			return zone;
		}
	}

	class AssetSnapshot
	{
		private Dictionary<string, AssetDigest> assets = new Dictionary<string, AssetDigest>();
		private Dictionary<string, AssetDigest> depends = new Dictionary<string, AssetDigest>();
		private const string DEPEND_SUFFIX = "_dep";

		public AssetSnapshot()
		{
		}

		public AssetSnapshot(string snapshotPath)
		{
			LoadSnapshot(assets, snapshotPath);
			string dependPath = PathUtil.AddFileSuffix(snapshotPath, DEPEND_SUFFIX);
			LoadSnapshot(depends, dependPath);
		}

		private void LoadSnapshot(Dictionary<string, AssetDigest> snapshot, string path)
		{
			if (!File.Exists(path))
			{
				return;
			}
			foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
			{
				AssetDigest a = new AssetDigest(line);
				if (a.IsValid())
				{
					snapshot[a.path] = a;
				}
			}
		}

		public AssetDigest GetAssetDigest(string path)
		{
			return GetDigest(assets, path);
		}

		public AssetDigest GetDependDigest(string path)
		{
			return GetDigest(depends, path);
		}

		private AssetDigest GetDigest(Dictionary<string, AssetDigest> snapshot, string path)
		{
			AssetDigest d = snapshot.Get(path);
			if (d == null)
			{
				d = new AssetDigest();
				d.path = path;
				snapshot[path] = d;
			}
			return d;
		}

		public void SaveDigest(string digestPath, string verPath)
		{
			string dir = PathUtil.GetDirectory(digestPath);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			StringBuilder digestStr = new StringBuilder();
			StringBuilder verStr = new StringBuilder();
			foreach (AssetDigest d in assets.Values)
			{
				if (!d.IsDeleted())
				{
					digestStr.Append(d.ToString()).AppendLine();
					verStr.Append(d.ToVerString()).AppendLine();
				}
			}
			File.WriteAllText(digestPath, digestStr.ToString());
			File.WriteAllText(verPath, verStr.ToString());
			StringBuilder dependStr = new StringBuilder();
			foreach (AssetDigest d in depends.Values)
			{
				if (!d.IsDeleted())
				{
					dependStr.Append(d.ToString()).AppendLine();
				}
			}
			File.WriteAllText(PathUtil.AddFileSuffix(digestPath, DEPEND_SUFFIX), dependStr.ToString());
		}
	}

	[Serializable]
	public class AssetDigest
	{
		public string path;
		public long timestamp;
		public string assetHash;
		public string metaHash;
		public int version;
		private BoolType modified = BoolType.Null;

		private string fullPath
		{
			get { return PathUtil.Combine(Application.dataPath, path); }
		}

		public AssetDigest()
		{
		}

		public AssetDigest(string line)
		{
			if (!line.IsEmpty())
			{
				string[] tok = line.Split(Cdn.VER_SEPARATOR);
				if (tok.Length > 1)
				{
					path = tok[0];
					long.TryParse(tok[1], out timestamp);
				}
				if (tok.Length > 2)
				{
					assetHash = tok[2];
				}
				if (tok.Length > 3)
				{
					metaHash = tok[3];
				}
				if (tok.Length > 4)
				{
					version = int.Parse(tok[4]);
				}
			}
		}

		public bool IsDeleted()
		{
			return modified == BoolType.Null;
		}

		public bool IsModified()
		{
			if (modified == BoolType.Null)
			{
				FileInfo file = new FileInfo(fullPath);
				FileInfo metafile = new FileInfo(fullPath+".meta");
				if (metafile.Exists)
				{
					string newMetaHash = GetMetaDigest(metafile);
					modified = modified.Or(metaHash != newMetaHash);
					metaHash = newMetaHash;
				} else
				{
					metaHash = string.Empty;
				}
				if (!modified.IsTrue()&&timestamp == file.LastWriteTimeUtc.Ticks)
				{ // fast check
					modified = BoolType.False;
				} else
				{
					timestamp = file.LastWriteTimeUtc.Ticks;
                    using (var s = file.Open(FileMode.Open, FileAccess.Read))
                    {
                        string newAssetHash = s.ComputeHash();
                        modified = modified.Or(assetHash != newAssetHash);
                        assetHash = newAssetHash;
                    }
				}
			}
			if (modified.IsTrue())
			{
				version++;
			}
			return modified.IsTrue();
		}

		StringBuilder metaTemp = new StringBuilder(10240);

		private string GetMetaDigest(FileInfo metafile)
		{
			metaTemp.Length = 0;
			StreamReader r = new StreamReader(metafile.Open(FileMode.Open, FileAccess.Read));
			while (!r.EndOfStream)
			{
				string line = r.ReadLine();
				if (!line.StartsWith("timeCreated:")&&!line.StartsWith("licenseType:"))
				{
					metaTemp.Append(line);
				}
			}
			r.Close();
			return metaTemp.ToString().ComputeHash();
		}

		public bool IsValid()
		{
			return !path.IsEmpty()&&timestamp != 0;
		}

		public override string ToString()
		{
			return string.Join(Cdn.VER_SEPARATOR.ToString(), path, timestamp, assetHash, metaHash, version);
		}

		public string ToVerString()
		{
			return string.Join(Cdn.VER_SEPARATOR.ToString(), path, version);
		}

		public override bool Equals(object obj)
		{
			if (obj is AssetDigest)
			{
				AssetDigest that = obj as AssetDigest;
				return this.path == that.path
					&&this.assetHash == that.assetHash
					&&this.metaHash == that.metaHash;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return path.GetHashCode();
		}
	}
}
