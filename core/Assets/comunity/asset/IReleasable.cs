using UnityEngine;

namespace core
{
    public interface IReleasable
    {
        Transform trans { get; }
        void Release();
    }
}
