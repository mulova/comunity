using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using mulova.comunity;
using mulova.commons;

namespace mulova.comunity
{
    public class InputBlocker : LogBehaviour
    {
        public GameObject ui;
        private HashSet<object> keys = new HashSet<object>();
        public const string BLOCK_UI = "block.ui";
        
        void OnEnable()
        {
            EventRegistry.RegisterListener(BLOCK_UI, OnBlock);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        void OnDisable()
        {
            EventRegistry.DeregisterListener(BLOCK_UI, OnBlock);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void SetEnabled(bool visible)
        {
            GetComponent<Collider2D>().enabled = visible;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Hide();
        }
        
        private void OnBlock(object data)
        {
            BlockInfo b = (BlockInfo)data;
            if (b.key == null)
            {
                Hide();
            } else if (b.visible)
            {
                ShowBlocker(b.key);
            } else
            {
                HideBlocker(b.key);
            }
        }
        
        private void ShowBlocker(object key)
        {
            log.Debug("Input Block +{0}", key);
            keys.Add(key);
            ui.SetActive(true);
        }
        
        private void HideBlocker(object key)
        {
            log.Debug("Input Block -{0}", key);
            keys.Remove(key);
            if (keys.IsEmpty())
            {
                ui.SetActive(false);
                log.Info("InputBlocker Off");
            }
        }
        
        private void Hide()
        {
            keys.Clear();
            ui.SetActive(false);
        }
        
        public static void Show(object k)
        {
            EventRegistry.SendEvent(BLOCK_UI, new BlockInfo() { key=k,visible=true});
        }
        
        public static void Hide(object k)
        {
            EventRegistry.SendEvent(BLOCK_UI, new BlockInfo() { key=k,visible=false});
        }
        
        public static void Clear()
        {
            EventRegistry.SendEvent(BLOCK_UI, new BlockInfo() { key=null,visible=false});
        }
        
        public static void SetVisible(object key, bool visible)
        {
            if (visible)
            {
                Show(key);
            } else
            {
                Hide(key);
            }
        }
        
        struct BlockInfo
        {
            public object key;
            public bool visible;
        }
    }
}