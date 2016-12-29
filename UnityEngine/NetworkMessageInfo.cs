namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>This data structure contains information on a message just received from the network.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct NetworkMessageInfo
    {
        private double m_TimeStamp;
        private NetworkPlayer m_Sender;
        private NetworkViewID m_ViewID;
        /// <summary>
        /// <para>The time stamp when the Message was sent in seconds.</para>
        /// </summary>
        public double timestamp =>
            this.m_TimeStamp;
        /// <summary>
        /// <para>The player who sent this network message (owner).</para>
        /// </summary>
        public NetworkPlayer sender =>
            this.m_Sender;
        /// <summary>
        /// <para>The NetworkView who sent this message.</para>
        /// </summary>
        public NetworkView networkView
        {
            get
            {
                if (this.m_ViewID == NetworkViewID.unassigned)
                {
                    Debug.LogError("No NetworkView is assigned to this NetworkMessageInfo object. Note that this is expected in OnNetworkInstantiate().");
                    return this.NullNetworkView();
                }
                return NetworkView.Find(this.m_ViewID);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern NetworkView NullNetworkView();
    }
}

