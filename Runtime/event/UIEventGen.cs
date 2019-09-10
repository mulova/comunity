//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace mulova.comunity
{
    /// <summary>
    /// [Event Out] send event when the specified ui event is set.
    /// </summary>
    public class UIEventGen : MonoBehaviour
    {
        [EventList] public string onClick;
        [EventList] public string onDoubleClick;
        [EventList] public string onPress;
        [EventList] public string onRelease;
        [EventList] public string onSelect;
        [EventList] public string onDeselect;
        [EventList] public string onHoverOver;
        [EventList] public string onHoverOut;
        [EventList] public string onDragOver;
        [EventList] public string onDragOut;
        
        void OnHover (bool isOver)
        {
            if (isOver) {
                EventRegistry.SendEvent(onHoverOver, this);
            } else {
                EventRegistry.SendEvent(onHoverOut, this);
            }
        }
        
        void OnPress (bool pressed)
        {
            if (pressed) {
                EventRegistry.SendEvent(onPress, this);
            } else {
                EventRegistry.SendEvent(onRelease, this);
            }
        }
        
        void OnSelect (bool selected)
        {
            if (selected) {
                EventRegistry.SendEvent(onSelect, this);
            } else {
                EventRegistry.SendEvent(onDeselect, this);
            }
        }
        
        void OnClick ()
        {
            EventRegistry.SendEvent(onClick, this);
        }
        
        void OnDoubleClick ()
        {
            EventRegistry.SendEvent(onDoubleClick, this);
        }
        
        void OnDragOver (GameObject go)
        {
            EventRegistry.SendEvent(onDragOver, this);
        }
        
        void OnDragOut (GameObject go)
        {
            EventRegistry.SendEvent(onDragOut, this);
        }
    }
}

