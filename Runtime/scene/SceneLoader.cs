//#define ASYNC_SCENE_LOADING
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using UnityEngine.SceneManagement;
using mulova.comunity;
using mulova.commons;
using System.Ex;

namespace mulova.comunity
{
    public class SceneLoader : SingletonBehaviour<SceneLoader>
    {
        public SceneTransit transit;

        public event Action<string> preCallbacks;
        public event Action<string> preOneshotCallbacks;
        public event Action<string> postCallbacks;
        public event Action<string> postOneshotCallbacks;

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoad;
            if (transit == null)
            {
                transit = go.AddComponent<DummySceneTransit>();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneLoaded -= OnSceneLoad;
        }

        public void ClearCallbacks()
        {
            preCallbacks = null;
            preOneshotCallbacks = null;
            postCallbacks = null;
            postOneshotCallbacks = null;
        }

        public void Load(object scene, object transitData)
        {
            if (transit.inProgress)
            {
                log.Warn("Loading Scene '{0}' is interrupted. ", scene);
                return;
            }
            string sceneName = scene.ToText();
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                InputBlocker.Show(this);
                transit.StartTransit(transitData);
                BeginSceneLoading(sceneName);
            } else
            {
                log.Info("Sceen loading is ignored. The same scene '{0}'is requested", sceneName);
            }
        }

#if ASYNC_SCENE_LOADING
        private static AsyncOperation loadLevelAsync;

        private IEnumerator LoadAsync(string scene, AssetBundle asset)
        {
            next = null;
            loadLevelAsync = SceneManager.LoadSceneAsync(scene);
            yield return loadLevelAsync;
            loadLevelAsync = null;
            SceneStack.Push(scene);
            InputBlocker.Hide(this);
        }
#endif

        private void LoadSync(string scene)
        {
            try
            {
                SceneManager.LoadScene(scene);
                SceneStack.Push(scene);
            } catch (Exception ex)
            {
                log.Error(ex);
                SceneManager.LoadScene(0);
            }
            InputBlocker.Hide(this);
        }
        
        private void BeginSceneLoading(string sceneName)
        {
            log.Info("Loading Scene... {0}", sceneName);
            preCallbacks.Call(sceneName);
            preOneshotCallbacks.Call(sceneName);
            preOneshotCallbacks = null;

            if (Platform.isEditor)
            {
                LoadSync(sceneName);
            } else
            {                
                if (BuildConfig.STREAMING_SCENE_FROM > 0 && !SceneStack.HasVisited(sceneName))
                {
                    Cdn.cache.GetAssetBundle(sceneName+FileTypeEx.ASSET_BUNDLE, false, asset => {
#if ASYNC_SCENE_LOADING
                        if (asset != null)
                        {
                            StartCoroutine(LoadAsync(sceneName, asset));
                        } else
                        {
                            LoadSync(sceneName);
                        }
#else
                        LoadSync(sceneName);
#endif
                    });
                } else
                {
                    LoadSync(sceneName);
                }
            }
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            transit.EndTransit();
            postCallbacks.Call(scene.name);
            postOneshotCallbacks.Call(scene.name);
            postOneshotCallbacks = null;
        }
    }
}


