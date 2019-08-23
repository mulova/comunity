using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildMode
{
	private const string PREF_KEY = "AssetBundlesEditorLocalMode";

#if UNITY_EDITOR
	public static string _prjName;
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

	public static string _prefKey;
	public static string prefKey
	{
		get
		{
			if (_prefKey == null)
			{
				string[] s = Application.dataPath.Split('/');
				_prefKey = s[s.Length - 2]+PREF_KEY;
			}
			return _prefKey;
		}
	}
#endif

	public static bool isDemo {
		get {
			return false && !isRelease;
		}
	}

	public static bool isEditor {
		get
		{
#if UNITY_EDITOR
			return UnityEditor.EditorPrefs.GetBool(prefKey, true);
#else
			return false;
#endif
		}
		set
		{
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetBool(prefKey, value);
#endif
		}
	}

	public static bool isPerfProfile
	{
		get
		{
			return false;
		}
	}

	public static bool isSimulation {
		get {
			#if UNITY_EDITOR
			return !isEditor;
			#else
			return false;
			#endif
		}
	}

	public static bool isVerbose
	{
		get
		{
#if SERVICE_ALPHA || SERVICE_BETA
			return true;
#else
			return false;
#endif
		}
	}

	/// <value><c>true</c> if is build or simulation mode; otherwise, <c>false</c>.</value>
	public static bool isBuild {
		get {
			return !isEditor;
		}
		set {
			isEditor = !value;
		}
	}

    public static bool isProduction
    {
        get
        {
#if SERVICE_ALPHA || SERVICE_BETA
            return false;
#else
			return true;
#endif
        }
    }

    public static bool isRelease {
		get {
			#if SERVICE_ALPHA || SERVICE_BETA || SERVICE_RC
			return false;
			#else
			return true;
			#endif
		}
	}

    public static void ReassignSceneShader(string sceneName)
	{
#if UNITY_EDITOR
		var s = SceneManager.GetSceneByName(sceneName);
		foreach (var r in s.GetRootGameObjects())
		{
			BuildMode.ReassignShader(r);
		}
#endif
	}
	public static void ReassignShader(Object o)
	{
#if UNITY_EDITOR
		GameObject go = null;
		if (o is GameObject) {
			go = (o as GameObject);
		} else if (o is Component) {
			go = (o as Component).gameObject;
		}
		if (go != null) {
			foreach (var r in go.GetComponentsInChildren<Renderer>(true)) {
				if (r.sharedMaterials != null) {
					foreach (var m in r.sharedMaterials) {
						ReassignMaterialShader(m);
					}
				}
			}
			foreach (var p in go.GetComponentsInChildren<Projector>(true)) {
				ReassignMaterialShader(p.material);
			}
		}
#endif
	}

	public static void ReassignMaterialShader(Material m)
	{
			if (m != null && m.shader != null) {
				m.shader = Shader.Find(m.shader.name);
			}
	}
}

