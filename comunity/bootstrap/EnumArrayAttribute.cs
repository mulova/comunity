using System;
using UnityEngine;

namespace comunity
{
    public class EnumArrayAttribute : PropertyAttribute 
    {
        public Type enumType;

        public EnumArrayAttribute(Type enumType)
        {
            this.enumType = enumType;
        }
    }
}
