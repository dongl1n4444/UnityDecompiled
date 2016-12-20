namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    internal struct UIntDecimal
    {
        [FieldOffset(0)]
        public decimal decimalValue;
        [FieldOffset(0)]
        public ulong longValue1;
        [FieldOffset(8)]
        public ulong longValue2;
    }
}

