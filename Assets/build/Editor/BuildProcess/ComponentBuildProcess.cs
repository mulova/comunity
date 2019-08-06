using System.Collections.Generic;
using Object = UnityEngine.Object;
using System;
using UnityEngine;
using UnityEditor;
using mulova.commons;
using comunity;
using System.Text.Ex;
using System.Collections.Generic.Ex;
using UnityEngine.Ex;

namespace build
{
	public abstract class ComponentBuildProcess
	{
		public abstract Type compType { get; }

		protected abstract void VerifyComponent(Component comp);

		protected abstract void PreprocessComponent(Component comp);

		protected abstract void PreprocessOver(Component comp);

		private HashSet<string> errors = new HashSet<string>();
		private Object currentObj;
		private object[] options;
		protected bool isCdnAsset 
		{ 
			get
			{
				return AssetBundlePath.inst.IsCdnAsset(currentObj);
			}
		}

		public virtual string title
		{
			get
			{
				return GetType().Name;
			}
		}

		public bool isAsset
		{
			get
			{
				return AssetDatabase.GetAssetPath(currentObj).IsNotEmpty();
			}
		}

		public string path
		{
			get
			{
				string p = AssetDatabase.GetAssetPath(currentObj);
				if (p != null)
				{
					return p;
				}
				if (currentObj is GameObject)
				{
					return string.Format("[{0}]{1}", EditorSceneBridge.currentScene, (currentObj as GameObject).transform.GetScenePath());
				}
				return EditorSceneBridge.currentScene;
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

		public void Verify(Object obj, Component comp, object[] options)
		{
			if (compType == null ^ comp == null)
			{
				return;
			}
			if (compType != null && !compType.IsAssignableFrom(comp.GetType()))
			{
				return;
			}
			try
			{
				this.currentObj = obj;
				this.options = options;
				VerifyComponent(comp);
			} catch (Exception ex)
			{
				errors.Add(string.Concat(path, "\n", ex.Message, "\n", ex.StackTrace));
			}
		}

		public void Preprocess(Object obj, Component comp, object[] options)
		{
			if (compType == null ^ comp == null)
			{
				return;
			}
			if (compType != null && !compType.IsAssignableFrom(comp.GetType()))
			{
				return;
			}
			try
			{
				this.currentObj = obj;
				this.options = options;
				if (!IsOption(BuildScript.VERIFY_ONLY))
				{
					PreprocessComponent(comp);
				}
			} catch (Exception ex)
			{
				errors.Add(string.Concat(path, "\n", ex.Message, "\n", ex.StackTrace));
			}
		}

		public void PreprocessOver(Object obj, Component comp, object[] options)
		{
			if (compType == null ^ comp == null)
			{
				return;
			}
			if (compType != null && !compType.IsAssignableFrom(comp.GetType()))
			{
				return;
			}
			try
			{
				this.currentObj = obj;
				this.options = options;
				if (!IsOption(BuildScript.VERIFY_ONLY))
				{
					PreprocessOver(comp);
				}
			} catch (Exception ex)
			{
				errors.Add(string.Concat(path, "\n", ex.Message, "\n", ex.StackTrace));
			}
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

		public static string GetErrorMessages()
		{
			if (processPool != null)
			{
				List<string> errors = new List<string>();
				HashSet<ComponentBuildProcess> processes = new HashSet<ComponentBuildProcess>();
				foreach (KeyValuePair<Type, List<ComponentBuildProcess>> entry in processPool)
				{
					if (entry.Value != null)
					{
						processes.AddAll(entry.Value);
					}
				}
				foreach (var p in processes)
				{
					string err = p.GetErrorMessage();
					if (err.IsNotEmpty())
					{
						errors.Add(err);
					}
				}

				return errors.Join("\n");
			} else
			{
				return null;
			}
		}

		private static MultiMap<Type, ComponentBuildProcess> processPool;

		private static List<ComponentBuildProcess> GetBuildProcessor(Type type)
		{
			// collect BuildProcessors
			if (processPool == null)
			{
				processPool = new MultiMap<Type, ComponentBuildProcess>();
				List<Type> bps = ReflectionUtil.FindClasses<ComponentBuildProcess>();
				foreach (Type t in bps)
				{
					if (!t.IsAbstract)
					{
						ComponentBuildProcess b = Activator.CreateInstance(t) as ComponentBuildProcess;
						processPool.Add(b.compType, b);
					}
				}
			}
			if (processPool.ContainsKey(type))
			{
				return processPool[type];
			} else
			{
				// handle inheritance
				Type baseType = type;
				while (baseType != null)
				{
					if (processPool.ContainsKey(baseType))
					{
						List<ComponentBuildProcess> b = processPool.GetSlot(baseType);
						if (b != null)
						{
							processPool.AddRange(type, b);
						}
						return b;
					}
					baseType = baseType.BaseType;
				}
				processPool[type] = null;
				return null;
			}
		}

		protected void SetDirty(Object o)
		{
			EditorTraversal.SetDirty(o);
		}

		public static void Reset()
		{
			processPool = null;
		}

		public static void VerifyComponents(Object obj, params object[] options)
		{
			ProcessComponents((p,o,c,op)=>p.Verify(o, c, op), obj, options);
		}

		public static void PreprocessComponents(Object obj, params object[] options)
		{
			ProcessComponents((p,o,c,op)=>p.Preprocess(o, c, op), obj, options);
		}

		public static void PreprocessOver(Object obj, params object[] options)
		{
			ProcessComponents((p,o,c,op)=>p.PreprocessOver(o, c, op), obj, options);
		}

		private static void ProcessComponents(Action<ComponentBuildProcess, Object, Component, object[]> action, Object obj, params object[] options)
		{
			if (obj is Component)
			{
				Component c = obj as Component;
				if (!c.CompareTag("EditorOnly"))
				{
					List<ComponentBuildProcess> processes = GetBuildProcessor(c.GetType());
					if (processes != null)
					{
						foreach (ComponentBuildProcess p in processes)
						{
							action(p, obj, c, options);
						}
					}
				}
			} else if (obj is GameObject)
			{
				foreach (Component c in (obj as GameObject).GetComponentsInChildren<Component>(true))
				{
					if (c != null&&!c.CompareTag("EditorOnly"))
					{
						List<ComponentBuildProcess> processes = GetBuildProcessor(c.GetType());
						if (processes != null)
						{
							foreach (var p in processes)
							{
								action(p, obj, c, options);
							}
						}
					} else
					{
						// missing component
					}
				}
			}
		}
	}
}
