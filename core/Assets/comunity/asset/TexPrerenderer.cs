using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Object = UnityEngine.Object;

namespace comunity
{
    /// <summary>
    /// TexListener is called after loaded texture is set to UITexture.
    /// 
    /// </summary>
    [RequireComponent(typeof(MeshTexLoader))]
    public class TexPrerenderer : MonoBehaviour
    {
        [SerializeField]
        public MeshTexLoader _canvas;

        public MeshTexLoader canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = GetComponent<MeshTexLoader>();
                }
                return _canvas;
            }
        }

        public List<AssetRef> assets;

        public event Action callback;

        private Queue<AssetRef> queue = new Queue<AssetRef>();

        public void Add(AssetRef asset)
        {
            queue.Enqueue(asset);
        }

        private void Load()
        {
            if (queue.Count > 0)
            {
                canvas.Load(queue.Dequeue(), null);
            } else
            {
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
            }
        }

        void Update()
        {
            if (canvas.IsIdle())
            {
                Load();
            }
        }
    }
}
