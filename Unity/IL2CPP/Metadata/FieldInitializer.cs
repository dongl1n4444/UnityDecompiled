namespace Unity.IL2CPP.Metadata
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct FieldInitializer
    {
        public string Name;
        public object Value;
    }
}

