using System;
using System.Text.RegularExpressions;
using mulova.comunity;
using mulova.preprocess;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.build
{
    public class TextureAssetBuildProcess : AssetBuildProcess
    {
        private Regex reg = new Regex(" (?<w>[0-9]+)x(?<h>[0-9]+) ");
        public override string title => "Texture Asset";
        public override Type assetType => typeof(Texture);

        protected override void Verify(string path, Object obj)
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
                        log.Log(LogType.Error, $"[{path}] Change texture size {width}x{height} => {wpot}x{hpot}", null, obj);
                        TextureUtil.Resize(tex, wpot, hpot);
                    } else if (im.maxTextureSize < width||im.maxTextureSize < height)
                    {
                        TextureUtil.ScaleToImporterSizeRespectRatio(tex);
                    }
                }
            }
        }

        protected override void Preprocess(string path, UnityEngine.Object obj)
        {
        }

        protected override void Postprocess(string path, Object obj)
        {
        }
    }
}