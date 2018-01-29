using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using commons;

namespace comunity {
	public class SerializationUtil  {
		public static Color ReadColor(string name, SerializationInfo info, StreamingContext context) {
			Color c;
			c.r = (float)info.GetDouble(name+".r");
			c.g = (float)info.GetDouble(name+".g");
			c.b = (float)info.GetDouble(name+".b");
			c.a = (float)info.GetDouble(name+".a");
			return c;
		}
		
		public static void WriteColor(Color c, string name, SerializationInfo info, StreamingContext context) {
			info.AddValue(name+".r", c.r);
			info.AddValue(name+".g", c.g);
			info.AddValue(name+".b", c.b);
			info.AddValue(name+".a", c.a);
		}
		
		public static Vector2 ReadVector2(string name, SerializationInfo info, StreamingContext context) {
			Vector2 vec;
			vec.x = (float)info.GetDouble(name+".x");
			vec.y = (float)info.GetDouble(name+".y");
			return vec;
		}
		
		public static void WriteVector2(Vector2 vec, string name, SerializationInfo info, StreamingContext context) {
			info.AddValue(name+".x", vec.x);
			info.AddValue(name+".y", vec.y);
		}
		
		public static void WriteVector3(Vector3 v, TextWriter writer) {
			writer.WriteLine(v.x.ToString());
			writer.WriteLine(v.y.ToString());
			writer.WriteLine(v.z.ToString());
		}
		
		public static Vector3 ReadVector3(TextReader reader) {
			float x = float.Parse(reader.ReadLine());
			float y = float.Parse(reader.ReadLine());
			float z = float.Parse(reader.ReadLine());
			return new Vector3(x, y, z);
		}
		
		public static Vector3 ReadVector3(string name, SerializationInfo info, StreamingContext context) {
			Vector3 vec;
			vec.x = (float)info.GetDouble(name+".x");
			vec.y = (float)info.GetDouble(name+".y");
			vec.z = (float)info.GetDouble(name+".z");
			return vec;
		}
		
		public static void WriteVector3(Vector3 vec, string name, SerializationInfo info, StreamingContext context) {
			info.AddValue(name+".x", vec.x);
			info.AddValue(name+".y", vec.y);
			info.AddValue(name+".z", vec.z);
		}
		
		public static T ReadEnum<T>(string name, T defaultValue, SerializationInfo info, StreamingContext context) where T : struct, IComparable, IConvertible, IFormattable {
			string enumName = info.GetString(name);
			return EnumUtil.Parse<T>(enumName, defaultValue);
		}
		
		public static void WriteEnum<T>(string name, T enumValue, SerializationInfo info, StreamingContext context) {
			info.AddValue(name, enumValue.ToString());
		}
		
		public static T ReadObject<T>(string path) where T : class {
			if (File.Exists(path)==false) {
				return default(T);
			}
			#if !UNITY_WEBGL || UNITY_EDITOR
			BinarySerializer reader = new BinarySerializer(path, FileAccess.Read);
			T obj = reader.Deserialize<T>();
			reader.Close();
			return obj;
			#else
			return null;
			#endif
		}
		
		public static bool WriteObject<T>(string path, T t) where T : class {
			#if !UNITY_WEBGL || UNITY_EDITOR
			BinarySerializer writer = new BinarySerializer(path, FileAccess.Write);
			bool success = writer.Serialize(t) == null;
			success &= writer.Close() == null;
			PlatformMethods.inst.SetNoBackupFlag(path);
			return success;
			#else
			return false;
			#endif
		}
	}
}
