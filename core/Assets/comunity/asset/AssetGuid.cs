using System;
using UnityEngine;

namespace comunity
{
    [System.Serializable]
    public class AssetGuid
    {
        [HideInInspector]
        public string guid;

        public bool isValid
        {
            get
            {
                return guid.IsNotEmpty();
            }
        }

        public static implicit operator string(AssetGuid a)
        {
            return a.guid;
        }
    }
}

