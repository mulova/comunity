using UnityEngine;
using System;

public static class AppConfig
{
#if SERVICE_ALPHA || SERVICE_BETA
    private static string _apiVer;
    public static string API_VER {
        get
        {
            return GamePrefs.ApiVer.GetString(_apiVer);
        }
        set
        {
            _apiVer = value;
        }
    }
#else
    public static string API_VER { get; private set; }
#endif
    public static string API_SERVER { get; private set; } = null;
    public static string SESSION_ADDRESS { get; private set; } = null;
    public static string CDN { get; private set; } = null;

    public static string configPostfix
    {
        get
        {
            string type = BuildInfo.TYPE;
            if ((BuildMode.isEditor && Application.isPlaying) || string.IsNullOrEmpty(type))
            {
#if SERVICE_ALPHA
                type = "alpha";
#elif SERVICE_BETA
                type = "beta";
#elif SERVICE_RC
                type = "rc";
#else
                type = "store";
#endif
            }
            return GetConfigId(type);
        }
    }

    public static string GetConfigId(string buildType)
    {
        if (buildType.IndexOf("alpha", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "alpha";
        }
        else if (buildType.IndexOf("beta", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "beta";
        }
        else if (buildType.IndexOf("adhoc", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "rc_adhoc";
        }
        else if (buildType.IndexOf("rc", StringComparison.OrdinalIgnoreCase) >= 0
                 || buildType.IndexOf("staging", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "rc";
        }
        else if (buildType.IndexOf("store_enterprise", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "store_enterprise";
        }
        else
        {
            return "store";
        }
    }

    public static string GetPhase(string buildType)
    {
        if (buildType.IndexOf("alpha", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "alpha";
        }
        else if (buildType.IndexOf("beta", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "beta";
        }
        else if (buildType.IndexOf("adhoc", StringComparison.OrdinalIgnoreCase) >= 0
                 || buildType.IndexOf("rc", StringComparison.OrdinalIgnoreCase) >= 0
                 || buildType.IndexOf("staging", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return "rc";
        }
        else
        {
            return "store";
        }
    }

    public static string GetFilePath(string phase)
    {
        return $"config/app_config_{GetConfigId(phase)}";
    }

    static AppConfig()
    {
        Load();
    }

    public static PropertiesReader GetReader(string path)
    {
        PropertiesReader p = new PropertiesReader();
        TextAsset properties = Resources.Load<TextAsset>(GetFilePath(path));
        if (properties != null)
        {
            p.Load(properties);
        }
        return p;
    }

    public static PropertiesReader Load()
    {
        var p = GetReader($"config/app_config_{configPostfix}");
        API_VER = p.GetString(nameof(API_VER), string.Empty);
        API_SERVER = p.GetString(nameof(API_SERVER), null);

#if SERVICE_ALPHA || SERVICE_BETA
        CDN = p.GetString(nameof(CDN), null);
        SESSION_ADDRESS = p.GetString(nameof(SESSION_ADDRESS), null);
#endif
        if (string.IsNullOrEmpty(CDN))
        {
            CDN = null;
        }
        if (string.IsNullOrEmpty(SESSION_ADDRESS))
        {
            SESSION_ADDRESS = null;
        }
        return p;
    }

    public new static string ToString()
    {
        return Load().ToString();
    }
}
