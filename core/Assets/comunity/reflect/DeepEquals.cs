using System;
using System.Reflection;
using System.Collections.Generic;

namespace core {
	public class DeepEquals
	{
		private readonly MemberInfoRegistry registry = new MemberInfoRegistry();

		/// <summary>
		/// if equals, return true
		/// </summary>
		/// <param name="o1">O1.</param>
		/// <param name="o2">O2.</param>
		public bool Compare(object o1, object o2) {
			if (o1 == null && o2 == null) {
				return true;
			}
			if (o1 == null || o2 == null) {
				return false;
			}
			Type t = o1.GetType();
			List<FieldInfo> fields = registry.GetFields(t);
			foreach (FieldInfo f in fields) {
				object v1 = f.GetValue (o1);
				object v2 = f.GetValue (o2);
				if (f.FieldType.IsClass) {
					if (!Compare(v1, v2)) {
						return false;
					}
				} else {
					if (!object.Equals(v1, v2)) {
						return false;
					}
				}
			}
			return true;
		}
	}
}

