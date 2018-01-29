using UnityEngine;
using System.Collections;

namespace comunity
{
    public abstract class SceneTransit : Script
    {
        public abstract void StartTransit(object data);
        public abstract void EndTransit();

        public abstract bool inProgress { get; }
    }
}