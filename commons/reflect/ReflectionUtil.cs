//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Reflection;
using System.Collections.Generic;

namespace commons
{
	public static class ReflectionUtil
	{
		public static readonly Loggerx log = LogManager.GetLogger(typeof(ReflectionUtil));

		/// <summary>
		/// Enable reflection in iOS
		/// </summary>
		public static void ForceMonoReflection()
		{
			System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		}

		public static readonly BindingFlags INSTANCE_FLAGS = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.FlattenHierarchy;
		public static readonly BindingFlags VAR_FLAGS = INSTANCE_FLAGS&~BindingFlags.SetProperty&~BindingFlags.GetProperty;

		public static T NewInstance<T>()
		{
//			if (typeof(ScriptableObject).IsAssignableFrom(typeof(T)))
//			{
//				return default(T);
//			} else 
                if (typeof(string).IsAssignableFrom(typeof(T)))
			{
				return default(T);
			}
			return (T)Activator.CreateInstance(typeof(T));
			//        ConstructorInfo c = typeof(T).GetConstructor(new Type[0]);
			//        Object o = c.Invoke(new Object[] {});
			//        return (T)o;
			//		return (D)typeof(D).GetConstructor(new Type[] { typeof(E)}).Invoke(new object[] {e})
		}

		public static T NewInstance<T>(string clsName)
		{
			return (T)Activator.CreateInstance(GetType(clsName));
		}

		public static void SetFieldValue<T>(object obj, string fieldName, T fieldValue)
		{
			BindingFlags flags = INSTANCE_FLAGS|BindingFlags.Static;
			obj.GetType().GetField(fieldName, flags).SetValue(obj, fieldValue);
		}

		public static void SetStaticFieldValue<T>(Type type, string fieldName, T fieldValue)
		{
			BindingFlags flags = INSTANCE_FLAGS|BindingFlags.Static;
			type.GetField(fieldName, flags).SetValue(null, fieldValue);
		}

		public static FieldInfo GetField(object obj, string fieldName)
		{
			BindingFlags flags = INSTANCE_FLAGS|BindingFlags.Static;
			return obj.GetType().GetField(fieldName, flags);
		}

		public static FieldInfo GetField<T>(string fieldName)
		{
			BindingFlags flags = INSTANCE_FLAGS|BindingFlags.Static;
			return typeof(T).GetField(fieldName, flags);
		}

		public static List<FieldInfo> ListFields(Type type, ICollection<Type> excludes)
		{
			return ListFields(type, VAR_FLAGS, excludes);
		}

		public static List<FieldInfo> ListFields(Type type, BindingFlags flags, ICollection<Type> excludes)
		{
			List<FieldInfo> fields = new List<FieldInfo>();
			HashSet<Type> excludeSet = excludes != null? new HashSet<Type>(excludes) : null;
			foreach (FieldInfo f in type.GetFields(flags))
			{
				if (excludeSet == null||!excludeSet.Contains(f.DeclaringType))
				{
					fields.Add(f);
				}
			}
			fields.Sort(new FieldInfoComparer());
			return fields;
		}

		public static T GetFieldValue<T>(object obj, string fieldName)
		{
			FieldInfo field = GetField(obj, fieldName);
			object v = field.GetValue(obj);
			if (v == null)
			{
				return default(T);
			}
			return (T)v;
		}

		public static T GetStaticFieldValue<T>(string fieldName)
		{
			FieldInfo field = GetField<T>(fieldName);
			object v = field.GetValue(null);
			if (v == null)
			{
				return default(T);
			}
			return (T)v;
		}

		public static T GetAttribute<T>(FieldInfo f) where T:Attribute
		{
			foreach (Attribute attr in f.GetCustomAttributes(true))
			{
				if (attr.TypeId == typeof(T))
				{
					return attr as T;
				}
			}
			return null;
		}

		public static T GetAttribute<T>(Type type) where T:Attribute
		{
			foreach (Attribute attr in type.GetCustomAttributes(true))
			{
				if (attr.TypeId == typeof(T))
				{
					return attr as T;
				}
			}
			return null;
		}

		public static T GetAttribute<T>(object obj, string fieldName) where T:Attribute
		{
			FieldInfo f = GetField(obj, fieldName);
			Type fieldType = f.FieldType;
			if (fieldType.IsArray)
			{
				fieldType = fieldType.GetElementType();
			}
			return GetAttribute<T>(fieldType);
		}

		public static List<PropertyInfo> ListProperty(Type type, BindingFlags flags, ICollection<Type> excludes)
		{
			List<PropertyInfo> props = new List<PropertyInfo>();
			HashSet<Type> excludeSet = excludes != null? new HashSet<Type>(excludes) : null;
			foreach (PropertyInfo p in type.GetProperties(flags))
			{
				if (excludeSet == null||!excludeSet.Contains(p.DeclaringType))
				{
					props.Add(p);
				}
			}
			props.Sort(new PropertyInfoComparer());
			return props;
		}

