using System;
using UnityEngine;

namespace comunity
{
    public interface IReleasablePool
    {
        Transform trans { get; }
        void Add(IReleasable r);
        void Remove(IReleasable r);
        void Release();
    }
}
