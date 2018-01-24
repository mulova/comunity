using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace core
{
    public static class MemberInfoRegistryEx
    {
        public static bool ObjectRefFilter(MemberInfo m)
        {
            if (m is FieldInfo)
            {
                return SerializableFieldFilter(m as FieldInfo);
            } else if (m is PropertyInfo)
            {
                return SerializablePropertyFilter(m as PropertyInfo);
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Filter for the serializable fields
        /// </summary>
        /// <returns><c>true</c>, if a member is serializable field, <c>false</c> otherwise.</returns>
        /// <param name="m">M.</param>
        public static bool SerializableFieldFilter(FieldInfo f)
        {
            if (f.FieldType == typeof(String))
            {
                return false;
            }
            if (!f.IsPublic&&!Attribute.IsDefined(f, typeof(SerializeField)))
            {
                return false;
            }
            if (Attribute.IsDefined(f, typeof(NonSerializedAttribute)))
            {
                return false;
            }
            if (!Attribute.IsDefined(f, typeof(SerializableAttribute))
                &&!Attribute.IsDefined(f.FieldType, typeof(SerializableAttribute)))
            {
                return false;
            }
            return true;
        }

        public static bool SerializablePropertyFilter(PropertyInfo p)
        {
            if (Attribute.IsDefined(p.PropertyType, typeof(NonSerializedAttribute)))
            {
                return false;
            }
            if (!Attribute.IsDefined(p.PropertyType, typeof(SerializableAttribute)))
            {
                return false;
            }
            return true;
        }
    }
}

