//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using mulova.commons;

namespace mulova.comunity {
	/**
	 * Copy object's property, field values.
	 * Exclude() to exclude specific type's properties/fields
	 */
	public class ObjCopy {
		public delegate bool IsAssignable(MemberInfo m, object oldValue, object newValue);
		
		private Dictionary<string, bool> excludeTypes = new Dictionary<string, bool>();
		private MultiKeyMap<string, string, bool> excludeFields = new MultiKeyMap<string, string, bool>();
		private IsAssignable assignable;
		private bool includeProperty;
		
		public static BindingFlags FLAGS = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
		
		public ObjCopy(bool copyProperty) : this(copyProperty, null) {
		}
		
		/**
		 * @param copyProperty true이면 property 값도 설정한다.
		 * @param copyRef true이면 reference type도 복사한다.
		 */
		public ObjCopy(bool copyProperty, IsAssignable assignable) {
			this.assignable = assignable==null? IsAssignable0: assignable;
			this.includeProperty = copyProperty;
		}
		
		public void ExcludeType(params string[] typeNames) {
			foreach (string t in typeNames) {
				excludeTypes[t] = true;
			}
		}
		
		public void ExcludeField(string typeName, string fieldName) {
			excludeFields.Add(typeName, fieldName, true);
		}
		
		/**
		 * @return changed members/properties
		 */
		public List<MemberInfo> SetValue(Component src, Component dst) {
			List <MemberInfo> ret = new List<MemberInfo>();

			if (includeProperty) {
				Dictionary<PropertyInfo, object> properties = new Dictionary <PropertyInfo, object> ();
				foreach (PropertyInfo property in GetProperties(src))
				{
					if (src is Transform && property.Name == "parent") {
						continue;
					}
					properties [property] = property.GetValue (src, null);
				}
				
				foreach (PropertyInfo property in GetProperties(dst)) {
					if (properties.ContainsKey(property)) {
						object newValue = properties[property];
						object oldValue = property.GetValue(dst, null);
						if (assignable(property, oldValue, newValue)) {
							try {
								property.SetValue(dst, newValue, null);
								ret.Add(property);
							} catch (Exception ex) {
								Exception e = ex.InnerException!=null? ex.InnerException: ex;
								if (!(e is System.NotImplementedException)) {
									Debug.LogError(e.Message);
								}
							}
						}
					}
				}
			}
			
			Dictionary<FieldInfo, object> fields = new Dictionary <FieldInfo, object> ();
			foreach (FieldInfo field in GetFields(src))
			{
				fields [field] = field.GetValue (src);
			}
						
			
			List <FieldInfo> fieldList = GetFields (dst);
			foreach (FieldInfo field in fieldList)
			{
				if (fields.ContainsKey(field)) {
					object newValue = fields[field];
					object oldValue = field.GetValue(dst);
					if (assignable(field, oldValue, newValue)) {
						field.SetValue(dst, fields [field]);
						ret.Add(field);
					}
				}
			}
			return ret;
		}
		
		private List <PropertyInfo> GetProperties(Component component)
		{
			List <PropertyInfo> properties = new List <PropertyInfo> ();
			foreach (PropertyInfo propertyInfo in component.GetType().GetProperties())
			{
				string typeName = propertyInfo.DeclaringType.FullName;
				if (excludeTypes.ContainsKey(typeName)) {
					continue;
				}
				if (excludeFields.Get(typeName, propertyInfo.Name))
				{
					continue;
				}
				if (propertyInfo.CanRead && propertyInfo.CanWrite) {
					properties.Add (propertyInfo);
				}
			}
			
			return properties;
		}
		
		private List <FieldInfo> GetFields (Component component)
		{
			List <FieldInfo> fields;
			
			fields = new List <FieldInfo> ();
			foreach (FieldInfo field in component.GetType().GetFields(FLAGS))
			{
				string typeName = field.DeclaringType.FullName;
				if (excludeTypes.ContainsKey(typeName)) {
					continue;
				}
				if (excludeFields.Get(typeName, field.Name))
				{
					continue;
				}
				if (field.IsLiteral == false) {
					fields.Add (field);
				}
			}
			
			return fields;
		}
		
		private bool IsAssignable0(MemberInfo m, object oldValue, object newValue) {
			return true;
		}
	}
	
}

