using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace comunity
{
    public static class BuildInfo
    {
        public static readonly string FILE_NAME = "build_info";
        public static string TYPE { get; set; }
        public static bool STORE { get; set; }
        public static string BRANCH { get; private set; }
        public static string REV { get; private set; }
        public static long BUILD_TIMESTAMP { get; private set; }
        public static long COMMIT_TIMESTAMP { get; private set; }
        public static string PREFERRED_UNITY { get; private set; }

        // generated
        public static string VERSION { get; private set; }
        public static string UNITY_VERSION { get; private set; }
        public static bool BATCH { get; private set; }
        public static string PLATFORM { get; private set; }
        public static DateTime BUILD_UTC { get { return TimestampToDate(BUILD_TIMESTAMP); } }
        public static DateTime COMMIT_UTC { get { return TimestampToDate(COMMIT_TIMESTAMP); } }
        public static string BUILD_DATE { get { return TimestampToDateString(BUILD_TIMESTAMP); } }
        public static string COMMIT_DATE { get { return TimestampToDateString(COMMIT_TIMESTAMP); } }
        public static string SHORT_REV { get { return GetShortRev(REV); } }

        public static string PATH
        {
            get
            {
                return $"Assets/Resources/{FILE_NAME}.bytes";
            }

        }

        public static PropertiesReader prop
        {
            get
            {
                PropertiesReader p = new PropertiesReader();
                TextAsset properties = Resources.Load<TextAsset>(FILE_NAME);
                if (properties != null)
                {
                    p.Load(properties);
                }
                return p;
            }
        }

        static BuildInfo()
        {
            Load();
        }

    #if UNITY_EDITOR
        public static void Save()
        {
            PropertiesReader p = prop;
            p.Put(nameof(TYPE), TYPE);
            p.Put(nameof(STORE), STORE);
            p.Put(nameof(VERSION), VERSION);
            p.Put(nameof(UNITY_VERSION), UNITY_VERSION);
            p.Put(nameof(BATCH), BATCH);
            p.Put(nameof(PLATFORM), PLATFORM);
            p.Save(PATH);
            UnityEditor.AssetDatabase.ImportAsset(PATH);
        }
    #endif


        public static void Load()
        {
            Reload(prop);
        }

        public static void Reload(PropertiesReader p)
        {
            TYPE = p.GetString(nameof(TYPE), string.Empty);
    #if SERVICE_ALPHA
            STORE = p.GetBool(nameof(STORE), false);
    #else
            STORE = p.GetBool(nameof(STORE), true);
    #endif
            BRANCH = p.GetString(nameof(BRANCH), string.Empty);
            REV = p.GetString(nameof(REV), string.Empty);
            BUILD_TIMESTAMP = p.GetLong(nameof(BUILD_TIMESTAMP), 0);
            COMMIT_TIMESTAMP = p.GetLong(nameof(COMMIT_TIMESTAMP), 0);
            PREFERRED_UNITY = p.GetString(nameof(PREFERRED_UNITY), "2018.3.7f");

            VERSION = Application.version;
            UNITY_VERSION = Application.unityVersion;
            BATCH = Application.isBatchMode;
    #if UNITY_ANDROID
            PLATFORM = "Android";
    #else
            PLATFORM = "iOS";
    #endif
        }

        public static DateTime TimestampToDate(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) + new TimeSpan(timestamp * TimeSpan.TicksPerSecond);
        }

        public static string TimestampToDateString(long timestamp)
        {
            return TimestampToDate(timestamp).ToLocalTime().ToString(CultureInfo.InvariantCulture);
        }

        public new static string ToString()
        {
            StringBuilder str = new StringBuilder(1024);
            foreach (var pair in prop)
            {
                AppendKeyValuePair(str, pair.Key, pair.Value);
            }
            AppendKeyValuePair(str, nameof(BUILD_DATE), BUILD_DATE);
            AppendKeyValuePair(str, nameof(COMMIT_DATE), COMMIT_DATE);
            return str.ToString();
        }

        public static void AppendKeyValuePair(StringBuilder str, string key, object val)
        {
            str.Append(key).Append("=");
            if (val != null)
            {
                str.Append(val.ToString());
            }
            str.AppendLine();
        }

        public static string GetShortRev(string rev)
        {
            if (!string.IsNullOrEmpty(rev) && rev.Length >= 7)
            {
                return rev.Substring(0, 7);
            }
            else
            {
                return rev;
            }
        }
    }
}
