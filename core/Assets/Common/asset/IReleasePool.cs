using System;
using UnityEngine;

namespace core
{
    public interface IReleasablePool
    {
        Transform trans { get; }
        void Add(IReleasable r);
        void Remove(IReleasable r);
        void Release();
    }
}
