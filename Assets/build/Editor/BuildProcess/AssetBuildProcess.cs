using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.Text.RegularExpressions;
using mulova.commons;
using comunity;
using System.Text.Ex;
using System.Collections.Generic.Ex;

namespace build
{
	public abstract class AssetBuildProcess : Loggable
	{
		protected abstract void PreprocessAsset(string path, Object obj);

		protected abstract void VerifyAsset(string path, Object obj);

		public readonly Type assetType;

		private string title;
		private List<string> errors = new List<string>();
		private RegexMgr excludeExp = new RegexMgr();
		private RegexMgr includeExp = new RegexMgr();
		private object[] options;

		private static List<AssetBuildProcess> pool;

		private Regex excludePath
		{
			get
			{
				return excludeExp.exp;
			}
		}

		private Regex includePath
		{
			get
			{
				return includeExp.exp;
			}
		}

		public AssetBuildProcess(string errorTitle, Type assetType)
		{
			this.title = errorTitle;
			this.assetType = assetType;
		}

		public void Preprocess(string path, Object obj, params object[] options)
		{
			try
			{
				if (obj != null&&!assetType.IsAssignableFrom(obj.GetType()))
				{
					return;
				}
				if (excludePath != null&&excludePath.IsMatch(path))
				{
					return;
				}
				if (includePath != null&&!includePath.IsMatch(path))
				{
					return;
				}
				this.options = options;
				VerifyAsset(path, obj);
				if (!IsOption(BuildScript.VERIFY_ONLY))
				{
					PreprocessAsset(path, obj);
				}
			} catch (Exception ex)
			{
				errors.Add(string.Concat(path, "\n", ex.Message, "\n", ex.StackTrace));
			}
		}

		public bool IsOption(object o)
		{
			if (options == null)
			{
				return false;
			}
			foreach (object option in options)
			{
				if (option == o)
				{
					return true;
				}
			}
			return false;
		}

		public T GetOption<T>()
		{
			if (options != null)
			{
				foreach (object option in options)
				{
					if (option is T)
					{
						return (T)option;
					}
				}
			}
			return default(T);
		}

		protected void AddError(string msg)
		{
			if (msg.IsNotEmpty())
			{
				errors.Add(msg);
			}
		}

		protected void AddErrorConcat(params string[] msg)
		{
			errors.Add(string.Concat(msg));
		}

		protected void AddErrorFormat(string format, params object[] param)
		{
			errors.Add(string.Format(format, param));
		}

		public string GetErrorMessage()
		{
			if (errors.IsNotEmpty())
			{
				return string.Format("{0}: {1}", title, errors.Join(", "));
			} else
			{
				return string.Empty;
			}
		}

		public void AddExcludePattern(string regexPattern)
		{
			excludeExp.AddPattern(regexPattern);
		}

		public void AddIncludePattern(string regexPattern)
		{
			includeExp.AddPattern(regexPattern);
		}

		public static string JoinErrorMessage(AssetBuildProcess[] processors)
		{
			List<string> errors = new List<string>();
			if (processors != null)
			{
				foreach (var proc in processors)
				{
					string err = proc.GetErrorMessage();
					if (err.IsNotEmpty())
					{
						errors.Add(err);
					}
				}
			}
			return errors.Join("\n");
		}

		private static List<AssetBuildProcess> GetBuildProcessors()
		{
			// collect BuildProcessors
			if (pool == null)
			{
				pool = new List<AssetBuildProcess>();
				List<Type> bps = ReflectionUtil.FindClasses<AssetBuildProcess>();
				foreach (Type t in bps)
				{
					if (!t.IsAbstract)
					{
						AssetBuildProcess b = Activator.CreateInstance(t) as AssetBuildProcess;
						pool.Add(b);
					}
				}
			}
			return pool;
		}

		public static void PreprocessAssets(string path, Object obj, params object[] options)
		{
			foreach (AssetBuildProcess p in GetBuildProcessors())
			{
				p.Preprocess(path, obj, options);
			}
		}

		public static string GetErrorMessages()
		{
			List<string> errors = new List<string>();
			foreach (AssetBuildProcess p in GetBuildProcessors())
			{
				string err = p.GetErrorMessage();
				if (err.IsNotEmpty())
				{
					errors.Add(err);
				}
			}

			return errors.Join("\n");
		}

		public static void Reset()
		{
			pool = null;
		}

		public void Verify(List<Object> list, TexFormatGroup texFormat)
		{
			if (list.IsNotEmpty())
			{
				AssetBuildProcess.Reset();
				foreach (var o in list)
				{
					AssetBuildProcess.PreprocessAssets(AssetDatabase.GetAssetPath(o), o, BuildScript.VERIFY_ONLY, texFormat);
				}
				string verifyError = AssetBuildProcess.GetErrorMessages();
				if (verifyError.IsNotEmpty())
				{
					Debug.LogError(verifyError);
					EditorUtility.DisplayDialog("Verify Fails", verifyError, "OK");
				}
			}
		}

		protected void SetDirty(Object o)
		{
			EditorTraversal.SetDirty(o);
		}
	}
}
