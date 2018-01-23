using UnityEditor;
using UnityEngine;
using commons;

namespace core
{
    public class ScriptableObjectGen
    {
        [MenuItem("Assets/ScriptableObject")]
        public static void GenerateScriptableObject() {
            Object scriptableObj = Selection.activeObject;
            string selPath = AssetDatabase.GetAssetPath(scriptableObj);
            string path = PathUtil.GetDirectory(selPath);
            EditorAssetUtil.CreateScriptableObject(scriptableObj.name, path+scriptableObj.name+".asset");
        }
        
        [MenuItem("Assets/ScriptableObject", true)]
        public static bool IsGenerateScriptableObject() {
            if (Selection.activeObject != null && Selection.activeObject.GetType() == typeof(MonoScript)) {
                MonoScript script = Selection.activeObject as MonoScript;
                return script.GetClass().IsSubclassOf(typeof(ScriptableObject)) && !script.GetClass().IsSubclassOf(typeof(Editor));
            }
            return false;
        }
    }
}


