using System;
using System.Runtime.InteropServices;
using UnityEngine.Profiling;

[StructLayout(LayoutKind.Sequential, Size=1)]
internal struct ProfilerBlock : IDisposable
{
    public ProfilerBlock(string name)
    {
        Profiler.BeginSample(name);
    }

    public void Dispose()
    {
        Profiler.EndSample();
    }
}

