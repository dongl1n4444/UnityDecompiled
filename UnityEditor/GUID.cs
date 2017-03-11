namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct GUID
    {
        private uint m_Value0;
        private uint m_Value1;
        private uint m_Value2;
        private uint m_Value3;
        public GUID(string hexRepresentation)
        {
            TryParse(hexRepresentation, out this);
        }

        public static bool operator ==(GUID x, GUID y) => 
            ((((x.m_Value0 == y.m_Value0) && (x.m_Value1 == y.m_Value1)) && (x.m_Value2 == y.m_Value2)) && (x.m_Value3 == y.m_Value3));

        public static bool operator !=(GUID x, GUID y) => 
            !(x == y);

        public override bool Equals(object obj)
        {
            GUID guid = (GUID) obj;
            return (guid == this);
        }

        public override int GetHashCode() => 
            this.m_Value0.GetHashCode();

        public bool Empty() => 
            ((((this.m_Value0 == 0) && (this.m_Value1 == 0)) && (this.m_Value2 == 0)) && (this.m_Value3 == 0));

        [Obsolete("Use TryParse instead")]
        public bool ParseExact(string hex) => 
            TryParse(hex, out this);

        public static bool TryParse(string hex, out GUID result)
        {
            HexToGUIDInternal(hex, out result);
            return !result.Empty();
        }

        public static GUID Generate()
        {
            GUID guid;
            GenerateInternal(out guid);
            return guid;
        }

        public override string ToString() => 
            GUIDToHexInternal(ref this);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern string GUIDToHexInternal(ref GUID value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void HexToGUIDInternal(string hex, out GUID result);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void GenerateInternal(out GUID result);
    }
}

