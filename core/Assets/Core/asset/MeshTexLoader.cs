using System;

using UnityEngine;
using System.Collections.Generic;


namespace core
{
    [RequireComponent(typeof(Renderer))]
    public class MeshTexLoader : Script, IReleasable
    {
        public Renderer rend;
        public bool shared;
        public string exclusiveId;
        [HideInInspector]
        public bool recoverOnEnable;
        // temporaily clear texture while on disable
        [HideInInspector]
        public bool removeOnDisable;
        private string curUrl;
        private string newUrl;
        private Action<Texture> loadedCallback;
        private TexLoaderStatus status = TexLoaderStatus.Idle;
        private Texture propertyTex;
        public Action<MaterialPropertyBlock> setMaterialProperty;
        [HideInInspector]
        public string editorTexPath;
        private bool cdn;

        private MaterialPropertyBlock _propertyBlock;
        private MaterialPropertyBlock propertyBlock
        {
            get
            {
                if (_propertyBlock == null)
                {
                    _propertyBlock = new MaterialPropertyBlock();
                }
                return _propertyBlock;
            }
        }


        private int _texId;
        internal int mainTexId
        {
            get
            {
                if (_texId == 0)
                {
                    _texId = Shader.PropertyToID("_MainTex");
                }
                return _texId;
            }
        }

        internal Texture tex
        {
            get
            {
                if (shared)
                {
                    return propertyTex;
                } else
                {
                    if (Application.isPlaying)
                    {
                        return rend.material.mainTexture;
                    } else
                    {
                        return rend.sharedMaterial.mainTexture;
                    }
                }
            }
            set
            {
                if (shared)
                {
                    propertyTex = value;
                    if (propertyTex != null)
                    {
                        UpdatePropertyBlock();
                    }
                } else
                {
                    rend.material.mainTexture = value;
                }
            }
        }

        public void Load(AssetRef asset, Action<Texture> callback)
        {
            if (rend != null&&!asset.isEmpty)
            {
                if (asset.cdn)
                {
                    Load(asset.path, callback);
                } else if (asset.GetReference() != null)
                {
                    this.loadedCallback = callback;
                    SetTexture(asset.GetReference() as Texture);
                }
            }
        }

        public void Load(string url, Action<Texture> callback = null)
        {
            Load(url, true, callback);
        }

        public void Load(string url, bool cdn, Action<Texture> callback = null)
        {
            this.loadedCallback = callback;
            this.cdn = cdn;
            if (url == curUrl&&status == TexLoaderStatus.Idle)
            { // same as now
                Finish();
            } else if (url == newUrl&&status == TexLoaderStatus.Download)
            { // same as previous request
                return;
            }

            if (status == TexLoaderStatus.Download)
            {
                Interrupt();
            }
            if (url.IsEmpty())
            {
                Clear();
            } else
            {
                this.newUrl = url;
                status = TexLoaderStatus.Download;
                LoadAsync();
            }
        }

        private void Interrupt()
        {
            log.Info("Download for {0} INTERRUPTED", newUrl);
            // remove pending if exists
            if (newUrl.IsNotEmpty())
            {
                loadedCallback = null;
            }
            Finish();
        }

        void Update()
        {
            if (status != TexLoaderStatus.Download&&curUrl != newUrl)
            {
                LoadAsync();
            }
            if (shared && propertyTex != null)
            {
                UpdatePropertyBlock();
            }
        }

        private void UpdatePropertyBlock()
        {
            propertyBlock.Clear();
            rend.GetPropertyBlock(propertyBlock);
            propertyBlock.SetTexture(mainTexId, propertyTex);
            if (setMaterialProperty != null)
            {
                setMaterialProperty(propertyBlock);
            }
            rend.SetPropertyBlock(propertyBlock);
        }


        internal void LoadAsync()
        {
            log.Debug("<{0}> loads texture {1}", name, newUrl);
            string url = newUrl;
            AssetCache cache = cdn? Cdn.cache : Web.cache;
            cache.GetTexture(url, tex =>
            {
                if (url == newUrl)
                { // check if another request is triggered while downloading is in progress.
                    curUrl = newUrl;
                    if (tex != null)
                    {
                        SetTexture(tex);
                    } else
                    {
                        log.Info("Can't access {0}", url);
                        Clear();
                    }
                }
                Finish();
            }, exclusiveId);
        }

        public void Clear()
        {
            if (status == TexLoaderStatus.Download)
            {
                Interrupt();
            }
            curUrl = null;
            newUrl = null;
            RemoveTexture();
        }

        private void RemoveTexture()
        {
            tex = null;
        }

        void OnDisable()
        {
            if (recoverOnEnable)
            {
                // reload texture when enabled again
                if (curUrl.IsNotEmpty())
                {
                    newUrl = curUrl;
                    status = TexLoaderStatus.Idle;
                }
                curUrl = null;
                RemoveTexture();
            } else if (removeOnDisable)
            {
                Clear();
            }
        }

        public bool IsIdle()
        {
            return status == TexLoaderStatus.Idle;
        }

        private void Finish()
        {
            status = TexLoaderStatus.Idle;
            if (loadedCallback != null)
            {
                Action<Texture> a = loadedCallback;
                loadedCallback = null;
                a.Call(tex);
            }
        }

        private void SetTexture(Texture t)
        {
            if (t != null)
            {
                log.Info("<{0}> set texture {1}", name, t.name);
                tex = t;
            } else
            {
                Clear();
            }
            Finish();
        }

        public void Release()
        {
            Clear();
        }

        public float GetFloat(int id)
        {
            if (shared)
            {
                return propertyBlock.GetFloat(id);
            } else
            {
                return rend.material.GetFloat(id);
            }
        }

        public Texture GetTexture(int id)
        {
            if (shared)
            {
                return propertyBlock.GetTexture(id);
            } else
            {
                return rend.material.GetTexture(id);
            }
        }
        
        public Vector4 GetInt(int id)
        {
            if (shared)
            {
                return propertyBlock.GetVector(id);
            } else
            {
                return rend.material.GetVector(id);
            }
        }
        
        public Matrix4x4 GetMatrix(int id)
        {
            if (shared)
            {
                return propertyBlock.GetMatrix(id);
            } else
            {
                return rend.material.GetMatrix(id);
            }
        }
    }

    enum TexLoaderStatus
    {
        Idle,
        Download,
    }
}
