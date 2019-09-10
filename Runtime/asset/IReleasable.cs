using UnityEngine;

namespace mulova.comunity
{
    public interface IReleasable
    {
        Transform transform { get; }
        void Release();
    }
}
