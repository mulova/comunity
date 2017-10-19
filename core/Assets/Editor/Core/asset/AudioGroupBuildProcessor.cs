using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;
using commons;

namespace core
{
    public class AudioGroupBuildProcessor : ComponentBuildProcess
    {
        protected override void VerifyComponent(Component comp)
        {
            FindMissingAudioClips(comp as AudioGroup);
        }

        protected override void PreprocessComponent(Component comp)
        {
        }

        protected override void PreprocessOver(Component c)
        {
        }

        public override System.Type compType
        {
            get
            {
                return typeof(AudioGroup);
            }
        }

        private void FindMissingAudioClips(AudioGroup g)
        {
            string clipFolder = AssetDatabase.GUIDToAssetPath(g.assetDir.guid);
            var paths = EditorAssetUtil.ListAssetPaths(clipFolder, FileType.Audio, true);
            HashSet<string> local = GetHashSet(paths);

            foreach (AudioClipData a in g.data)
            {
                if (!local.Contains(a.path))
                {
                    AddErrorFormat("Missing clip '{0}', Check if filename is upper case.", a.path);
                } else
                {
                    local.Remove(a.path);
                }
            }
            if (local.IsNotEmpty())
            {
                AddErrorFormat("Missing clip data {0} in {1}", StringUtil.Join(",", local), g.csv.GetEditorPath());
            }
        }

        /// <summary>
        /// change filename to lowercase
        /// </summary>
        /// <returns>The hash set.</returns>
        /// <param name="paths">Paths.</param>
        private HashSet<string> GetHashSet(string[] paths)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string p in paths)
            {
                string dir = PathUtil.GetDirectory(p);
                string filename = Path.GetFileName(p).ToLower();
                set.Add(PathUtil.Combine(dir, filename));
            }
            return set;
        }
    }
}
