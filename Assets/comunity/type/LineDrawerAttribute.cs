using System;
using UnityEngine;

namespace comunity
{
    public class LineDrawerAttribute : PropertyAttribute
    {
        public readonly string[] names;

        public LineDrawerAttribute(params string[] names)
        {
            this.names = names;
        }
    }
}

