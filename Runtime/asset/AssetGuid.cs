using System.Text.Ex;
using mulova.commons;
using UnityEngine;

namespace mulova.comunity
{
    [System.Serializable]
    public class AssetGuid
    {
        [HideInInspector]
        public string guid;

        public bool isValid => !guid.IsEmpty();

        public static implicit operator string(AssetGuid a)
        {
            return a.guid;
        }
    }
}

