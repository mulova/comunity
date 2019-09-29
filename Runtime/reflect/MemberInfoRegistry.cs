using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Reflection;
using mulova.commons;
using UnityEngine;

namespace mulova.comunity
{
    /**
	 * Search the member variable's value
	 * Exclude() to exclude specific type's properties/fields
	 */
    public class MemberInfoRegistry
    {
        public delegate bool Filter(MemberInfo m);

        private MultiMap<Type, FieldInfo> fieldMap = new MultiMap<Type, FieldInfo>();
        private MultiMap<Type, PropertyInfo> propMap = new MultiMap<Type, PropertyInfo>();
        private HashSet<Type> excludeTypes = new HashSet<Type>();
        private MultiKeyMap<string, string, bool> excludeMembers = new MultiKeyMap<string, string, bool>();
        private Filter filter;
        private BindingFlags flags;
        private static Filter DUMMY_FILTER = m => true;
        private HashSet<int> fieldTraveled = new HashSet<int>();
        private HashSet<int> propertyTraveled = new HashSet<int>();
		
        public static BindingFlags FLAGS = (BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.FlattenHierarchy)&~BindingFlags.SetProperty&~BindingFlags.GetProperty;

        public MemberInfoRegistry() : this(DUMMY_FILTER, FLAGS)
        {
        }

        public MemberInfoRegistry(Filter filter) : this(filter, FLAGS)
        {
        }

        public MemberInfoRegistry(Filter filter, BindingFlags flags)
        {
            this.flags = flags;
            this.filter = filter;
            excludeMembers.Add("UnityEngine.Renderer", "material", true); // sharedMaterial is used 
            excludeMembers.Add("UnityEngine.Renderer", "materials", true); // sharedMaterial is used 
            excludeMembers.Add("UnityEngine.Material", "color", true);
            excludeMembers.Add("UnityEngine.Animator", "bodyRotation", true);
            excludeMembers.Add("UnityEngine.Animator", "bodyPosition", true);
            excludeMembers.Add("UnityEngine.Animator", "playbackTime", true);
/*
			excludeFields.Add("UnityEngine.Component", "rigidbody", true);
			excludeFields.Add("UnityEngine.Component", "rigidbody2D", true);
			excludeFields.Add("UnityEngine.Component", "camera", true);
			excludeFields.Add("UnityEngine.Component", "collider", true);
			excludeFields.Add("UnityEngine.Component", "collider2D", true);
			excludeFields.Add("UnityEngine.Component", "animation", true);
			excludeFields.Add("UnityEngine.Component", "hingeJoint", true);
			excludeFields.Add("UnityEngine.Component", "particleEmitter", true);
			excludeFields.Add("UnityEngine.Component", "particleSystem", true);
			excludeFields.Add("UnityEngine.Component", "networkView", true);
			excludeFields.Add("UnityEngine.Component", "guiText", true);
			excludeFields.Add("UnityEngine.Component", "guiElement", true);
			excludeFields.Add("UnityEngine.Component", "audio", true);
			excludeFields.Add("UnityEngine.Component", "light", true);
*/
        }

        public void ExcludeType(params Type[] types)
        {
            if (fieldMap.Count > 0)
            {
                Assert.Fail(null, "Already binded");
            }
            foreach (Type t in types)
            {
                excludeTypes.Add(t);
            }
        }

        public void ExcludeField(string typeName, string fieldName)
        {
            if (fieldMap.Count > 0)
            {
                Assert.Fail(null, "Already binded");
            }
            excludeMembers.Add(typeName, fieldName, true);
        }

        public List<FieldInfo> GetFields(Type type)
        {
            if (!fieldMap.ContainsKey(type))
            {
                Bind(type);
            }
            return fieldMap[type];
        }

        public List<PropertyInfo> GetProperties(Type type)
        {
            if (!propMap.ContainsKey(type))
            {
                Bind(type);
            }
            return propMap[type];
        }

        public void BeginSearch()
        {
            fieldTraveled.Clear();
            propertyTraveled.Clear();
        }

        private void Bind(Type clsType)
        {
            foreach (FieldInfo f in clsType.GetFields(flags))
            {
                if (excludeTypes.Contains(f.DeclaringType) || excludeMembers.Get(f.DeclaringType.FullName, f.Name))
                {
                    continue;
                }
                // skip deprecated
                bool exclude = false;
                foreach (object a in f.GetCustomAttributes(true))
                {
                    if (a is ObsoleteAttribute)
                    {
                        exclude = true;
                        break;
                    }
                }
                if (exclude)
                {
                    continue;
                }
                if (filter(f)&&!f.IsLiteral)
                {
                    fieldMap.Add(clsType, f);
                }
            }

            foreach (PropertyInfo p in clsType.GetProperties(flags))
            {
                if (!p.CanRead)
                {
                    continue;
                }
                if (excludeTypes.Contains(p.DeclaringType) || excludeMembers.Get(p.DeclaringType.FullName, p.Name))
                {
                    continue;
                }

                // Renderer.material in EditorMode is skipped because of memory leak
                if (!Application.isPlaying&&p.Name == "material" && p.DeclaringType.IsAssignableFrom(typeof(Renderer)))
                {
                    continue;
                }
                if (!Application.isPlaying&&p.Name == "mesh" && p.DeclaringType.IsAssignableFrom(typeof(MeshFilter)))
                {
                    continue;
                }

                // skip deprecated
                bool exclude = false;
                foreach (object a in p.GetCustomAttributes(true))
                {
                    if (a is ObsoleteAttribute)
                    {
                        exclude = true;
                        break;
                    }
                }
                if (exclude)
                {
                    continue;
                }
                if (filter(p))
                {
                    propMap.Add(clsType, p);
                }
            }
        }

