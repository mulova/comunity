using System.Collections.Generic;
using System.Reflection;
using commons;

namespace System.Ex
{
	public static class TypeEx
	{
		public static List<FieldInfo> ListFields(this Type type, BindingFlags flags, ICollection<Type> excludes)
		{
			return ReflectionUtil.ListFields(type, flags, excludes);
		}

		public static List<PropertyInfo> ListProperties(this Type type, BindingFlags flags, ICollection<Type> excludes)
		{
			return ReflectionUtil.ListProperty(type, flags, excludes);
		}
	}
}

