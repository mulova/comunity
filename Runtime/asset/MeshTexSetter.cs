using UnityEngine;
using System.Collections.Generic;
using mulova.commons;
using UnityEngine.Ex;
using System.Collections.Generic.Ex;

namespace mulova.comunity
{
    [RequireComponent(typeof(MeshTexLoader))]
    public class MeshTexSetter : InternalScript
    {
        public List<AssetRef> textures = new List<AssetRef>();
        private MeshTexLoader _texLoader;

        public MeshTexLoader texLoader
        {
            get
            {
                if (_texLoader == null)
                {
                    _texLoader = GetComponent<MeshTexLoader>();
                }
                return _texLoader;
            }
        }

        void OnEnable()
        {
            // invalid 
            if (texLoader == null)
            {
                log.Warn("No MaterialTexLoader in {0}", transform.GetScenePath());
                return;
            }
            if (texLoader.rend == null)
            {
                log.Warn("No UITexture in {0}", transform.GetScenePath());
                return;
            }
            if (textures.GetCount() == 1)
            {
                texLoader.Load(textures[0], null);
            }
        }

        void OnDisable()
        {
            //          texLoader.Clear();
        }

        public void SetTexture(int i)
        {
            if (textures.IsEmpty())
            {
                return;
            }
            if (Platform.isPlaying)
            {
                texLoader.Load(textures[i], null);
            } else
            {
                #if UNITY_EDITOR
                i = i.Clamp(0, textures.Count);
                if (texLoader.rend != null)
                {
                    AssetRef r = textures[i];
                    r.LoadAsset<Texture>(t =>
                    {
                        texLoader.tex = t;
                        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                    });
                }
                #else
                texLoader.Load(textures[i], null);
                #endif
            }
        }

        public bool IsEmpty()
        {
            bool empty = true;
            if (textures.IsNotEmpty())
            {
                foreach (AssetRef r in textures)
                {
                    empty &= r.isEmpty;
                }
            }
            return empty;
        }
    }
}
