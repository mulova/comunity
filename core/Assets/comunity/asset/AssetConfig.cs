using System;
using System.Security.Cryptography;
using UnityEngine;

namespace comunity
{
    /// <summary>
    /// Get Build setting from 'asset_config'
    /// </summary>
    public static class AssetConfig
    {
        public const string FILE_NAME = "asset_config.txt";
        public static bool TEX_NPOT { get; private set; }

        public const string KEY_TEX_NPOT = "TEX_NPOT";

        public static PropertiesReader config
        {
            get
            {
                PropertiesReader config = new PropertiesReader();
                config.LoadFile(FILE_NAME);
                return config;
            }
        }

        static AssetConfig()
        {
            Reset();
        }

        public static void Reset()
        {
            var c = config;
            TEX_NPOT = c.GetBool(KEY_TEX_NPOT, false);
        }
    }
}
