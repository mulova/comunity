using UnityEngine;

namespace comunity
{
    public interface IReleasable
    {
        Transform trans { get; }
        void Release();
    }
}
