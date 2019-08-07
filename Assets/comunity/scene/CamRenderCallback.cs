using System;
using UnityEngine;
using comunity;

namespace comunity
{
    [RequireComponent(typeof(Camera))]
    public class CamRenderCallback : InternalScript
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

