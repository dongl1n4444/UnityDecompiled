namespace Unity.IL2CPP
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct InvokerData : IEquatable<InvokerData>
    {
        public readonly bool VoidReturn;
        public readonly int ParameterCount;
        public InvokerData(bool voidReturn, int parameterCount)
        {
            this.VoidReturn = voidReturn;
            this.ParameterCount = parameterCount;
        }

        public override int GetHashCode() => 
            (this.VoidReturn.GetHashCode() ^ this.ParameterCount.GetHashCode());

        public bool Equals(InvokerData other) => 
            ((this.VoidReturn == other.VoidReturn) && (this.ParameterCount == other.ParameterCount));
    }
}

