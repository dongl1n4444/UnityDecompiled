namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Il2CppTypeData
    {
        public readonly int Attrs;
        public readonly TypeReference Type;
        public Il2CppTypeData(TypeReference type, int attrs)
        {
            this.Type = type;
            this.Attrs = attrs;
        }

        public override string ToString() => 
            $"{this.Type.FullName} [{this.Attrs}]";
    }
}

