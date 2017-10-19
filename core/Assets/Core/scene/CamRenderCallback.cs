using System;
using UnityEngine;
using core;

namespace core
{
    [RequireComponent(typeof(Camera))]
    public class CamRenderCallback : Script
    {
        public event Action postRender;
        
        void OnPostRender()
        {
            if (postRender != null)
            {
                postRender();
            }
        }
    }
}

