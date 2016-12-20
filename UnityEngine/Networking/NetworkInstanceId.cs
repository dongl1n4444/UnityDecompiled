namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// <para>This is used to identify networked objects across all participants of a network. It is assigned at runtime by the server when an object is spawned.</para>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct NetworkInstanceId
    {
        [SerializeField]
        private readonly uint m_Value;
        /// <summary>
        /// <para>A static invalid NetworkInstanceId that can be used for comparisons.</para>
        /// </summary>
        public static NetworkInstanceId Invalid;
        internal static NetworkInstanceId Zero;
        public NetworkInstanceId(uint value)
        {
            this.m_Value = value;
        }

        /// <summary>
        /// <para>Returns true if the value of the NetworkInstanceId is zero.</para>
        /// </summary>
        /// <returns>
        /// <para>True if zero.</para>
        /// </returns>
        public bool IsEmpty()
        {
            return (this.m_Value == 0);
        }

        public override int GetHashCode()
        {
            return (int) this.m_Value;
        }

        public override bool Equals(object obj)
        {
            return ((obj is NetworkInstanceId) && (this == ((NetworkInstanceId) obj)));
        }

        public static bool operator ==(NetworkInstanceId c1, NetworkInstanceId c2)
        {
            return (c1.m_Value == c2.m_Value);
        }

        public static bool operator !=(NetworkInstanceId c1, NetworkInstanceId c2)
        {
            return (c1.m_Value != c2.m_Value);
        }

        /// <summary>
        /// <para>Returns a string of "NetID:value".</para>
        /// </summary>
        /// <returns>
        /// <para>String representation of this object.</para>
        /// </returns>
        public override string ToString()
        {
            return this.m_Value.ToString();
        }

        /// <summary>
        /// <para>The internal value of this identifier.</para>
        /// </summary>
        public uint Value
        {
            get
            {
                return this.m_Value;
            }
        }
        static NetworkInstanceId()
        {
            Invalid = new NetworkInstanceId(uint.MaxValue);
            Zero = new NetworkInstanceId(0);
        }
    }
}

