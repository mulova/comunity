//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Reflection;
using System.Text;
using commons;

public static class ObjectEx {

	public static bool EqualsEx(this object o1, object o2) {
		if (o1 == null ^ o2 == null) {
			return false;
		}
		if (o1 != null) {
			return o1.Equals(o2);
		}
		return true;
	}

	public static string ToText(this object o) {
		if (o == null) {
			return string.Empty;
		}
		return o.ToString();
	}

	public static string[] ToStringArr(this object[] arr) {
		string[] strArr = new string[arr.Length];
		for (int i=0; i<arr.Length; ++i) {
			strArr[i] = arr[i].ToText();
		}
		return strArr;
	}

	private static void Indent(StringBuilder sb, int indent)
	{
		// indent
		for (int i=0; i<indent; ++i)
		{
			sb.Append("  ");
		}
	}

	public static string ToStringReflection(this object obj) {
		StringBuilder sb = new StringBuilder();
		return ToStringReflection(obj, sb, 0);
	}

	private static string ToStringReflection(object obj, StringBuilder sb, int indent) {
		if (obj == null)
		{
			sb.Append("NULL");
			sb.AppendLine();
		} else if (obj.GetType().IsPrimitive || obj is string)
		{
			sb.Append(obj.ToString());
			sb.AppendLine();
		} else if (obj.GetType().IsEnum)
		{
			sb.AppendFormat("{0}.{1}", obj.GetType().FullName, obj.ToString());
			sb.AppendLine();
		} else
		{
			foreach (FieldInfo f in obj.GetType().GetFields(ReflectionUtil.INSTANCE_FLAGS))
			{
				Indent(sb, indent);
				sb.Append(f.Name);
				if (f.FieldType.IsArray)
				{
					Array arr = (f.GetValue(obj) as Array);
					if (arr == null)
					{
						sb.Append(" = NULL");
						sb.AppendLine();
					} else if (arr.Rank == 1)
					{
						int arrLength = arr.GetLength(0);
						sb.Append(" (arrsize ");
						sb.Append(arrLength.ToString());
						sb.Append(")");
						sb.AppendLine();
						for (int i=0; i<arrLength; ++i)
						{
							Indent(sb, indent+1);
							sb.Append("[");
							sb.Append(i.ToString());
							sb.Append("] ");
							object e = arr.GetValue(i);
							ToStringReflection(e, sb, i+1);
						}
					} else
					{
						sb.Append(" (arrdimension ");
						sb.Append(arr.Rank);
						sb.Append(")");
						sb.AppendLine();
					}
				}
				else
				{
					sb.Append(" = ");
					ToStringReflection(f.GetValue(obj), sb, indent+1);
				}
			}
		}
		return sb.ToString();
	}
}