        /**
		 * @return changed members/properties
		 */
        public bool HasValue(object obj, object val)
        {
            return GetFieldForValue(obj, val) != null||GetPropertyForValue(obj, val) != null;
        }

        /// <summary>
        /// Gets the first FieldInfo containing val.
        /// For array, only 1D array is supported.
        /// </summary>
        /// <returns>The field for value.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="val">Value.</param>
        public FieldInfo GetFieldForValue(object obj, object val)
        {
            if (val == null)
            {
                return null;
            }
            int hash = obj.GetHashCode();
            if (fieldTraveled.Contains(hash))
            {
                return null;
            }
            fieldTraveled.Add(hash);

            IEnumerable<FieldInfo> fieldMatch = GetFields(obj.GetType());
            if (fieldMatch != null)
            {
                foreach (FieldInfo f in fieldMatch)
                {
                    object fval = null;
					try
					{
						fval = f.GetValue(obj);
					} catch {
					}
                    if (GameObjectEquals(fval, val))
                    {
                        return f;
                    } else if (typeof(IEnumerable).IsAssignableFrom(f.FieldType))
                    {
                        IEnumerable collection = fval as IEnumerable;
                        if (!EqualityComparer<IEnumerable>.Default.Equals(collection, default(IEnumerable)))
                        {
                            foreach (object e in collection)
                            {
                                if (GameObjectEquals(e, val))
                                {
                                    return f;
                                } else if (e != null) {
                                    FieldInfo recurse = GetFieldForValue(e, val);
                                    if (recurse != null)
                                    {
                                        return recurse;
                                    }
                                }
                            }
                        }
                    } else if (fval != null && ReflectionUtil.GetAttribute<SerializableAttribute>(fval.GetType()) != null)
                    {
                        return GetFieldForValue(fval, val);
                    }
                }
            }
            return null;
        }

        private bool GameObjectEquals(object v1, object v2)
        {
            if (v1 == v2)
            {
                return true;
            }
            GameObject o1 = null;
            GameObject o2 = null;
            if (v1 is Component)
            {
                Component c = v1 as Component;
                if (c != null)
                {
                    o1 = c.gameObject;
                }
            } else if (v1 is GameObject)
            {
                o1 = v1 as GameObject;
            }
            if (v2 is Component)
            {
                Component c = v2 as Component;
                if (c != null)
                {
                    o2 = c.gameObject;
                }
            } else if (v2 is GameObject)
            {
                o2 = v2 as GameObject;
            }
            return o1 == o2;
        }

        /// <summary>
        /// Gets the first PropertyInfo containing val.
        /// For array, only 1D array is supported.
        /// </summary>
        /// <returns>The property field for value.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="val">Value.</param>
        public PropertyInfo GetPropertyForValue(object obj, object val)
        {
            if (val == null)
            {
                return null;
            }
            int hash = obj.GetHashCode();
            if (propertyTraveled.Contains(hash))
            {
                return null;
            }
            propertyTraveled.Add(hash);

            IEnumerable<PropertyInfo> propMatch = GetProperties(obj.GetType());
            if (propMatch != null)
            {
                foreach (PropertyInfo p in propMatch)
                {
                    // Renderer.material in EditorMode is skipped because of memory leak
                    if (!Application.isPlaying&&p.Name == "material"&&p.DeclaringType.IsAssignableFrom(typeof(Renderer)))
                    {
                        continue;
                    }
                    if (!Application.isPlaying&&p.Name == "mesh"&&p.DeclaringType.IsAssignableFrom(typeof(MeshFilter)))
                    {
                        continue;
                    }
                    MethodInfo set = p.GetSetMethod(true);
                    MethodInfo get = p.GetGetMethod(true);
                    if (get != null && set != null)
                    {
                        ParameterInfo[] param = get.GetParameters();
                        if (param.IsEmpty())
                        {
							object oval = null;
							try
							{
								oval = p.GetValue(obj, null);
							} catch {
							}
                            if (oval == null)
                            {
                                return null;
                            }
                            if (oval == val)
                            {
                                return p;
                            } else if (!(oval is string) && !oval.GetType().IsPrimitive && ReflectionUtil.GetAttribute<SerializableAttribute>(oval.GetType()) != null)
                            {
                                return GetPropertyForValue(oval, val);
                            }
                        }
                    }
                }
            }
            return null;
        }


        public static bool RefFilter(MemberInfo m)
        {
            if (m is FieldInfo)
            {
                FieldInfo f = m as FieldInfo;
                if (f.FieldType == typeof(String))
                {
                    return false;
                }
                return f.FieldType.IsClass;
            } else if (m is PropertyInfo)
            {
                PropertyInfo p = m as PropertyInfo;
                if (p.PropertyType == typeof(String))
                {
                    return false;
                }
                return p.PropertyType.IsClass;
            }
            return false;
        }
    }
	
}

