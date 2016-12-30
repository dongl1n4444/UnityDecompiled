namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Ping any given IP address (given in dot notation).</para>
    /// </summary>
    public sealed class Ping
    {
        internal IntPtr m_Ptr;

        /// <summary>
        /// <para>Perform a ping to the supplied target IP address.</para>
        /// </summary>
        /// <param name="address"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern Ping(string address);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        public extern void DestroyPing();
        ~Ping()
        {
            this.DestroyPing();
        }

        /// <summary>
        /// <para>The IP target of the ping.</para>
        /// </summary>
        public string ip { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Has the ping function completed?</para>
        /// </summary>
        public bool isDone { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>This property contains the ping time result after isDone returns true.</para>
        /// </summary>
        public int time { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

