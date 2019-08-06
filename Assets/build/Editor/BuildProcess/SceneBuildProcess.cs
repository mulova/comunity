using System.Collections.Generic;
using Object = UnityEngine.Object;
using System;
using UnityEngine;
using UnityEditor;
using mulova.commons;
using System.Text.Ex;
using System.Collections.Generic.Ex;

namespace build
{
    public abstract class SceneBuildProcess
    {
        protected abstract void VerifyScene(IEnumerable<Transform> sceneRoots);
        protected abstract void PreprocessScene(IEnumerable<Transform> sceneRoots);
        
        private List<string> errors = new List<string>();
        private object[] options;
        protected string scene { get; private set; }

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
        
        public void Preprocess(IEnumerable<Transform> sceneRoots, object[] options)
        {
            try
            {
                this.options = options;
                this.scene = scene;
                VerifyScene(sceneRoots);
				if (IsOption(BuildScript.VERIFY_ONLY))
                {
                    PreprocessScene(sceneRoots);
                }
            } catch (Exception ex)
            {
                errors.Add(string.Concat(scene, "\n", ex.Message, "\n", ex.StackTrace));
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
                return string.Format("{0}: {1}", scene, errors.Join(", "));
            } else
            {
                return string.Empty;
            }
        }
        
        public static string GetErrorMessages()
        {
            List<string> errors = new List<string>();
            foreach (SceneBuildProcess p in processPool)
            {
                string err = p.GetErrorMessage();
                if (err.IsNotEmpty())
                {
                    errors.Add(err);
                }
            }
            return errors.Join("\n");
        }

        private static List<SceneBuildProcess> pool;

        protected static List<SceneBuildProcess> processPool
        {
            get
            {
                // collect BuildProcessors
                if (pool == null)
                {
                    pool = new List<SceneBuildProcess>();
                    foreach (Type t in ReflectionUtil.FindClasses<SceneBuildProcess>())
                    {
                        if (!t.IsAbstract)
                        {
                            SceneBuildProcess b = Activator.CreateInstance(t) as SceneBuildProcess;
                            pool.Add(b);
                        }
                    }
                }
                return pool;
            }
        }

        public static void Reset()
        {
            pool = null;
        }

        public static void PreprocessScenes(IEnumerable<Transform> sceneRoots, params object[] options)
        {
            foreach (SceneBuildProcess p in processPool)
            {
                p.Preprocess(sceneRoots, options);
            }
        }

    }
}
