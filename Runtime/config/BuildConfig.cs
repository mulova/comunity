using System.Ex;
using System.Text.Ex;
using mulova.unicore;
using UnityEngine;

namespace mulova.comunity
{
    /// <summary>
    /// Get Build setting from 'Resources/build_configuration'
    /// </summary>
    public static class BuildConfig
    {
        public static string BUILD_ID { get; private set; }
        public static string ZONE { get; set; }
        public static string RUNTIME { get; private set; } // Runtime player name
        public static string PLATFORM { get; private set; } // Platform is group of Targets. 'win' contains win32, win64
        public static string TARGET { get; private set; } // Target is the same as unity command line argument of -buildTarget
        public static string LANGUAGE { get; private set; }
        public static string VERSION { get; private set; }
        public static int VERSION_CODE { get; private set; }
        public static string REVISION { get; private set; }
        public static string DETAIL { get; private set; }
        public static string RES_VERSION { get; private set; }
        public static long BUILD_TIME { get; private set; }
        public static int STREAMING_SCENE_FROM { get; private set; }
        public static int STREAMING_ASSET_VERSION { get; private set; }
        public static string UNITY_VER { get; private set; }
        public static string CDN { get; private set; }
        public static bool TEST { get; private set; }

        public static SystemLanguage[] SYSTEM_LANGUAGES { get; private set; }
        
        public const string FILE_NAME = "build_configuration";
        
        public const  string ZONE_LIVE = "LIVE";
        public const  string ZONE_BETA = "BETA";
        public const  string ZONE_ALPHA = "ALPHA";
        public const  string ZONE_DEV = "DEV";
        
        public const string TARGET_ANDROID = "android";
        public const string TARGET_OSX = "osx";
        public const string TARGET_WIN = "win";
        public const string TARGET_IOS = "ios";
        public const string TARGET_LINUX = "linux";
        public const string TARGET_WEBGL = "webgl";
        
        public static Properties config
        {
            get
            {
                Properties config = new Properties();
                config.LoadResource(FILE_NAME);
                return config;
            }
        }
        
        static BuildConfig()
        {
            Reset();
        }
        
        public static void Reset()
        {
            var c = config;
            
            BUILD_ID = c.GetString(nameof(BUILD_ID), string.Empty);
            ZONE = c.GetString(nameof(ZONE), ZONE_LIVE);
            RUNTIME = c.GetString(nameof(RUNTIME), string.Empty);
            PLATFORM = c.GetString(nameof(PLATFORM), string.Empty);
            TARGET = c.GetString(nameof(TARGET), string.Empty);
            LANGUAGE = c.GetString(nameof(LANGUAGE), string.Empty);
            VERSION = c.GetString(nameof(VERSION), "1.0");
            VERSION_CODE = c.GetInt(nameof(VERSION_CODE), 1);
            REVISION = c.GetString(nameof(REVISION), string.Empty);
            DETAIL = c.GetString(nameof(DETAIL), string.Empty);
            STREAMING_SCENE_FROM = c.GetInt(nameof(STREAMING_SCENE_FROM), 0);
            STREAMING_ASSET_VERSION = c.GetInt(nameof(STREAMING_ASSET_VERSION), 0);
            RES_VERSION = c.GetString(nameof(RES_VERSION), "01");
            BUILD_TIME = c.GetLong(nameof(BUILD_TIME), 0);
            UNITY_VER = c.GetString(nameof(UNITY_VER), string.Empty);
            TEST = c.GetBool(nameof(TEST), false);

            string[] langs = LANGUAGE.SplitCSV();
            SYSTEM_LANGUAGES = langs.ConvertAll(l=>l.ParseEnum<SystemLanguage>(SystemLanguage.English));
        }

        public static string GetPlatformName(this RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return TARGET_ANDROID;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return TARGET_OSX;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return TARGET_WIN;
                case RuntimePlatform.IPhonePlayer:
                    return TARGET_IOS;
                case RuntimePlatform.LinuxPlayer:
                    return TARGET_LINUX;
                case RuntimePlatform.WebGLPlayer:
                    return TARGET_WEBGL;
                default:
                    return "none";
            }
        }
    }
}
