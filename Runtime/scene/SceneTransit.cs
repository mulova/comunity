using UnityEngine;
using System.Collections;

namespace mulova.comunity
{
    public abstract class SceneTransit : InternalScript
    {
        public abstract void StartTransit(object data);
        public abstract void EndTransit();

        public abstract bool inProgress { get; }
    }
}