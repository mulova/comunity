using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace core
{
    public class TimerControl : MonoBehaviour
    {
        public TimerData[] timers = new TimerData[0];
        public bool startAutomatically;
        private List<Timer> timerList = new List<Timer>();
        private bool dispatching;
        
        public Timer this[int i]
        {
            get
            {
                if (timerList.Count <= i)
                {
                    ResizeTimer();
                }
                return timerList[i];
            }
        }

        public void Reset()
        {
            timers = new TimerData[0];
            ResizeTimer();
        }
        
        public void Add(TimerData data)
        {
            Array.Resize<TimerData>(ref timers, timers.Length+1);
            timers[timers.Length-1] = data;
            ResizeTimer();
        }

        public void Set(float duration, Action callback)
        {
            Reset();
            Add(new TimerData(duration, callback));
        }

        void Start()
        {
            if (startAutomatically)
            {
                Begin();
            }
        }
        
        public void Begin()
        {
            ResizeTimer();
            foreach (Timer t in timerList)
            {
                if (t.IsStopped())
                {
                    t.Begin();
                }
            }
        }
        
        public void Stop()
        {
            foreach (Timer t in timerList)
            {
                t.Stop();
            }
        }
        
        public void ResizeTimer()
        {
            if (timers.Length == timerList.Count)
            {
                return;
            }
            if (dispatching)
            {
                timerList = new List<Timer>(timerList);
            }
            if (timers.Length < timerList.Count)
            {
                timerList.RemoveRange(timers.Length, timerList.Count-timers.Length);
            } else
            {
                while (timerList.Count < timers.Length)
                {
                    timerList.Add(new Timer(timers[timerList.Count]));
                }
            }
        }
        
        void Update()
        {
            dispatching = true;
            ResizeTimer();
            foreach (Timer t in timerList)
            {
                t.Update(Time.deltaTime);
            }
            dispatching = false;
        }
        
        public void Pause()
        {
            foreach (Timer t in timerList)
            {
                t.Pause();
            }
        }
        
        public void Resume()
        {
            foreach (Timer t in timerList)
            {
                t.Resume();
            }
        }
        
        /**
         * Invoke callback once and destroy timer
         */
        public static TimerControl CreateTimer(GameObject gameObj, Action callback, float duration, bool repeat)
        {
            TimerControl timer = gameObj.AddComponent<TimerControl>();
            TimerData data = new TimerData();
            data.duration = duration;
            data.repeat = repeat;
            timer.Add(data);
            timer[0].AddCallback(callback);
            
            return timer;
        }
    }
}

