using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using commons;

namespace core
{
	/// <summary>
	/// text asset을 읽어 String -> String  map을 생성한다.
	/// </summary>
	public class PropertiesReader : Loggable
	{
		public bool initialized { get; private set; }
        public bool binary;
        private string path;

        private SortedDictionary<string, string> dic = new SortedDictionary<string, string>();

        public PropertiesReader(string path)
        {
            this.path = path;
            LoadFile(path);
        }

        /**
         * @param textAssetPath text-asset path in 'Resources' Folder
         */
        public PropertiesReader() { }

		public PropertiesReader(TextAsset database)
		{
			Load(database);
		}

		public IEnumerable<string> Keys
		{
			get { return dic.Keys; }
		}

		public IEnumerable<string> Values
		{
			get { return dic.Values; }
		}

		public string this[string key]
		{
			get
			{
				return dic.Get(key);
			}
			set
			{
				Put(key, value);
			}
		}

		public string GetString(string key, string defVal)
		{
            string val = defVal;
            dic.TryGetValue(key, out val);
			return val;
		}

		public int GetInt(string key, int defVal)
		{
			string val = this[key];
			int i = defVal;
			if (val != null&&int.TryParse(val, out i))
			{
				return i;
			}
			return defVal;
		}

		public long GetLong(string key, long defVal)
		{
			string val = this[key];
			long l = defVal;
			if (val != null&&long.TryParse(val, out l))
			{
				return l;
			}
			return defVal;
		}

		public float GetFloat(string key, float defVal)
		{
			string val = this[key];
			float f = defVal;
			if (val != null&&float.TryParse(val, out f))
			{
				return f;
			}
			return defVal;
		}

		public double GetDouble(string key, double defVal)
		{
			string val = this[key];
			double d = defVal;
			if (val != null&&double.TryParse(val, out d))
			{
				return d;
			}
			return defVal;
		}

		public bool GetBool(string key, bool defVal)
		{
			string val = this[key];
			bool b = defVal;
			if (val != null&&bool.TryParse(val, out b))
			{
				return b;
			}
            return BoolTypeEx.Parse(val).IsTrue();
		}

        public bool HasKey(string key)
        {
            return dic.ContainsKey(key);
        }

		public T GetEnum<T>(object key, T defaultValue) where T:struct, IComparable, IConvertible, IFormattable
		{
			string str = GetString(key.ToString(), null);
			return EnumUtil.Parse(str, defaultValue);
		}

		public void Put(string key, object val)
		{
			string str = val != null? val.ToString() : null;
			dic[key] = str;
		}

        public void Remove(string key)
        {
            dic.Remove(key);
        }

		public void LoadResource(string resPath)
		{
			TextAsset db = Resources.Load(resPath) as TextAsset;
			Load(db);
		}

        public static bool Exists(string filePath)
        {
            #if UNITY_WEBGL
            return PlayerPrefs.HasKey(filePath);
            #else
            return File.Exists(filePath);
            #endif      
        }

		public void LoadFile(string filePath, bool errorTolerant = false)
		{
#if UNITY_WEBGL
            LoadText(PlayerPrefs.GetString(filePath, string.Empty));
#else
            if (errorTolerant && !File.Exists(filePath))
            {
                return;
            }
            if (binary)
            {
                LoadBinary(File.ReadAllBytes(filePath));
            } else
            {
                LoadText(File.ReadAllText(filePath));
            }
#endif
		}

		public void Load(TextAsset textAsset)
		{
			if (textAsset == null)
			{
				return;
			}
            if (binary)
            {
                LoadBinary(textAsset.bytes);
            } else
            {
                LoadText(textAsset.text);
            }
		}

        public void Clear()
        {
            dic.Clear();
        }

        public void LoadBinary(byte[] bytes)
        {
            var ser = new BinarySerializer(bytes);
            foreach (var pair in ser.Deserialize<SortedDictionary<string, string>>())
            {
                dic.Add(pair.Key, pair.Value);
            }
            ser.Close();
        }

        public void LoadText(string content)
        {
			initialized = true;
            StringReader r = new StringReader(content);
            string line = null;
            while ((line = r.ReadLine()) != null)
            {
                if (line.Length == 0||line.StartsWith("#")||line.StartsWith("//"))
                {
                    continue;
                }
                int i = line.IndexOf("=");
                if (i > 0)
                {
                    string key = line.Substring(0, i).Trim();
                    string v = line.Substring(i+1).Trim();
                    dic[key] = v;
                } else
                {
                    dic[line.Trim()] = null;
                }
            }
		}

        public void Save()
        {
            Save(path);
        }

		public void Save(string filePath)
		{
            #if UNITY_WEBGL
            PlayerPrefs.SetString(path, ToString());
			#else
            if (binary)
            {
                BinarySerializer ser = new BinarySerializer(filePath, FileAccess.Write);
                ser.Serialize(dic);
                ser.Close();
            } else
            {
                string dir = PathUtil.GetDirectory (filePath);
                if (dir.IsNotEmpty() && !Directory.Exists (dir)) {
                    Directory.CreateDirectory (dir);
                }
                File.WriteAllText(filePath, ToString());
            }
			#endif
		}

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			foreach (KeyValuePair<string, string> pair in dic)
			{
                str.Append(pair.Key).Append('=');
                if (pair.Value.IsNotEmpty())
                {
                    str.Append(pair.Value);
                }
                str.AppendLine();
			}
			return str.ToString();
		}
	}
}