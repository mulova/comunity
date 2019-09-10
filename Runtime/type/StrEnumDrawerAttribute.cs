using UnityEngine;

namespace mulova.comunity
{
    public class StrEnumDrawerAttribute : PropertyAttribute
    {
        public readonly string strVar;
        public readonly string enumVar;

        public StrEnumDrawerAttribute(string strVar, string enumVar)
        {
            this.strVar = strVar;
            this.enumVar = enumVar;
        }
    }
}

