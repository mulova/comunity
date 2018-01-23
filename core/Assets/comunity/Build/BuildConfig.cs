using System;
using System.Security.Cryptography;
using UnityEngine;

namespace core
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
        public static string DIGEST { get; private set; }
        public static string UNITY_VER { get; private set; }
        public static bool TEST { get; private set; }

        public static SystemLanguage[] SYSTEM_LANGUAGES { get; private set; }
        
        public const string KEY_BUILD_ID = "BUILD_ID";
        public const string KEY_LANGUAGE = "LANG";
        public const string KEY_VERSION = "VERSION";
        public const string KEY_VERSION_CODE = "VERSION_CODE";
        public const string KEY_RUNTIME = "RUNTIME";
        public const string KEY_PLATFORM = "PLATFORM";
        public const string KEY_RES_VER = "RES_VER";
        public const string KEY_BUILD_TIME = "BUILD_TIME";
        public const string KEY_REVISION = "REV";
        public const string KEY_STREAMING_ASSET = "STREAMING_ASSET";
        public const string KEY_STREAMING_SCENE_FROM = "STREAMING_SCENE_FROM";
        public const string KEY_DIGEST = "DIGEST";
        public const string KEY_UNITY_VER = "UNITY_VER";
        public const string KEY_TARGET = "TARGET";
        public const string KEY_ZONE = "ZONE";
        public const string KEY_TEST = "TEST";
        public const string KEY_DETAIL = "DETAIL";
        public const string KEY_CDN = "CDN";
        
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
        
        public static PropertiesReader config
        {
            get
            {
                PropertiesReader config = new PropertiesReader();
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
            
            BUILD_ID = c.GetString(KEY_BUILD_ID, string.Empty);
            ZONE = c.GetString(KEY_ZONE, ZONE_LIVE);
            RUNTIME = c.GetString(KEY_RUNTIME, string.Empty);
            PLATFORM = c.GetString(KEY_PLATFORM, string.Empty);
            TARGET = c.GetString(KEY_TARGET, string.Empty);
            LANGUAGE = c.GetString(KEY_LANGUAGE, string.Empty);
            VERSION = c.GetString(KEY_VERSION, "1.0");
            VERSION_CODE = c.GetInt(KEY_VERSION_CODE, 1);
            REVISION = c.GetString(KEY_REVISION, string.Empty);
            DETAIL = c.GetString(KEY_DETAIL, string.Empty);
            STREAMING_SCENE_FROM = c.GetInt(KEY_STREAMING_SCENE_FROM, 0);
            STREAMING_ASSET_VERSION = c.GetInt(KEY_STREAMING_ASSET, 0);
            RES_VERSION = c.GetString(KEY_RES_VER, "01");
            BUILD_TIME = c.GetLong(KEY_BUILD_TIME, 0);
            DIGEST = c.GetString(KEY_DIGEST, string.Empty);
            UNITY_VER = c.GetString(KEY_UNITY_VER, string.Empty);
            TEST = c.GetBool(KEY_TEST, false);

            string[] langs = BuildConfig.LANGUAGE.SplitCSV();
            SYSTEM_LANGUAGES = langs.Convert(l=>l.ParseEnum<SystemLanguage>(SystemLanguage.English));
        }
        
        public static string GenerateDigest(byte[] src)
        {
            if (src != null)
            {
                using (SHA256 shaM = new SHA256Managed())
                {
                    byte[] bytes = src;
                    bytes = shaM.ComputeHash(bytes);
                    return BitConverter.ToString(bytes);
                }
            }
            return string.Empty;
        }
    }
}
