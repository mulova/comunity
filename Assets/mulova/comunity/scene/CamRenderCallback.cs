using System;
using UnityEngine;
using comunity;

namespace comunity
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

