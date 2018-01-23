using System;
using UnityEngine;

using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Text;
using crypto.ex;
using commons;

namespace core
{
    /// <summary>
    /// Game preference.
    /// No error even if no data exists
    /// </summary>
    public class GamePref
    {
        private PropertiesReader pref;
        private string prefPath;
        private bool flush;

        public GamePref(string id, bool flushEverytime)
        {
            this.flush = flushEverytime;
            prefPath = PathUtil.Combine(Platform.downloadPath, id);
            PlatformMethods.inst.SetNoBackupFlag(prefPath);
            pref = new PropertiesReader();
            if (PropertiesReader.Exists(prefPath))
            {
                pref.LoadFile(prefPath);
            }
        }

        public void Save()
        {
            pref.Save(prefPath);
        }

        public bool HasKey(string key)
        {
            return pref.HasKey(key);
        }

        public bool GetBool(object key, bool b)
        {
            string k = key.ToText();
            if (HasKey(k))
            {
                return pref.GetString(k, "0") != "0";
            }
            return b;
        }

        public void SetBool(object key, bool b)
        {
            pref.Put(key.ToText(), b? "1" : "0");
            if (flush)
            {
                Save();
            }
        }

        public int GetInt(object key, int i)
        {
            string k = key.ToText();
            return pref.GetInt(k, i);
        }

        public void SetInt(object key, int i)
        {
            pref.Put(key.ToText(), i);
            if (flush)
            {
                Save();
            }
        }

        public float GetFloat(object key, float defaultValue)
        {
            string k = key.ToText();
            return pref.GetFloat(k, defaultValue);
        }

        public void SetFloat(object key, float f)
        {
            pref.Put(key.ToText(), f);
            if (flush)
            {
                Save();
            }
        }

        public double GetDouble(object key, double defaultValue)
        {
            string k = key.ToText();
            return pref.GetDouble(k, defaultValue);
        }

        public void SetDouble(object key, double d)
        {
            pref.Put(key.ToText(), d);
            if (flush)
            {
                Save();
            }
        }

        public string GetString(object key, string defaultValue)
        {
            string k = key.ToText();
            return pref.GetString(k, defaultValue);
        }

        public void SetString(object key, string s)
        {
            pref.Put(key.ToText(), s);
            if (flush)
            {
                Save();
            }
        }

        public T GetEnum<T>(object key, T defaultValue) where T:struct, IComparable, IConvertible, IFormattable
        {
            string str = GetString(key.ToText(), null);
            return EnumUtil.Parse(str, defaultValue);
        }

        public void SetEnum(object key, Enum e)
        {
            SetString(key.ToText(), e.ToText());
            if (flush)
            {
                Save();
            }
        }

        public void Remove(object key)
        {
            pref.Remove(key.ToText());
            if (flush)
            {
                Save();
            }
        }

        public List<string> GetList(object key, List<string> defaultValue, string separator = ",")
        {
            return GetList(key, s => s, defaultValue, separator);
        }

        public List<T> GetList<T>(object key, Func<string, T> converter, List<T> defaultValue, string separator = ",")
        {
            string str = GetString(key.ToText(), null);
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    string[] tokens = str.Split(new string[] { separator }, StringSplitOptions.None);
                    List<T> list = new List<T>();
                    foreach (string t in tokens)
                    {
                        list.Add(converter(t));
                    }
                    return list;
                } catch
                {
                }
            }
            return defaultValue;
        }

        public void SetList<T>(object key, List<T> values, string separator = ",")
        {
            SetString(key, StringUtil.Join(separator, values));
        }

        public void SetDictionary<K, V>(object key, Dictionary<K, V> values, string separator = "/")
        {
            SetString(key, StringUtil.Join(separator, values));
        }

        public Dictionary<K, V> GetDictionary<K, V>(object key, Func<string, K> keyConverter, Func<string, V> valueConverter)
        {
            string value = GetString(key.ToText(), null);
            if (value.IsEmpty())
            {
                return new Dictionary<K, V>();
            }

            try
            {
                string[] pairArr = value.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<K, V> dic = new Dictionary<K, V>();
                // pairStr format is [Key, Value]
                foreach (string pairStr in pairArr)
                {
                    string[] strs = pairStr.Substring(1, pairStr.Length-2).Split(new char[] { ',' });
                    dic.Add(keyConverter(strs[0]), valueConverter(strs[1]));
                }
                return dic;
            } catch
            {
                Debug.LogError("Some parsing error occurred");
                return null;
            }
        }
    }
}
