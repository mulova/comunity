#if UNITY_5_3_OR_NEWER
#define MULTI_SCENE
#endif

using UnityEditor.SceneManagement;

namespace mulova.comunity
{
	public static class EditorSceneBridge
	{
		public static string currentScene
		{
			get
			{
				#if MULTI_SCENE
				return EditorSceneManager.GetActiveScene().path;
				#else
				return EditorApplication.currentScene;
				#endif
			}
		}
		
		public static void OpenScene(string path)
		{
			#if MULTI_SCENE
			EditorSceneManager.OpenScene(path);
			#else
			EditorApplication.OpenScene(path);
			#endif
		}
		
		public static void MarkSceneDirty()
		{
			#if MULTI_SCENE
			EditorSceneManager.MarkAllScenesDirty();
			#else
			EditorApplication.MarkSceneDirty();
			#endif
		}
		
		public static bool isSceneDirty
		{
			get
			{
				#if MULTI_SCENE
				return EditorSceneManager.GetActiveScene().isDirty;
				#else
				return EditorApplication.isSceneDirty;
				#endif
			}
		}
		
		public static void SaveScene()
		{
			#if MULTI_SCENE
			EditorSceneManager.SaveOpenScenes();
			#else
			EditorApplication.SaveScene();
			#endif
		}
		
		public static bool SaveCurrentSceneIfUserWantsTo()
		{
			#if MULTI_SCENE
			return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			#else
			return EditorApplication.SaveCurrentSceneIfUserWantsTo();
			#endif
		}
		
	}
}


