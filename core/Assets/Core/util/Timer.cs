//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System;
using commons;

namespace core
{
    /// <summary>
    /// 지정된 시간후에 ShowType event를 수행한다.
    /// </summary>
    public class Timer
    {
        public TimerData data;

        public float Duration
        {
            get { return data.duration; }
            set { data.duration = value; }
        }

        public bool Repeat
        {
            get { return data.repeat; }
            set { data.repeat = value; }
        }

        public bool Enabled
        {
            get { return data.enabled; }
            set { data.enabled = value; }
        }

        private float remain = -1;

        public float RemainingTime
        {
            get { return remain; }
        }

        private EventDispatcher callback = new EventDispatcher(false);
        private EventDispatcher oneShotCallback = new EventDispatcher(true);

        public Timer()
        {
            this.data = new TimerData();
        }

        public Timer(TimerData data)
        {
            this.data = data;
        }

        public Timer(Action callback) : this()
        {
            AddCallback(callback);
        }

        public void Pause()
        {
            this.Enabled = false;
        }

        public void Resume()
        {
            this.Enabled = true;
        }

        public void SetCallback(Action callback)
        {
            this.callback.SetCallback(callback);
        }

        public void AddCallback(Action callback)
        {
            this.callback.AddCallback(callback);
        }

        public void RemoveCallback(Action handler)
        {
            this.callback.RemoveCallback(handler);
        }

        public void ClearCallBack()
        {
            this.callback.Clear();
        }

        public void Begin()
        {
            remain = Duration;
            Resume();
        }

        public void Begin(float time)
        {
            this.Duration = time;
            Begin();
        }

        public void Begin(float time, Action oneShotCallback)
        {
            this.Duration = time;
            this.oneShotCallback.AddCallback(oneShotCallback);
            Begin();
        }

        // No callback
        public void Stop()
        {
            remain = -1;
        }

        public void Fastforward()
        {
            if (remain > 0)
            {
                remain = 0;
            }
        }

        public bool IsStopped()
        {
            return remain < 0;
        }

        public void Update(float time)
        {
            if (remain < 0||!Enabled)
            {
                return;
            }
            remain -= time;
            if (remain < 0)
            {
                this.callback.Broadcast();
                this.oneShotCallback.Broadcast();
                if (data.method != null)
                {
                    data.method.InvokeMethod();
                }
                if (data.callback != null)
                {
                    data.callback();
                }
                if (Repeat)
                {
                    Begin();
                }
            }
        }

    }

}

