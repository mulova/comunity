using System;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;
using System.Text.RegularExpressions;
using comunity;

namespace build
{
    public class TextureAssetBuildProcess : AssetBuildProcess
    {
        private Regex reg = new Regex(" (?<w>[0-9]+)x(?<h>[0-9]+) ");

        public TextureAssetBuildProcess() : base("Texture Asset", typeof(Texture))
        {
        }

        protected override void VerifyAsset(string path, Object obj)
        {
            // resize
            if (!AssetConfig.TEX_NPOT)
            {
                if (AssetBundlePath.inst.IsRawCdnPath(path))
                {
                    Texture tex = obj as Texture;
                    var output = TextureUtil.Exec("identify", path);
                    var match = reg.Match(output.stdout);
                    int width = int.Parse(match.Groups["w"].Value);
                    int height = int.Parse(match.Groups["h"].Value);
                    TextureImporter im = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (!Mathf.IsPowerOfTwo(width)||!Mathf.IsPowerOfTwo(height))
                    {
                        int wpot = Mathf.ClosestPowerOfTwo(width);
                        int hpot = Mathf.ClosestPowerOfTwo(height);
                        log.Warn("[{0}] Change texture size {1}x{2} => {3}x{4}", path, width, height, wpot, hpot);
                        TextureUtil.Resize(tex, wpot, hpot);
                    } else if (im.maxTextureSize < width||im.maxTextureSize < height)
                    {
                        TextureUtil.ScaleToImporterSizeRespectRatio(tex);
                    }
                }
            }
        }

        protected override void PreprocessAsset(string path, UnityEngine.Object obj)
        {
        }
    }
}