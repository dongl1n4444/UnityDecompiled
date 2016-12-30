namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShaderBufferInfo
    {
        public string name;
        public int flags;
    }
}

