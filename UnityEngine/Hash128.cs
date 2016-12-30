namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Represent the hash value.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Hash128
    {
        private uint m_u32_0;
        private uint m_u32_1;
        private uint m_u32_2;
        private uint m_u32_3;
        /// <summary>
        /// <para>Construct the Hash128.</para>
        /// </summary>
        /// <param name="u32_0"></param>
        /// <param name="u32_1"></param>
        /// <param name="u32_2"></param>
        /// <param name="u32_3"></param>
        public Hash128(uint u32_0, uint u32_1, uint u32_2, uint u32_3)
        {
            this.m_u32_0 = u32_0;
            this.m_u32_1 = u32_1;
            this.m_u32_2 = u32_2;
            this.m_u32_3 = u32_3;
        }

        /// <summary>
        /// <para>Get if the hash value is valid or not. (Read Only)</para>
        /// </summary>
        public bool isValid =>
            ((((this.m_u32_0 != 0) || (this.m_u32_1 != 0)) || (this.m_u32_2 != 0)) || (this.m_u32_3 != 0));
        /// <summary>
        /// <para>Convert Hash128 to string.</para>
        /// </summary>
        public override string ToString() => 
            Internal_Hash128ToString(this.m_u32_0, this.m_u32_1, this.m_u32_2, this.m_u32_3);

        /// <summary>
        /// <para>Convert the input string to Hash128.</para>
        /// </summary>
        /// <param name="hashString"></param>
        public static Hash128 Parse(string hashString)
        {
            Hash128 hash;
            INTERNAL_CALL_Parse(hashString, out hash);
            return hash;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Parse(string hashString, out Hash128 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern string Internal_Hash128ToString(uint d0, uint d1, uint d2, uint d3);
        public override bool Equals(object obj) => 
            ((obj is Hash128) && (this == ((Hash128) obj)));

        public override int GetHashCode() => 
            (((this.m_u32_0.GetHashCode() ^ this.m_u32_1.GetHashCode()) ^ this.m_u32_2.GetHashCode()) ^ this.m_u32_3.GetHashCode());

        public static bool operator ==(Hash128 hash1, Hash128 hash2) => 
            ((((hash1.m_u32_0 == hash2.m_u32_0) && (hash1.m_u32_1 == hash2.m_u32_1)) && (hash1.m_u32_2 == hash2.m_u32_2)) && (hash1.m_u32_3 == hash2.m_u32_3));

        public static bool operator !=(Hash128 hash1, Hash128 hash2) => 
            !(hash1 == hash2);
    }
}

