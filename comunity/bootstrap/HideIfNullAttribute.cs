using UnityEngine;

namespace comunity
{
    public class HideIfNullAttribute : PropertyAttribute
    {
        public readonly string nullPropertyName;
        public readonly bool hide;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nullPropertyName"></param>
        /// <param name="hide">If false, just disable </param>
        public HideIfNullAttribute(string nullPropertyName, bool hide = true)
        {
            this.nullPropertyName = nullPropertyName;
            this.hide = hide;
        }
    }
}
