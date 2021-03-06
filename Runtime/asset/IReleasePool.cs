﻿using System;
using UnityEngine;

namespace mulova.comunity
{
    public interface IReleasablePool
    {
        Transform transform { get; }
        void Add(IReleasable r);
        void Remove(IReleasable r);
        void Release();
    }
}
