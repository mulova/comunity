using System.Runtime.CompilerServices;
using System;
using System.Threading.Tasks;

public static class AsyncExtensions
{
    public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan)
    {
        return Task.Delay(timeSpan).GetAwaiter();
    }
}
