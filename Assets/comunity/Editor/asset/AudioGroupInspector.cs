using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Text;
using System.IO;
using commons;

namespace comunity
{
    [CustomEditor(typeof(AudioGroup))]
    public class AudioGroupInspector : Editor
    {
        private AudioGroup group;
        private string curClip;
        
        void OnEnable()
        {
            group = target as AudioGroup;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            bool changed = false;
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = group.assetDir.isValid&&!group.csv.isEmpty;
            if (GUILayout.Button("Regenerate", EditorStyles.toolbarButton))
            {
                ExportAudioCsv(group);
                changed = true;
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            if (changed)
            {
                CompatibilityEditor.SetDirty(group);
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtil.Popup<string>(ref curClip, new List<string>(group.clips));
                if (GUILayout.Button("Play"))
                {
                    group.Play(curClip);
                }
                if (GUILayout.Button("Stop"))
                {
                    group.Stop(curClip);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public static void ExportAudioCsv(AudioGroup group)
        {
            ExportAudioCsv(AssetDatabase.GUIDToAssetPath(group.assetDir.guid), group.csv.GetEditorPath());
            CompatibilityEditor.SetDirty(group);
        }

        public static void ExportAudioCsv(string audioFolder, string csvPath)
        {
            string[] paths = EditorAssetUtil.ListAssetPaths(audioFolder, FileType.Audio, true);
            HashSet<string> missingClips = new HashSet<string>();
            foreach (string p in paths)
            {
                missingClips.Add(EditorAssetUtil.GetAssetRelativePath(p));
            }

            StringBuilder csv = new StringBuilder();
            csv.Append("id,path,loop,interruptable,instanceCount,volume,length\n");
            // load csv and refresh clip length
            Dictionary<string, AudioClipData> dic = new Dictionary<string, AudioClipData>();
            if (File.Exists(csvPath))
            {
                TextAsset csvAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(csvPath);
                SpreadSheet ss = new SpreadSheet(csvAsset.bytes);
                ss.trimSpace = true;
                dic = ss.GetRowIndexer<string, AudioClipData>(d => d.key, 1);

                // refresh clip length
                foreach (var pair in dic)
                {
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/"+pair.Value.path);
                    if (clip != null)
                    {
                        pair.Value.length = clip.length;
                        missingClips.Remove(pair.Value.path);
                        csv.Append(pair.Value.ToCsv()).AppendLine();
                    }
                }
            }
            // save csv
            foreach (string p in missingClips)
            {
                string key = PathUtil.GetFileNameWithoutExt(p).ToLower();
                string path = PathUtil.Combine(PathUtil.GetDirectory(p), key+Path.GetExtension(p));
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/"+p);
                AudioClipData data = dic.Get(key);
                if (data == null)
                {
                    data = new AudioClipData();
                    data.key = key;
                    data.loop = false;
                    data.interruptable = true;
                }
                data.path = path;
                data.length = clip.length;
                csv.Append(data.ToCsv()).AppendLine();
            }
            File.WriteAllText(csvPath, csv.ToString());
            AssetDatabase.ImportAsset(csvPath, ImportAssetOptions.ForceUpdate);
        }
    }
}