		public static PropertyInfo GetProperty(object obj, string propName)
		{
			BindingFlags flags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.Instance|BindingFlags.FlattenHierarchy|BindingFlags.GetProperty|BindingFlags.SetProperty;
			return obj.GetType().GetProperty(propName, flags);
		}

		public static object GetPropertyValue(object obj, string propName)
		{
			return GetProperty(obj, propName).GetValue(obj, null);
		}

		public static void SetPropertyValue(object obj, string propName, object val)
		{
			GetProperty(obj, propName).SetValue(obj, val, null);
		}

		public static List<MethodInfo> ListMethods(Type type, ICollection<Type> excludes)
		{
			return ListMethods(type, VAR_FLAGS, excludes);
		}

		public static List<MethodInfo> ListMethods(Type type, BindingFlags flags, ICollection<Type> excludes)
		{
			List<MethodInfo> methods = new List<MethodInfo>();
			HashSet<Type> excludeSet = excludes != null? new HashSet<Type>(excludes) : null;
			foreach (MethodInfo m in type.GetMethods(flags))
			{
				if (excludeSet == null||!excludeSet.Contains(m.DeclaringType)&&!m.IsSpecialName)
				{
					methods.Add(m);
				}
			}
			methods.Sort(new MethodInfoComparer());
			return methods;
		}

		public static object Invoke(object instance, string methodName, params object[] parameters)
		{
			MethodInfo method = instance.GetType().GetMethod(methodName, ReflectionUtil.VAR_FLAGS);
			return method.Invoke(instance, parameters);
		}


		private static int assemblyCounts;
		private static Dictionary<string, Type> types;

		public static Type GetType(string fullName)
		{
			if (fullName.IsEmpty())
			{
				return null;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			if (assemblyCounts != assemblies.Length)
			{
				assemblyCounts = assemblies.Length;
				types = new Dictionary<string, Type>();
				foreach (Assembly assembly in assemblies)
				{
					foreach (Type type in assembly.GetTypes())
					{
						types[type.FullName] = type;
					}
				}
			}
			return types.Get(fullName);
		}

		/// <summary>
		/// Execs the script
		/// </summary>
		/// <param name="methodStr">[class_name].[static_method_name]</param>
		public static object ExecuteMethod(string methodStr)
		{
			if (methodStr.IsEmpty())
			{
				return null;
			}
			int dot = methodStr.LastIndexOf('.');
			string className = methodStr.Substring(0, dot);
			string method = methodStr.Substring(dot+1, methodStr.Length-(dot+1));
			return ReflectionUtil.GetType(className).GetMethod(method).Invoke(null, null);
		}

		private static MultiMap<Type, Type> attrTypes;

		public static List<Type> FindClassesWithAttribute<T>() where T: Attribute
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			if (attrTypes == null)
			{
				attrTypes = new MultiMap<Type, Type>();
			}
			if (!attrTypes.ContainsKey(typeof(T)))
			{
				foreach (Assembly assembly in assemblies)
				{
					foreach (Type type in assembly.GetTypes())
					{
						if (type.GetCustomAttributes(typeof(T), true).IsNotEmpty())
						{
							attrTypes.Add(typeof(T), type);
						}
					}
				}
			}
			return attrTypes[typeof(T)];
		}

		public static List<Type> FindClasses<T>() where T: class
		{
			return FindClasses(typeof(T));
		}

		public static List<Type> FindClasses(Type type)
		{
			List<Type> found = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				try
				{
					foreach (Type t in assembly.GetTypes())
					{
						if (type.IsAssignableFrom(t))
						{
							found.Add(t);
						}
					}
				} catch (Exception ex)
				{
					log.Error(ex);
				}
			}
			return found;
		}

		private class PropertyInfoComparer : Comparer<PropertyInfo>
		{
			public override int Compare(PropertyInfo x, PropertyInfo y)
			{
				if (x != null)
				{
					if (y == null)
					{
						return -1;
					} else
					{
						return x.Name.CompareTo(y.Name);
					}
				}
				return 1;
			}
		}

		private class FieldInfoComparer : Comparer<FieldInfo>
		{
			public override int Compare(FieldInfo x, FieldInfo y)
			{
				if (x != null)
				{
					if (y == null)
					{
						return -1;
					} else
					{
						return x.Name.CompareTo(y.Name);
					}
				}
				return 1;
			}
		}

		private class MethodInfoComparer : Comparer<MethodInfo>
		{
			public override int Compare(MethodInfo x, MethodInfo y)
			{
				if (x != null)
				{
					if (y == null)
					{
						return -1;
					} else
					{
						return x.Name.CompareTo(y.Name);
					}
				}
				return 1;
			}
		}
	}
}
