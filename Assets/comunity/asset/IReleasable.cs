using UnityEngine;

namespace comunity
{
    public interface IReleasable
    {
        Transform transform { get; }
        void Release();
    }
}
