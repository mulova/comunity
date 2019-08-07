using UnityEngine;
using System;
using System.Collections.Generic;
using Nullable = mulova.commons.Nullable;
using Object = UnityEngine.Object;
using mulova.commons;
using comunity;
using UnityEngine.Ex;
using System.Ex;
using System.Collections.Generic.Ex;

namespace audio
{
    /// <summary>
    /// if the downloader is set, assets are downloaded and loaded automatically.
    /// else loaded by db at Start().
    /// </summary>
    public class AudioGroup : InternalScript
    {
        public AssetGuid assetDir;
        private static Dictionary<string, AudioGroup> pool = new Dictionary<string, AudioGroup>();

        public static AudioGroup Get(string key)
        {
            return pool.Get(key);
        }

        private AudioDataTable _table;

        public AudioDataTable table
        {
            get
            {
                if (_table == null&&!csv.isEmpty)
                {
                    _table = new AudioDataTable(csv.path);
                }
                return _table;
            }
        }

        private Stack<object> stack;
        public AudioPlayer playerPrefab;
        public int playerCount = 1;
        public bool streaming = true;
        public bool useStack;
        public AssetRef csv;

        public IEnumerable<string> clips
        {
            get
            { 
                if (table != null)
                {
                    return table.keys;
                }
                {
                    return null;
                }
            }
        }

        public IEnumerable<AudioClipData> data
        {
            get
            { 
                if (table != null)
                {
                    return table.rows;
                } else
                {
                    return null;
                }
            }
        }

        public bool isPlaying
        {
            get
            {
                foreach (AudioPlayer p in players)
                {
                    if (p.isPlaying)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private AudioPlayer[] _players;
        public AudioPlayer[] players
        {
            get
            {
                if (_players == null)
                {
                    _players = new AudioPlayer[playerCount];
                    for (int i=0; i<playerCount; ++i)
                    {
                        _players[i] = playerPrefab.InstantiateEx();
                        _players[i].gameObject.SetActive(true);
                    }
                }
                return _players;
            }
        }

        public int GetPlayingClipCount(string key)
        {
            int count = 0;
            foreach (AudioPlayer p in players)
            {
                if (p.IsPlaying(key))
                {
                    count++;
                }
            }
            return count;
        }

        void OnEnable()
        {
            pool[name] = this;
            AddAudioListener(OnVoidEvent, OnFloatEvent, OnBoolEvent);
        }

        void OnDisable()
        {
            pool.Remove(name);
            RemoveAudioListener(OnVoidEvent, OnFloatEvent, OnBoolEvent);
        }

        public AudioClipData GetData(string key)
        {
            if (table == null)
            {
                return null;
            }
            return table.GetRow(key);
        }

        public void SetMute(bool mute)
        {
            foreach (AudioPlayer p in players)
            {
                p.SetMute(mute);
            }
        }

        public void Stop()
        {
            foreach (AudioPlayer p in players)
            {
                p.Stop();
            }
        }

        public void Stop(string key)
        {
            foreach (AudioPlayer p in players)
            {
                if (p.IsPlaying(key))
                {
                    p.Stop();
                }
            }
        }

        public void Play(string id, float volume = 1f)
        {
            PlayDelayed(id, volume, 0);
        }

        public void PlayDelayed(string id, float volume, float delay)
        {
            AudioClipData d = GetData(id);
            if (d == null)
            {
                return;
            }
            AudioPlayer p = GetAvailablePlayer();
            if (!p.isPlaying||p.isInterruptable)
            {
                p.volume = volume;
                comunity.AudioClipLoadType loadType = streaming? comunity.AudioClipLoadType.Streaming: comunity.AudioClipLoadType.Uncompressed;
                Cdn.cache.GetAudio(d.path, loadType, clip =>
                {
                    if (clip != null)
                    {
                        p.Play(d, clip, delay);
                    }
                }); 
            } else
            {
                log.Warn("All audio players({0}) are busy. Can't play {1}", players.Length, id);
            }

        }

        private AudioPlayer GetAvailablePlayer()
        {
            Array.Sort(players);
            return players[0];
        }

        private void OnVoidEvent(object evt)
        {
            Play(evt.ToText());
        }

        private void OnFloatEvent(object evt, float val)
        {
            PlayDelayed(evt.ToText(), 1, val);
        }

        private void OnBoolEvent(object evt, bool val)
        {
            if (useStack)
            {
                if (stack == null)
                {
                    stack = new Stack<object>();
                }
                if (val)
                {
                    stack.Push(evt);
                    OnVoidEvent(evt);
                } else
                {
                    if (stack.NotEmpty())
                    {
                        stack.Pop();
                        if (stack.NotEmpty())
                        {
                            OnVoidEvent(stack.Peek());
                        } else
                        {
                            Stop(evt.ToText());
                        }
                    } else
                    {
                        Stop();
                    }
                }
            } else
            {
                if (val)
                {
                    OnVoidEvent(evt);
                } else
                {
                    Stop();
                }
            }
        }

        #region Event System

        public static readonly string VOID_EVENT_ID = "_AudioGroup_Void";
        public static readonly string FLOAT_EVENT_ID = "_AudioGroup_Float";
        public static readonly string BOOL_EVENT_ID = "_AudioGroup_Bool";

        private static void AddAudioListener(Action<object> callback, Action<object, float> fCallback, Action<object, bool> bCallback)
        {
            Messenger<object>.AddListener(VOID_EVENT_ID, callback);
            Messenger<object, float>.AddListener(FLOAT_EVENT_ID, fCallback);
            Messenger<object, bool>.AddListener(BOOL_EVENT_ID, bCallback);
        }

        private static void RemoveAudioListener(Action<object> callback, Action<object, float> fCallback, Action<object, bool> bCallback)
        {
            Messenger<object>.RemoveListener(VOID_EVENT_ID, callback);
            Messenger<object, float>.RemoveListener(FLOAT_EVENT_ID, fCallback);
            Messenger<object, bool>.RemoveListener(BOOL_EVENT_ID, bCallback);
        }

        public static void Broadcast(object evt)
        {
            Messenger<object>.Broadcast(VOID_EVENT_ID, evt);
        }

        public static void Broadcast(object evt, float val)
        {
            Messenger<object, float>.Broadcast(FLOAT_EVENT_ID, evt, val);
        }

        public static void Broadcast(object evt, bool val)
        {
            Messenger<object, bool>.Broadcast(BOOL_EVENT_ID, evt, val);
        }

        public static void Reload()
        {
            foreach (AudioGroup g in Object.FindObjectsOfType<AudioGroup>())
            {
                g._table = null;
            }
        }

        #endregion
    }
}

