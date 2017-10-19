using UnityEngine;
using System;
using System.Collections;

namespace core
{
    public class IgnoreScaleTimer : MonoBehaviour
    {
        private TimerData data;

        public void SetTimerData(TimerData data)
        {
            this.data = data;
        }

        public void SetTimerData(float dur, Action callback, bool loop = false)
        {
            this.data = new TimerData(dur, callback);
            this.data.repeat = loop;
        }

        public void Begin()
        {
            StartCoroutine(BeginIgnoreScale());
        }

        public void Stop()
        {
            StopCoroutine(BeginIgnoreScale());
        }

        private IEnumerator BeginIgnoreScale()
        {
            if (data == null)
            {
                yield return null;
            }
            bool isPlaying = true;
            float _progressTime = 0F;
            float _timeAtLastFrame = 0F;
            float _timeAtCurrentFrame = 0F;
            float deltaTime = 0F;
            
            //Set time that timer started
            _timeAtLastFrame = Time.realtimeSinceStartup;
            while (isPlaying)
            {
                //Set time of present frame
                _timeAtCurrentFrame = Time.realtimeSinceStartup;
                deltaTime = _timeAtCurrentFrame-_timeAtLastFrame;
                _progressTime += deltaTime;
                
                if (_progressTime >= data.duration)
                {
                    if (data.repeat)
                    {
                        _progressTime = 0.0f;
                        data.callback.Call();
                    } else
                    {
                        isPlaying = false;
                    }
                }
                //Set previous frame time to present time
                _timeAtLastFrame = _timeAtCurrentFrame; 
                yield return new WaitForEndOfFrame();
            }
            yield return null;
            data.callback.Call();
        }
    }
}