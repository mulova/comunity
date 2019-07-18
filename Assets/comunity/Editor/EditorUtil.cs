//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEngine;
using commons;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEditor.SceneManagement;
using System.Text.Ex;

namespace comunity
{
	public static class EditorUtil
	{
        public static SceneView sceneView {
            get {
                if (SceneView.currentDrawingSceneView != null)
                {
                    return SceneView.currentDrawingSceneView;
                }
                if (SceneView.lastActiveSceneView != null)
                {
                    return SceneView.lastActiveSceneView;
                }
                if (SceneView.sceneViews.Count > 0)
                {
                    return (SceneView)SceneView.sceneViews[0];
                }
                return null;
            }
        }

		public static void OpenExplorer(string path)
		{
			if (Application.platform == RuntimePlatform.OSXEditor) {
				EditorUtility.RevealInFinder(path);
			} else {
				System.Diagnostics.Process.Start("explorer.exe", "/select," + path.Replace(@"/", @"\"));
			}
		}

		public static string OpenFilePanel(string title, string ext)
		{
			string saved = EditorPrefs.GetString(title, "Assets/");
			string file = EditorUtility.OpenFilePanel(title, saved, ext);
			if (file.IsNotEmpty()) {
				EditorPrefs.SetString(title, file);
			}
			return file;
		}

		public static string OpenFolderPanel(string title)
		{
			string saved = EditorPrefs.GetString(title, "Assets/");
			string parent = PathUtil.GetParent(saved);
			string dir = Path.GetFileName(saved);
			string folder = EditorUtility.OpenFolderPanel(title, parent, dir);
			if (folder.IsNotEmpty()) {
				EditorPrefs.SetString(title, folder);
			}
			return folder;
		}

		public static ExecOutput ExecuteCommand(string command, string param)
		{
			return ExecuteCommand(command, param, Encoding.Default);
		}

		/// <summary>
		/// Executes a shell command synchronously.
		/// </summary>
		/// <param name="command">string command</param>
		/// <returns>string, as output of the command.</returns>
		public static ExecOutput ExecuteCommand(string command, string param, Encoding enc)
		{
			try {
				//          Debug.Log("Running command: " + command);
				// create the ProcessStartInfo using "cmd" as the program to be run,
				// and "/c " as the parameters.
				// Incidentally, /c tells cmd that we want it to execute the command that follows,
				// and then exit.
				string cmd = null;
				string prm = null;
				if (Application.platform == RuntimePlatform.WindowsEditor) {
					cmd = "cmd";
					prm = string.Format("/c \"{0}\" {1}", command, param);
				} else {
					cmd = command;
					prm = param;
				}
				System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo(cmd, prm);
				Debug.LogFormat("Exec> {0} {1}", cmd, prm);
                
				// The following commands are needed to redirect the standard output.
				// This means that it will be redirected to the Process.StandardOutput StreamReader.
				procStartInfo.StandardErrorEncoding = enc;
				procStartInfo.StandardOutputEncoding = enc;
				procStartInfo.RedirectStandardOutput = true;
				procStartInfo.RedirectStandardError = true;
				procStartInfo.UseShellExecute = false;
				// Do not create the black window.
				procStartInfo.CreateNoWindow = true;
				// Now we create a process, assign its ProcessStartInfo and start it
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo = procStartInfo;
				proc.Start();
				// Get the output into a string
				string stdout = EncodingUtil.Encode(proc.StandardOutput.BaseStream, enc);
				string stderr = EncodingUtil.Encode(proc.StandardError.BaseStream, enc);
				return new ExecOutput(stdout, stderr);
			} catch (System.Exception e) {
				// Log the exception
				return new ExecOutput(e);
			}
		}

		public static bool DisplayProgressBar(string title, string info, float progress)
		{
			if (SystemInfo.graphicsDeviceID != 0) {
				return EditorUtility.DisplayCancelableProgressBar(title, info, progress);
			}
//            log.Debug("{0} ({1:P2})", info, progress);
			return false;
		}

		public static void SaveScene()
		{
			if (!EditorSceneBridge.currentScene.IsEmpty() && EditorSceneBridge.currentScene != "Untitled") {
				if (EditorSceneBridge.isSceneDirty) {
					EditorSceneBridge.SaveScene();
				}
			}
		}

		public static List<GameObject> GetSceneRoots()
		{
			var list = new List<GameObject>();
			for (int i = 0; i < EditorSceneManager.sceneCount; ++i) {
				list.AddRange(EditorSceneManager.GetSceneAt(i).GetRootGameObjects());
			}
			return list;
		}

		public static void SetDirty(Object o)
		{
			if (Application.isPlaying || o == null) {
				return;
			}
			GameObject go = null;
			if (o is GameObject) {
				go = o as GameObject;
			} else if (o is Component) {
				go = (o as Component).gameObject;
			}
			if (go != null && go.scene.IsValid()) {
				EditorSceneManager.MarkSceneDirty(go.scene);
			} else {
				UnityEditor.EditorUtility.SetDirty(o);
			}
		}

		public static void DisplayProgressBar<T>(string title, IList<T> list, Action<T> action)
		{
			try {
				for (int i = 0; i < list.Count; ++i) {
					if (EditorUtility.DisplayCancelableProgressBar(title, list[i].ToString(), i / (float)list.Count)) {
						return;
					}
					action(list[i]);
				}
			} finally {
				EditorUtility.ClearProgressBar();
			}
		}
	}
}


