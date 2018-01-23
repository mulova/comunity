using System;

namespace core
{
    /// <summary>
    /// Test configuration values.
    /// GetValue() always return default values on release build
    /// </summary>
    public static class TestConfig
    {
        private static PropertiesReader map;
        private static PropertiesReader Map {
            get {
                if (map == null) {
                    map = new PropertiesReader();
                    if (Platform.isDebug) {
                        map.LoadResource("test_config");
                    }
                }
                return map;
            }
        }
        
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <returns>return defValue on release build</returns>
        /// <param name="key">Key.</param>
        public static string GetString(string key, string defValue) {
            if (Platform.isReleaseBuild) {
                return defValue;
            }
            return Map.GetString(key, defValue);
        }
        
        /// <summary>
        /// Gets the bool.
        /// </summary>
        /// <returns>return defValue on release build
        /// <param name="key">Key.</param>
        public static bool GetBool(string key, bool defValue) {
            if (Platform.isReleaseBuild) {
                return defValue;
            }
            return Map.GetBool(key, defValue);
        }
        
        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <returns>return defValue on release build</returns>
        /// <param name="key">Key.</param>
        public static int GetInt(string key, int defValue) {
            if (Platform.isReleaseBuild) {
                return defValue;
            }
            return Map.GetInt(key, defValue);
        }
        
        /// <summary>
        /// Gets the long.
        /// </summary>
        /// <returns>return defValue on release build</returns>
        /// <param name="key">Key.</param>
        public static long GetLong(string key, long defValue) {
            if (Platform.isReleaseBuild) {
                return defValue;
            }
            return Map.GetLong(key, defValue);
        }
    }
}


