using UnityEngine;

namespace comunity
{
    public class IfAttribute : PropertyAttribute
    {
        public readonly string refPropertyPath;
        public readonly object value;
        public readonly IfAction action;

        public IfAttribute(string nullPropertyName, object val, IfAction action)
        {
            this.refPropertyPath = nullPropertyName;
            this.value = val;
            this.action = action;
        }
    }
}
