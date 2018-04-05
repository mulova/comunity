using UnityEngine;
using System;
using comunity;

namespace audio
{
    /// <summary>
    /// if the downloader is set, assets are downloaded and loaded automatically.
    /// else loaded by db at Start().
    /// </summary>
    public class AudioPlayer : Script, IComparable<AudioPlayer>
    {
        public AudioSource source;
        [Range(0, 1)] public float volume = 1;
        private AudioClipData current = NULL;
        private static readonly AudioClipData NULL = new AudioClipData();

        private Timer _loopTimer;
        private Timer loopTimer
        {
            get
            {
                if (_loopTimer == null)
                {
                    _loopTimer = new Timer(OnAudioPlayOver);
                }
                return _loopTimer;
            }
        }

        void Start()
        {
            if (source == null)
            {
                source = GetComponent<AudioSource>();
            }
        }

        public bool isPlaying
        {
            get 
            {
                return current != NULL;
            }
        }

        public bool isInterruptable
        {
            get
            {
                return current.interruptable;
            }
        }

        public bool IsPlaying(string key)
        {
            return current != NULL&&current.key == key;
        }

        public AudioClipData GetCurrent()
        {
            return current;
        }

        public float GetRemainingTime()
        {
            return loopTimer.RemainingTime;
        }

        void Update()
        {
            loopTimer.Update(Time.unscaledDeltaTime);
        }

        void OnAudioPlayOver()
        {
            if (current != NULL&&current.loop)
            {
                Play(current, source.clip, 0);
            } else
            {
                current = NULL;
            }
        }

        public void Stop()
        {
            if (current == NULL)
            {
                return;
            }
//            log.Info("Stop {0}", current.key);
            source.Stop();
            current = NULL;
        }

        /// <summary>
        /// Play the audio clip already loaded
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="clip">Clip.</param>
        /// <param name="delay">Delay.</param>
        public void Play(AudioClipData data, AudioClip clip, float delay)
        {
            if (data != NULL&&clip != null && source != null)
            {
                current = data;
                source.clip = clip;
                source.volume = volume;
                if (delay > 0)
                {
                    source.PlayDelayed(delay);
                } else
                {
                    source.Play();
                }
                loopTimer.Begin(delay+current.length);
                log.Debug("{0}: {1}", data, clip);
            } else
            {
                log.Warn("Not reachable {0}", data.key);
            }
        }

        public void SetMute(bool mute)
        {
            source.mute = mute;
        }

        #region IComparable implementation

        public int CompareTo(AudioPlayer that)
        {
            if (that == null)
            {
                return -1;
            }
            if (this.isPlaying^that.isPlaying)
            {
                return this.isPlaying? 1 : -1;
            }

            if (this.isInterruptable^that.isInterruptable)
            {
                return this.isInterruptable? -1 : 1;
            }
            float time = this.GetRemainingTime()-that.GetRemainingTime();
            if (time > 0)
            {
                return 1;
            } else if (time < 0)
            {
                return -1;
            } else
            {
                return 0;
            }
        }

        #endregion
    }
}

