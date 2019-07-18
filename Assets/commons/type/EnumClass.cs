using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text.Ex;
using System.Collections.Generic.Ex;

namespace commons
{
    [Serializable]
    public class EnumClass<T> where T: EnumClass<T>
    {
        public string id { get { return _id; }}
        public string name { get { return _name; }}
        public int ordinal { get { return _ordinal; }}
        //    [UnityEngine.SerializeField]
        private string _id;
        //    [UnityEngine.SerializeField]
        private string _name;
        //    [UnityEngine.SerializeField]
        private int _ordinal;
        private static Dictionary<string, T> parser = new Dictionary<string, T>();
        private static List<T> list = new List<T>();
        private static List<T> nonNullList = new List<T>();
        private static Loggerx log = LogManager.GetLogger(typeof(T));
        
        public EnumClass() {} // serializable only
        
        protected EnumClass(string id) : this(id, id)  { }
        
        protected EnumClass(string id, int ordinal) : this(id, id, ordinal)  { }
        
        protected EnumClass(string id, string name) : this(id, name, list.Count) { }
        
        protected EnumClass(string id, string name, int ordinal)
        {
            if (id != null) {
                this._id = id;
                this._name = name;
                nonNullList.Add(this as T);
            } else {
                this._id = string.Empty;
                this._name = string.Empty;
            }
            this._ordinal = ordinal;
            parser.Add(this._id, this as T);
            if (this._id != this._name) {
                parser.Add(this._name, this as T);
            }
            list.Add(this as T);
        }
        
        public static int Size
        {
            get {
                Init();
                return list.Count;
            }
        }
        
        public static List<T> Values
        {
            get { 
                Init();
                return list;
            }
        }
        
        public static List<T> NonNullValues
        {
            get { 
                Init();
                return nonNullList;
            }
        }
        
        private static void Init()
        {
            // run static constructor if not called yet
            if (parser.Count == 0)
            {
                Type type = typeof(T);
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }
        
        public bool IsNull() {
            return _id.IsEmpty();
        }
        
        public T intern()
        {
            return Parse(id);
        }
        
        public static implicit operator int(EnumClass<T> e)
        {
            return e._ordinal;
        }
        
        public static implicit operator EnumClass<T>(int i)
        {
            if (i >= list.Count)
            {
                log.Error("index ({0}) out of range{1}", i, list.Count);
                return list[0];
            }
            return list[i];
        }
        
        public static implicit operator EnumClass<T>(string str)
        {
            return Parse(str);
        }
        
        public static implicit operator string(EnumClass<T> str)
        {
            if (str != null)
            {
                return str.id;
            } else
            {
                return null;
            }
        }
        
        public static T ParseIgnoreCase(string str)
        {
            return ParseIgnoreCase (str, default(T));
        }
        
        public static T ParseIgnoreCase(string str, T defValue)
        {
            Init ();
            if (!str.IsEmpty ()) {
                KeyValuePair<string, T> val = parser.Find (pair => pair.Key.EqualsIgnoreCase (str));
                return val.Key.IsEmpty() ? defValue : val.Value;
            } else {
                return defValue;
            }
        }
        
        public static T Parse(string str)
        {
            return Parse(str, default(T));
        }
        
        public static T Parse(string str, T defValue)
        {
            Init();
            if (!str.IsEmpty())
            {
                T val = null;
                if (parser.TryGetValue(str, out val)) {
                    return val;
                }
            }
            return defValue;
        }
        
        public static bool operator >=(EnumClass<T> a, EnumClass<T> b)
        {
            return a._ordinal >= b._ordinal;
        }
        
        public static bool operator <=(EnumClass<T> a, EnumClass<T> b)
        {
            return a._ordinal <= b._ordinal;
        }
        
        public static bool operator >(EnumClass<T> a, EnumClass<T> b)
        {
            return a._ordinal > b._ordinal;
        }
        
        public static bool operator <(EnumClass<T> a, EnumClass<T> b)
        {
            return a._ordinal < b._ordinal;
        }
        
        public static bool operator ==(EnumClass<T> a, EnumClass<T> b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            
            // If one is null, but not both, return false.
            if (System.Object.Equals(a, null) || System.Object.Equals(b, null))
            {
                return false;
            }
            
            // Return true if the fields match:
            return a._id == b._id;
        }
        
        public static bool operator !=(EnumClass<T> a, EnumClass<T> b)
        {
            return !(a == b);
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is string)
            {
                string str = obj as string;
                return _id == str;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            EnumClass<T> that = obj as EnumClass<T>;
            return that._id == this._id;
        }
        
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
        
        public override string ToString()
        {
            return _name;
        }
    }
    
}