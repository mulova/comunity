using UnityEditor;

namespace build
{
    public class AssetBuilder
    {
        public static void ClearAllAssetBundleNames()
        {
            foreach (var n in AssetDatabase.GetAllAssetBundleNames())
            {
                AssetDatabase.RemoveAssetBundleName(n, true);
            }
            AssetBundleDep.inst.Clear();
        }
    }
}
