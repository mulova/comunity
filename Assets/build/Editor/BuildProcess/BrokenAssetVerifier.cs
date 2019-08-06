using UnityEngine;
using mulova.commons;
using System.Text.Ex;

namespace build
{
    public class BrokenAssetVerifier : AssetBuildProcess
    {
        
        public BrokenAssetVerifier() : base("Broken Asset", typeof(Object))
        {
        }
        
        protected override void VerifyAsset(string path, Object obj)
        {
            if (obj == null)
            {
                if (path.Is(FileType.Prefab) && obj == null)
                {
                    AddError(path);
                } else if (path.Is(FileType.Asset) && obj == null)
                {
                    AddError(path);
                }
            }
        }
        
        protected override void PreprocessAsset(string path, UnityEngine.Object obj)
        {
        }
    }
}
