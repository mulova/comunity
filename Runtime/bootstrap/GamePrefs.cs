using System;
using System.Collections.Generic;
using UnityEngine;

namespace mulova.comunity
{
    public class GamePrefs
    {
	    public static readonly GamePrefs OptionPreset = new GamePrefs("_gfx_preset");
	    public static readonly GamePrefs OptionShadow = new GamePrefs("_gfx_texture");
	    public static readonly GamePrefs OptionResolution = new GamePrefs("_gfx_resolution");
	    public static readonly GamePrefs OptionFrame = new GamePrefs("_gfx_frame");
        public static readonly GamePrefs OptionAntialiasing = new GamePrefs("_gfx_antialiasing");
        public static readonly GamePrefs OptionBgm = new GamePrefs("_bgm_volume", 1f);
        public static readonly GamePrefs OptionSfx = new GamePrefs("_sfx_volume", 1f);
        public static readonly GamePrefs OptionVoice = new GamePrefs("_voice_volume", 1f);

        private static object[] DONT_CLEAR_STRING = new object[] { };
        private static object[] DONT_CLEAR_INT = new object[] { OptionPreset, OptionShadow, OptionResolution, OptionFrame, OptionAntialiasing };
        private static object[] DONT_CLEAR_FLOAT = new object[] { OptionBgm, OptionSfx, OptionVoice };

        public readonly string name;

	    private static List<GamePrefs> prefs;

	    private GamePrefs(string name, object defValue = null)
	    {
		    this.name = name;
		    if (prefs == null)
		    {
			    prefs = new List<GamePrefs>();
		    }
		    prefs.Add(this);
		    if (defValue != null && !Exists())
		    {
			    if (defValue is int)
			    {
				    Set((int)defValue);
			    } else if (defValue is bool)
			    {
				    Set((bool)defValue);
			    }
			    else if (defValue is string)
			    {
				    Set((string)defValue);
			    }
		    }
	    }

	    public override string ToString()
	    {
		    return name;
	    }

	    public override int GetHashCode()
	    {
		    return name.GetHashCode();
	    }

	    public override bool Equals(object obj)
	    {
		    if (obj is GamePrefs)
		    {
			    return this.name == (obj as GamePrefs).name;
		    }
		    return false;
	    }

	    public bool Exists()
	    {
		    return PlayerPrefs.HasKey(name);
	    }

        public int GetInt(int value = 0)
        {
            return PlayerPrefs.GetInt(name, value);
        }

        public long GetLong(long value = 0)
        {
            long.TryParse(PlayerPrefs.GetString(name), out value);
            return value;
        }

        public float GetFloat(float value = 0)
        {
            return PlayerPrefs.GetFloat(name, value);
        }

        public string GetString(string def = null)
	    {
		    return PlayerPrefs.GetString(name, def);
	    }

        public bool GetBool(bool val = false)
        {
            return GetBoolean(name, val);
        }

        public T Get<T>(T val) where T : struct, IComparable, IConvertible, IFormattable
        {
            return GamePrefs.GetEnum<T>(name, val);
        }

        public void Set(string value, bool save = true)
	    {
            PlayerPrefs.SetString(name, value);
            if (save)
            {
                PlayerPrefs.Save();
            }
        }

        public void Set(int value, bool save = true)
        {
            PlayerPrefs.SetInt(name, value);
            if (save)
            {
                PlayerPrefs.Save();
            }
        }

        public void Set(long value, bool save = true)
        {
            PlayerPrefs.SetString(name, value.ToString());
            if (save)
            {
                PlayerPrefs.Save();
            }
        }

        public void Set(float value, bool save = true)
        {
            PlayerPrefs.SetFloat(name, value);
            if (save)
            {
                PlayerPrefs.Save();
            }
        }

        public void Set(bool value, bool save = true)
        {
            PlayerPrefs.SetInt(name, value? 1: 0);
            if (save)
            {
                PlayerPrefs.Save();
            }
        }

        public void Remove(bool save = true)
	    {
		    PlayerPrefs.DeleteKey(name);
            if (save)
            {
                PlayerPrefs.Save();
            }
	    }

	    public static implicit operator int(GamePrefs g)
	    {
		    return g.GetInt(0);
	    }

        public static implicit operator bool(GamePrefs g)
        {
            return g.GetBool(false);
        }

        public static implicit operator float(GamePrefs g)
        {
            return g.GetFloat(0);
        }

        //public static implicit operator string(GamePrefs g)
        //{
        //	return g.GetString(null);
        //}

        public static void ClearAll()
	    {
            Dictionary<string, string> strMap = new Dictionary<string, string>();
            Dictionary<string, int> intMap = new Dictionary<string, int>();
            Dictionary<string, float> floatMap = new Dictionary<string, float>();
            foreach (var k in DONT_CLEAR_STRING)
            {
                var key = k.ToString();
                strMap[key] = PlayerPrefs.GetString(key);
            }
            foreach (var k in DONT_CLEAR_INT)
            {
                var key = k.ToString();
                intMap[key] = PlayerPrefs.GetInt(key);
            }
            foreach (var k in DONT_CLEAR_FLOAT)
            {
                var key = k.ToString();
                floatMap[key] = PlayerPrefs.GetFloat(key);
            }
            PlayerPrefs.DeleteAll();
            foreach (var pair in strMap)
            {
                PlayerPrefs.SetString(pair.Key, pair.Value);
            }
            foreach (var pair in intMap)
            {
                PlayerPrefs.SetInt(pair.Key, pair.Value);
            }
            foreach (var pair in floatMap)
            {
                PlayerPrefs.SetFloat(pair.Key, pair.Value);
            }
            PlayerPrefs.Save();
        }

        public static string Log()
        {
            var str = new System.Text.StringBuilder(10240);
            foreach (var p in prefs)
            {
                str.Append(p.name).Append('=').Append(p.GetString()).AppendLine();
            }
            return str.ToString();
        }

        public static T GetEnum<T>(string key, T val) where T : struct, IComparable, IConvertible, IFormattable
        {
            T e = val;
            var str = PlayerPrefs.GetString(key, null);
            if (str != null)
            {
                Enum.TryParse<T>(str, out e);
            }
            return e;
        }

        public static void SetEnum<T>(string key, T val) where T : struct, IComparable, IConvertible, IFormattable
        {
            PlayerPrefs.SetString(key, val.ToString());
        }

        public static bool GetBoolean(string key, bool val = false)
        {
            return PlayerPrefs.GetInt(key, val ? 1 : 0) != 0;
        }

        public static void SetBoolean(string key, bool val)
        {
            PlayerPrefs.SetInt(key, val ? 1 : 0);
        }

        public static void SetInt(string key, int val)
        {
            PlayerPrefs.SetInt(key, val);
        }

        public static void SetString(string key, string val)
        {
            PlayerPrefs.SetString(key, val);
        }
    }
}

