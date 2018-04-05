using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using comunity;

namespace etc
{
    public class AppVersion : MonoBehaviour
    {
        public Text version;
        
        void Start()
        {
            SetVersion();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        public void SetVersion()
        {
            if (version != null)
            {
                DateTime buildTime = new DateTime(BuildConfig.BUILD_TIME);
                string zone = BuildConfig.ZONE;
                string buildTimeStr = buildTime.ToString("MMddTHHmm");
                if (zone == BuildConfig.ZONE_LIVE)
                {
                    zone = string.Empty;
                } else if (zone == BuildConfig.ZONE_BETA)
                {
                    zone = "B";
                } else if (zone == BuildConfig.ZONE_ALPHA)
                {
                    zone = "A";
                } else
                {
                    zone = "D";
                }
                version.SetText(string.Format("{0}{1}  {2}", BuildConfig.VERSION, zone, buildTimeStr));
            }
        }
        
        private void Hide()
        {
            version.enabled = false;
        }
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex > 1)
            {
                version.DestroyEx();
                this.DestroyEx();
            }
        }
    }
}

