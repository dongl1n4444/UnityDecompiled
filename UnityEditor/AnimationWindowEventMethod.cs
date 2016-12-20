namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct AnimationWindowEventMethod
    {
        public string name;
        public Type parameterType;
    }
}

