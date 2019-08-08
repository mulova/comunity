#if !UNITY_WEBGL
using UnityEngine;
using System.IO;
using mulova.commons;
using System.IO.Ex;

namespace comunity
{
    public class UniversalFileLogger : MonoBehaviour {
        public bool stackTrace;
        private StreamWriter writer;
        
        void OnEnable() {
            if (Application.isPlaying) {
                Application.logMessageReceived += WriteLog;
            }
        }
        
        private void WriteLog(string condition, string stack, LogType logType) {
            if (writer == null) {
                DirectoryInfo dir = null;
                if (Platform.isEditor) {
                    dir = new DirectoryInfo(Platform.dataPath);
                    dir = dir.Parent;
                    dir = new DirectoryInfo(Path.Combine(dir.FullName, "log"));
                } else {
                    dir = new DirectoryInfo(Path.Combine(Platform.downloadPath, "log"));
                }
                FileStream file = dir.CreateUniqueFile("log.txt");
                writer = new StreamWriter(file);
            }
            
            writer.WriteLine(condition);
            if (stackTrace) {
                writer.WriteLine(stack);
            }
            writer.Flush();
        }
        
        void OnDisable() {
            if (Application.isPlaying) {
                Application.logMessageReceived -= WriteLog;
            }
            if (writer != null) {
                writer.Close();
            }
            writer = null;
        }
    }
}
#endif