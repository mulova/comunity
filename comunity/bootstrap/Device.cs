
using UnityEngine;

namespace comunity
{
    public static class Device
    {
        public static string model
        {
            get
            {
                if (Application.isEditor)
                {
                    return ProjPrefs.device;
                } else
                {
                    return SystemInfo.deviceModel;
                }
            }
        }

        private static int _width;
        private static int _height;
        public static int width
        {
            get
            {
                if (_width == 0)
                {
                    _width = Screen.width;
                    _height = Screen.height;
                }
                return _width;
            }
        }
        public static int height
        {
            get
            {
                if (_height == 0)
                {
                    _width = Screen.width;
                    _height = Screen.height;
                }
                return _height;
            }
        }

        public static bool supportASTC
        {
            get
            {
                if (Application.isEditor)
                {
                    return true;
                }
                return SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_RGBA_8x8);
            }
        }
    }
}