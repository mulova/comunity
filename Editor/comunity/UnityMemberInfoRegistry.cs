using System;
using System.Reflection;
using mulova.commons;
using UnityEngine;

namespace mulova.comunity
{
    /**
	 * Search the member variable's value
	 * Exclude() to exclude specific type's properties/fields
	 */
    public class UnityMemberInfoRegistry : MemberInfoRegistry
    {
        public UnityMemberInfoRegistry()
        {
            this.isFound = GameObjectEquals;
            ExcludeField("UnityEngine.Renderer", "material"); // sharedMaterial is used 
            ExcludeField("UnityEngine.Renderer", "materials"); // sharedMaterial is used 
            ExcludeField("UnityEngine.Material", "color");
            ExcludeField("UnityEngine.Animator", "bodyRotation");
            ExcludeField("UnityEngine.Animator", "bodyPosition");
            ExcludeField("UnityEngine.Animator", "playbackTime");
            ExcludeField("UnityEngine.MeshFilter", "mesh");
/*
            ExcludeField("UnityEngine.Component", "rigidbody");
            ExcludeField("UnityEngine.Component", "rigidbody2D");
            ExcludeField("UnityEngine.Component", "camera");
            ExcludeField("UnityEngine.Component", "collider");
            ExcludeField("UnityEngine.Component", "collider2D");
            ExcludeField("UnityEngine.Component", "animation");
            ExcludeField("UnityEngine.Component", "hingeJoint");
            ExcludeField("UnityEngine.Component", "particleEmitter");
            ExcludeField("UnityEngine.Component", "particleSystem");
            ExcludeField("UnityEngine.Component", "networkView");
            ExcludeField("UnityEngine.Component", "guiText");
            ExcludeField("UnityEngine.Component", "guiElement");
            ExcludeField("UnityEngine.Component", "audio");
            ExcludeField("UnityEngine.Component", "light");
*/
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
            }
            else if (v1 is GameObject)
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
            }
            else if (v2 is GameObject)
            {
                o2 = v2 as GameObject;
            }
            return o1 == o2;
        }

        public static bool ObjectRefFilter(MemberInfo m)
        {
            if (m is FieldInfo)
            {
                return SerializableFieldFilter(m as FieldInfo);
            }
            else if (m is PropertyInfo)
            {
                return SerializablePropertyFilter(m as PropertyInfo);
            }
            else
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
            if (!f.IsPublic && !Attribute.IsDefined(f, typeof(SerializeField)))
            {
                return false;
            }
            if (Attribute.IsDefined(f, typeof(NonSerializedAttribute)))
            {
                return false;
            }
            if (!Attribute.IsDefined(f, typeof(SerializableAttribute))
                && !Attribute.IsDefined(f.FieldType, typeof(SerializableAttribute)))
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

