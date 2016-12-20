namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The NetworkPlayer is a data structure with which you can locate another player over the network.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct NetworkPlayer
    {
        internal int index;
        public NetworkPlayer(string ip, int port)
        {
            Debug.LogError("Not yet implemented");
            this.index = 0;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string Internal_GetIPAddress(int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int Internal_GetPort(int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string Internal_GetExternalIP();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int Internal_GetExternalPort();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string Internal_GetLocalIP();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int Internal_GetLocalPort();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern int Internal_GetPlayerIndex();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string Internal_GetGUID(int index);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern string Internal_GetLocalGUID();
        public static bool operator ==(NetworkPlayer lhs, NetworkPlayer rhs)
        {
            return (lhs.index == rhs.index);
        }

        public static bool operator !=(NetworkPlayer lhs, NetworkPlayer rhs)
        {
            return (lhs.index != rhs.index);
        }

        public override int GetHashCode()
        {
            return this.index.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is NetworkPlayer))
            {
                return false;
            }
            NetworkPlayer player = (NetworkPlayer) other;
            return (player.index == this.index);
        }

        /// <summary>
        /// <para>The IP address of this player.</para>
        /// </summary>
        public string ipAddress
        {
            get
            {
                if (this.index == Internal_GetPlayerIndex())
                {
                    return Internal_GetLocalIP();
                }
                return Internal_GetIPAddress(this.index);
            }
        }
        /// <summary>
        /// <para>The port of this player.</para>
        /// </summary>
        public int port
        {
            get
            {
                if (this.index == Internal_GetPlayerIndex())
                {
                    return Internal_GetLocalPort();
                }
                return Internal_GetPort(this.index);
            }
        }
        /// <summary>
        /// <para>The GUID for this player, used when connecting with NAT punchthrough.</para>
        /// </summary>
        public string guid
        {
            get
            {
                if (this.index == Internal_GetPlayerIndex())
                {
                    return Internal_GetLocalGUID();
                }
                return Internal_GetGUID(this.index);
            }
        }
        /// <summary>
        /// <para>Returns the index number for this network player.</para>
        /// </summary>
        public override string ToString()
        {
            return this.index.ToString();
        }

        /// <summary>
        /// <para>Returns the external IP address of the network interface.</para>
        /// </summary>
        public string externalIP
        {
            get
            {
                return Internal_GetExternalIP();
            }
        }
        /// <summary>
        /// <para>Returns the external port of the network interface.</para>
        /// </summary>
        public int externalPort
        {
            get
            {
                return Internal_GetExternalPort();
            }
        }
        internal static NetworkPlayer unassigned
        {
            get
            {
                NetworkPlayer player;
                player.index = -1;
                return player;
            }
        }
    }
}

