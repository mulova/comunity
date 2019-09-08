using System;
using UnityEngine;
using mulova.comunity;

namespace mulova.comunity
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

