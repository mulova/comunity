using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using System.Text;
using commons;


namespace core
{
    public class AudioTriggerInspectorImpl
    {
        public bool showLabel = true;
        public readonly AudioTrigger trigger;
        private AudioDataTable[] tables;
        private AudioDataTable selected;
        private string[] clips = new string[0];
        private static readonly string AUDIO_TRIGGER = "Assets/inspector_settings/audio_trigger.bytes";

        public AudioTriggerInspectorImpl()
        {
            this.tables = LoadTables();
        }

        public AudioTriggerInspectorImpl(AudioTrigger trigger)
        {
            this.trigger = trigger;
            this.tables = LoadTables();
            InitTables();
        }

        public AudioTriggerInspectorImpl(AudioTrigger trigger, AudioDataTable[] tables)
        {
            this.trigger = trigger;
            this.tables = tables;
            InitTables();
        }

        private void InitTables()
        {
            foreach (AudioDataTable t in tables)
            {
                string guid = AssetDatabase.AssetPathToGUID("Assets/"+t.path);
                if (guid == trigger.audioGroupGuid)
                {
                    SelectTable(t);
                    break;
                }
            }
        }

        public void SelectTable(AudioDataTable t)
        {
            trigger.audioGroupGuid = t!= null? AssetDatabase.AssetPathToGUID("Assets/"+t.path): null;
            this.selected = t;
            if (t != null&&t.keys != null)
            {
                clips = new List<string>(t.keys).ToArray();
            } else
            {
                clips = new string[0];
            }
        }

        public static AudioDataTable[] LoadTables()
        {
            TextAsset tablesAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(AUDIO_TRIGGER);
            if (tablesAsset != null)
            {
                List<AudioDataTable> list = new List<AudioDataTable>();
                LineParser parser = new LineParser(false);
                foreach (string guid in parser.Parse(tablesAsset.bytes, Encoding.UTF8))
                {
                    string path = EditorAssetUtil.GetAssetRelativePath(AssetDatabase.GUIDToAssetPath(guid));
                    if (path.IsNotEmpty())
                    {
                        var table = new AudioDataTable(path);
                        table.loader = new EditorAssetLoader().GetBytes;
                        list.Add(table);
                    }
                }
                return list.ToArray();
            } else
            {
                return new AudioDataTable[0];
            }
        }

        private void SaveTableList()
        {
            List<string> list = new List<string>();
            foreach (AudioDataTable t in tables)
            {
                if (t != null)
                {
                    list.Add(AssetDatabase.AssetPathToGUID("Assets/"+t.path));
                }
            }
            File.WriteAllLines(AUDIO_TRIGGER, list.ToArray());
        }

        public void DrawManageTableGUI()
        {
            TextAsset asset = null;
            if (EditorGUIUtil.ObjectField("Add Audio Table csv", ref asset, false))
            {
                string path = EditorAssetUtil.GetAssetRelativePath(asset);
                AudioDataTable table = new AudioDataTable(path);
                ArrayUtil.Add(ref tables, table);
                SaveTableList();
            }
        }

        public void OnInspectorGUI()
        {
            DrawInspectorGUI();
        }

        public bool DrawInspectorGUI()
        {
            if (DrawInspectorGUI(ref selected, ref trigger.clip))
            {
                SelectTable(selected);
                CompatibilityEditor.SetDirty(trigger);
                return true;
            }
            return false;
        }

        public bool DrawInspectorGUI(ref AudioDataTable table, ref string clip)
        {
            bool changed = false;
            // to lower
            if (clip.IsNotEmpty())
            {
                string c2 = clip.ToLower();
                if (c2 != clip)
                {
                    clip = c2;
                    changed = true;
                }
            }
            if (EditorGUIUtil.PopupNullable(showLabel? "Table" : null, ref table, tables, ObjToString.DefaultToString))
            {
                if (table != null && table.keys != null)
                {
                    clips = new List<string>(table.keys).ToArray();
                } else
                {
                    clips = new string[0];
                }
                changed = true;
            }
            if (EditorGUIUtil.PopupNullable(showLabel? "Clip" : null, ref clip, clips))
            {
                changed = true;
            }
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Play"))
                {
                    trigger.Play();
                }
            }
            return changed;
        }

    }
}
