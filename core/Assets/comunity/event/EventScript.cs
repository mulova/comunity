//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System;

namespace core
{
    public abstract class EventScript: Script
    {
        protected abstract void InitEvents(EventRegistry reg);
        
        private EventRegistry reg;
        
        protected virtual void OnEnable()
        {
            if (reg == null)
            {
                reg = new EventRegistry();
                InitEvents(reg);
            }
            reg.RegisterEvents();
        }
        
        protected virtual void OnDisable()
        {
            if (reg != null)
            {
                reg.DeregisterEvents();
            }
        }
    }
}

