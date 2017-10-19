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

namespace core
{
    public static class EditorUtil
    {
        public static void OpenExplorer(string path)
        {
            if (Application.platform == RuntimePlatform.OSXEditor) {
                EditorUtility.RevealInFinder(path);
            } else {
                System.Diagnostics.Process.Start("explorer.exe", "/select,"+path.Replace(@"/", @"\"));
            }
        }
        
        public static string OpenFilePanel(string title, string ext)
        {
            string saved = EditorPrefs.GetString(title, "Assets/");
            string file = EditorUtility.OpenFilePanel(title, saved, ext);
            if (file.IsNotEmpty())
            {
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
            if (folder.IsNotEmpty())
            {
                EditorPrefs.SetString(title, folder);
            }
            return folder;
        }
        
        public static ExecOutput ExecuteCommand(string command, string param) {
            return ExecuteCommand(command, param, Encoding.Default);
        }
        /// <summary>
        /// Executes a shell command synchronously.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public static ExecOutput ExecuteCommand(string command, string param, Encoding enc)
        {
            try
            {
                //          Debug.Log("Running command: " + command);
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                string cmd = null;
                string prm = null;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    cmd = "cmd";
                    prm = string.Format("/c \"{0}\" {1}", command, param);
                } else
                {
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
            }
            catch (System.Exception e)
            {
                // Log the exception
                UnityEngine.Debug.LogError("Got exception: " + e.Message);
                return new ExecOutput(e);
            }
        }
    }
}


