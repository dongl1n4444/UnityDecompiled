namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct TransformMaskElement
    {
        public string path;
        public float weight;
    }
}

