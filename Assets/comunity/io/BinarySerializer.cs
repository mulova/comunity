using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using commons;

namespace comunity
{
	
    /**
	 * Wraps BinarySerializer and use Surrogate so non 'Serializable' object can be serialized.
	 */
    public class BinarySerializer : IDisposable
    {
        public static readonly Loggerx log = LogManager.GetLogger (typeof(BinarySerializer));
        private Stream stream;
        private BinaryFormatter formatter;
        private bool enabled = true;
        private string path;
        private Exception exception;

        private BinaryFormatter Formatter {
            get {
                if (formatter == null) {
                    formatter = new BinaryFormatter ();
                    #if !UNITY_WEBGL
//					if (!Platform.platform.IsIOS()) {
                    SurrogateSelector ss = new SurrogateSelector ();
                    ss.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), new Vector2Surrogate ());
                    ss.AddSurrogate (typeof(Vector3), new StreamingContext (StreamingContextStates.All), new Vector3Surrogate ());
                    ss.AddSurrogate (typeof(Vector4), new StreamingContext (StreamingContextStates.All), new Vector4Surrogate ());
                    formatter.SurrogateSelector = ss;
//					}
                    #endif
                }
                return formatter;
            }
        }

        /**
		 * Create Serializer using MemoryStream
		 */
        public BinarySerializer (int capacity)
        {
            stream = new MemoryStream (capacity);
        }

        /**
		 * Create Deserializer using MemorySteam
		 */
        public BinarySerializer (TextAsset asset)
        {
            stream = new MemoryStream (asset.bytes, false);
        }

        /// <summary>
        /// Create serializer which flush every Serialize() call. Or Create Deserializer which reads file.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="access">Access.</param>
        public BinarySerializer (string path, FileAccess access)
        {
            this.path = path;
            #if !UNITY_WEBGL
            try {
                if (access == FileAccess.Write || access == FileAccess.ReadWrite) {
                    string dir = PathUtil.GetDirectory (path);
                    if (dir.IsNotEmpty () && !Directory.Exists (dir)) {
                        Directory.CreateDirectory (dir);
                    }
                    stream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write);
                    PlatformMethods.inst.SetNoBackupFlag (path);
                } else {
                    stream = new FileStream (path, FileMode.Open, FileAccess.Read);
                }
            } catch (Exception ex) {
                DoErrorHandling (ex);
                enabled = false;
            }
            #endif
        }

        public BinarySerializer (byte[] data)
        {
            stream = new MemoryStream (data);
        }

        public Exception Serialize (object obj)
        {
            this.exception = null;
            if (enabled == false || stream == null) {
                return null;
            }
            try {
                Formatter.Serialize (stream, obj);
                stream.Flush ();
                return null;
            } catch (Exception ex) {
                DoErrorHandling (ex);
                return ex;
            }
        }

        public void SerializeDictionary<K, V> (Dictionary<K, V> dic)
        {
            List<K> keys = new List<K> ();
            List<V> values = new List<V> ();
            foreach (KeyValuePair<K, V> pair in dic) {
                keys.Add (pair.Key);
                values.Add (pair.Value);
            }
            Serialize (keys.ToArray ());
            Serialize (values.ToArray ());
        }

        public T Deserialize<T> ()
        {
            this.exception = null;
            if (enabled == false || stream == null) {
                return default(T);
            }
            try {
                return (T)Formatter.Deserialize (stream);
            } catch (Exception ex) {
                DoErrorHandling (ex);
                return default(T);
            }
        }

        public string[] DeserializeStringArray ()
        {
            this.exception = null;
            if (enabled == false || stream == null) {
                return null;
            }
            try {
                return (string[])Formatter.Deserialize (stream);
            } catch (Exception ex) {
                DoErrorHandling (ex);
                return null;
            }
        }

        public Dictionary<K, V> DeserializeDictionary<K, V> ()
        {
            K[] keys = Deserialize<K[]> ();
            V[] values = Deserialize<V[]> ();
            if (keys != null && values != null && keys.Length == values.Length) {
                Dictionary<K, V> dic = new Dictionary<K, V> ();
                for (int i = 0; i < keys.Length; i++) {
                    dic [keys [i]] = values [i];
                }
                return dic;
            }
            return null;
        }

        private void DoErrorHandling (Exception ex)
        {
            if (ex != null) {
                log.Error (ex, null);
            }
            this.exception = ex;
        }

        public Exception GetException ()
        {
            return this.exception;
        }

        public bool IsAvailable ()
        {
            return stream == null || stream.Position < stream.Length;
        }

        public Exception Close ()
        {
            this.exception = null;
            if (stream != null) {
                try {
                    stream.Flush ();
                    stream.Close ();
                    if (path.IsNotEmpty ()) {
                        PlatformMethods.inst.SetNoBackupFlag (path);
                    }
                    stream = null;
                    return null;
                } catch (Exception ex) {
                    DoErrorHandling (ex);
                    return ex;
                }
            }
            return null;
        }

        public void SetEnabled (bool enable)
        {
            this.enabled = enable;
        }

        public bool Enabled ()
        {
            return enabled;
        }

        public void Dispose ()
        {
            Close();
        }
    }
	
    #if !UNITY_WEBGL
    sealed class Vector2Surrogate : ISerializationSurrogate
    {
        public void GetObjectData (System.Object obj, SerializationInfo info, StreamingContext context)
        {
            Vector2 vec2 = (Vector2)obj;
            info.AddValue ("x", vec2.x);
            info.AddValue ("y", vec2.y);
        }

        public System.Object SetObjectData (System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector2 vec2 = (Vector2)obj;
            vec2.x = (float)info.GetValue ("x", typeof(float));
            vec2.y = (float)info.GetValue ("y", typeof(float));
            return vec2;
        }
    }

    sealed class Vector3Surrogate : ISerializationSurrogate
    {
        public void GetObjectData (System.Object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 vec3 = (Vector3)obj;
            info.AddValue ("x", vec3.x);
            info.AddValue ("y", vec3.y);
            info.AddValue ("z", vec3.z);
        }

        public System.Object SetObjectData (System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 vec3 = (Vector3)obj;
            vec3.x = (float)info.GetValue ("x", typeof(float));
            vec3.y = (float)info.GetValue ("y", typeof(float));
            vec3.z = (float)info.GetValue ("z", typeof(float));
            return vec3;
        }
    }

    sealed class Vector4Surrogate : ISerializationSurrogate
    {
        public void GetObjectData (System.Object obj, SerializationInfo info, StreamingContext context)
        {
            Vector4 vec4 = (Vector4)obj;
            info.AddValue ("x", vec4.x);
            info.AddValue ("y", vec4.y);
            info.AddValue ("z", vec4.z);
            info.AddValue ("w", vec4.w);
        }

        public System.Object SetObjectData (System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector4 vec4 = (Vector4)obj;
            vec4.x = (float)info.GetValue ("x", typeof(float));
            vec4.y = (float)info.GetValue ("y", typeof(float));
            vec4.z = (float)info.GetValue ("z", typeof(float));
            vec4.w = (float)info.GetValue ("w", typeof(float));
            return vec4;
        }
    }
    #endif
}
