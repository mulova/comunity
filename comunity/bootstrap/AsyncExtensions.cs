using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System;
using System.Threading.Tasks;
using System.Threading;

public static class AsyncExtensions
{
    public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
    {
        return Task.Delay(timeSpan).GetAwaiter();
    }
}
