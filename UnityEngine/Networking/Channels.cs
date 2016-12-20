namespace UnityEngine.Networking
{
    using System;

    /// <summary>
    /// <para>Class containing constants for default network channels.</para>
    /// </summary>
    public class Channels
    {
        /// <summary>
        /// <para>The id of the default reliable channel used by the UNet HLAPI, This channel is used for state updates and spawning.</para>
        /// </summary>
        public const int DefaultReliable = 0;
        /// <summary>
        /// <para>The id of the default unreliable channel used for the UNet HLAPI. This channel is used for movement updates.</para>
        /// </summary>
        public const int DefaultUnreliable = 1;
    }
}

