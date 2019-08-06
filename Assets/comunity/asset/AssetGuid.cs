using System.Text.Ex;
using mulova.commons;
using UnityEngine;

namespace comunity
{
    [System.Serializable]
    public class AssetGuid
    {
        [HideInInspector]
        public string guid;

        public bool isValid => guid.IsNotEmpty();

        public static implicit operator string(AssetGuid a)
        {
            return a.guid;
        }
    }
}

