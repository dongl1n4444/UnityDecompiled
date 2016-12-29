namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The NetworkViewID is a unique identifier for a network view instance in a multiplayer game.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct NetworkViewID
    {
        private int a;
        private int b;
        private int c;
        /// <summary>
        /// <para>Represents an invalid network view ID.</para>
        /// </summary>
        public static NetworkViewID unassigned
        {
            get
            {
                NetworkViewID wid;
                INTERNAL_get_unassigned(out wid);
                return wid;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_get_unassigned(out NetworkViewID value);
        internal static bool Internal_IsMine(NetworkViewID value) => 
            INTERNAL_CALL_Internal_IsMine(ref value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_Internal_IsMine(ref NetworkViewID value);
        internal static void Internal_GetOwner(NetworkViewID value, out NetworkPlayer player)
        {
            INTERNAL_CALL_Internal_GetOwner(ref value, out player);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_Internal_GetOwner(ref NetworkViewID value, out NetworkPlayer player);
        internal static string Internal_GetString(NetworkViewID value) => 
            INTERNAL_CALL_Internal_GetString(ref value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string INTERNAL_CALL_Internal_GetString(ref NetworkViewID value);
        internal static bool Internal_Compare(NetworkViewID lhs, NetworkViewID rhs) => 
            INTERNAL_CALL_Internal_Compare(ref lhs, ref rhs);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_Internal_Compare(ref NetworkViewID lhs, ref NetworkViewID rhs);
        public static bool operator ==(NetworkViewID lhs, NetworkViewID rhs) => 
            Internal_Compare(lhs, rhs);

        public static bool operator !=(NetworkViewID lhs, NetworkViewID rhs) => 
            !Internal_Compare(lhs, rhs);

        public override int GetHashCode() => 
            ((this.a ^ this.b) ^ this.c);

        public override bool Equals(object other)
        {
            if (!(other is NetworkViewID))
            {
                return false;
            }
            NetworkViewID rhs = (NetworkViewID) other;
            return Internal_Compare(this, rhs);
        }

        /// <summary>
        /// <para>True if instantiated by me.</para>
        /// </summary>
        public bool isMine =>
            Internal_IsMine(this);
        /// <summary>
        /// <para>The NetworkPlayer who owns the NetworkView. Could be the server.</para>
        /// </summary>
        public NetworkPlayer owner
        {
            get
            {
                NetworkPlayer player;
                Internal_GetOwner(this, out player);
                return player;
            }
        }
        /// <summary>
        /// <para>Returns a formatted string with details on this NetworkViewID.</para>
        /// </summary>
        public override string ToString() => 
            Internal_GetString(this);
    }
}

