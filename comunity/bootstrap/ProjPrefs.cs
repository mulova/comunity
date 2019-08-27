using UnityEngine;

namespace comunity
{
    public static class ProjPrefs
    {
	    private static string _prjName;
	    public static string prjName
	    {
		    get
		    {
			    if (_prjName == null)
			    {
				    string[] s = Application.dataPath.Split('/');
				    _prjName = s[s.Length - 2];
			    }
			    return _prjName;
		    }
	    }

        public static bool skipDownload
        {
            get
            {
                return GetBool(nameof(skipDownload), false);
            }
            set
            {
                SetBool(nameof(skipDownload), value);
            }
        }

        public static string device
        {
            get
            {
                return GetString(nameof(device), "");
            }
            set
            {
			    SetString(nameof(device), value);
            }
        }

        public static string GetPrjKey(string key)
	    {
		    return prjName + key;
	    }

	    public static void SetString(string key, string val)
	    {
    #if UNITY_EDITOR
		    UnityEditor.EditorPrefs.SetString(GetPrjKey(key), val);
    #endif
	    }

	    public static void SetInt(string key, int val)
	    {
    #if UNITY_EDITOR
		    UnityEditor.EditorPrefs.SetInt(GetPrjKey(key), val);
    #endif
	    }

	    public static void SetFloat(string key, float val)
	    {
    #if UNITY_EDITOR
		    UnityEditor.EditorPrefs.SetFloat(GetPrjKey(key), val);
    #endif
	    }

	    public static void SetBool(string key, bool val)
	    {
    #if UNITY_EDITOR
		    UnityEditor.EditorPrefs.SetBool(GetPrjKey(key), val);
    #endif
	    }

	    public static string GetString(string key, string val)
	    {
    #if UNITY_EDITOR
		    return UnityEditor.EditorPrefs.GetString(GetPrjKey(key), val);
    #else
		    return val;
    #endif
	    }

	    public static int GetInt(string key, int val)
	    {
    #if UNITY_EDITOR
		    return UnityEditor.EditorPrefs.GetInt(GetPrjKey(key), val);
    #else
		    return val;
    #endif
	    }

	    public static float GetFloat(string key, float val)
	    {
    #if UNITY_EDITOR
		    return UnityEditor.EditorPrefs.GetFloat(GetPrjKey(key), val);
    #else
		    return val;
    #endif
	    }

	    public static bool GetBool(string key, bool val)
	    {
    #if UNITY_EDITOR
		    return UnityEditor.EditorPrefs.GetBool(GetPrjKey(key), val);
    #else
		    return val;
    #endif
	    }
    }
}

