using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Collections;

namespace comunity
{
    public class PropertiesReader : IEnumerable<KeyValuePair<string, string>>
    {
	    public bool initialized { get; private set; }

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
	    public PropertiesReader()
	    {
	    }

	    public PropertiesReader(TextAsset database)
	    {
		    Load(database);
	    }

	    public IEnumerable<string> Keys {
		    get { return dic.Keys; }
	    }

	    public IEnumerable<string> Values {
		    get { return dic.Values; }
	    }

        public string this [string key] {
		    get {
			    string val = null;
			    dic.TryGetValue(key, out val);
			    return val;
		    }
		    set {
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
		    if (val != null && int.TryParse(val, out i)) {
			    return i;
		    }
		    return defVal;
	    }

	    public long GetLong(string key, long defVal)
	    {
		    string val = this[key];
		    long l = defVal;
		    if (val != null && long.TryParse(val, out l)) {
			    return l;
		    }
		    return defVal;
	    }

	    public float GetFloat(string key, float defVal)
	    {
		    string val = this[key];
		    float f = defVal;
		    if (val != null && float.TryParse(val, out f)) {
			    return f;
		    }
		    return defVal;
	    }

	    public double GetDouble(string key, double defVal)
	    {
		    string val = this[key];
		    double d = defVal;
		    if (val != null && double.TryParse(val, out d)) {
			    return d;
		    }
		    return defVal;
	    }

	    public bool GetBool(string key, bool defVal)
	    {
		    string val = this[key];
		    bool b = defVal;
		    if (val != null && bool.TryParse(val, out b)) {
			    return b;
		    }
		    return defVal;
	    }

	    public bool HasKey(string key)
	    {
		    return dic.ContainsKey(key);
	    }

	    public T GetEnum<T>(object key, T defaultValue) where T:struct, IComparable, IConvertible, IFormattable
	    {
		    string str = GetString(key.ToString(), null);
		    try 
		    {
			    return (T)Enum.Parse(typeof(T), str);
		    } catch (Exception ex)
		    {
			    Debug.LogException(ex);
			    return defaultValue;
		    }
	    }

	    public void Put(string key, object val)
	    {
		    string str = val != null ? val.ToString() : null;
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
		    return File.Exists(filePath);
	    }

	    public void LoadFile(string filePath, bool errorTolerant = false)
	    {
		    if (errorTolerant && !File.Exists(filePath)) {
			    return;
		    }
		    LoadText(File.ReadAllText(filePath));
	    }

	    public void Load(TextAsset textAsset)
	    {
		    if (textAsset == null) {
			    return;
		    }
		    LoadText(textAsset.text);
	    }

	    public void Clear()
	    {
		    dic.Clear();
	    }

	    public void LoadText(string content)
	    {
		    initialized = true;
		    StringReader r = new StringReader(content);
		    string line = null;
		    while ((line = r.ReadLine()) != null) {
			    if (line.Length == 0 || line.StartsWith("#") || line.StartsWith("//")) {
				    continue;
			    }
			    int i = line.IndexOf("=");
			    if (i > 0) {
				    string key = line.Substring(0, i).Trim();
				    string v = line.Substring(i + 1).Trim();
                    if (v.Length > 2 && ((v[0] == '"' && v[v.Length-1] == '"') || (v[0] == '\'' && v[v.Length - 1] == '\'')))
                    {
                        v = v.Substring(1, v.Length-2);
                    }
				    dic[key] = v;
			    } else {
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
		    File.WriteAllText(filePath, ToString());
	    }

	    public override string ToString()
	    {
		    StringBuilder str = new StringBuilder();
		    foreach (KeyValuePair<string, string> pair in dic) {
			    str.Append(pair.Key).Append('=');
			    if (!string.IsNullOrEmpty(pair.Value)) {
				    str.Append(pair.Value);
			    }
			    str.AppendLine();
		    }
		    return str.ToString();
	    }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)dic).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)dic).GetEnumerator();
        }
    }
}
