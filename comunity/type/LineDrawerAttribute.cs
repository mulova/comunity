using System;
using UnityEngine;

namespace mulova.comunity
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

