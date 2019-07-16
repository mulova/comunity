using System;
using UnityEngine;
using commons;

namespace comunity
{
    public class MaterialTexLoader : MonoBehaviour
    {
        public MaterialTexData[] materials;
        public const string TEX1 = "_MainTex";
        public const string TEX2 = "_AlphaTex";
        private Action callback;
        private int count;
        
        public void Begin(Action callback) {
            this.callback = callback;
            count = 0;
            foreach (MaterialTexData m in materials) {
                // set main texture
                Load(m.tex1, m.material, TEX1);
                Load(m.tex2, m.material, TEX2);
            }
        }
        
        private void Load(AssetRef texRef, Material mat, string texName) {
            if (texRef.isEmpty) {
                count++;
                if (count == materials.Length*2) {
                    callback.Call();
                }
            } else {
                Cdn.cache.GetTexture (texRef.path, tex => {
                    if (tex != null) {
                        mat.SetTexture (texName, tex);
                    }
                    count++;
                    if (count == materials.Length * 2) {
                        callback.Call ();
                    }
                });
            }
        }
    }

    [System.Serializable]
    public class MaterialTexData {
        public Material material;
        public AssetRef tex1 = new AssetRef();
        public AssetRef tex2 = new AssetRef();
    }
}
