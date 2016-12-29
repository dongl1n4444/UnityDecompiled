namespace UnityEngine.Experimental.Director
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Generic playable used to blend ScriptPlayable.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct GenericMixerPlayable
    {
        internal Playable handle;
        internal Playable node =>
            this.handle;
        /// <summary>
        /// <para>Creates an GenericMixerPlayable.</para>
        /// </summary>
        public static GenericMixerPlayable Create()
        {
            GenericMixerPlayable playable = new GenericMixerPlayable();
            InternalCreate(ref playable);
            return playable;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalCreate(ref GenericMixerPlayable playable);
        /// <summary>
        /// <para>Call this method to release the resources associated to this Playable.</para>
        /// </summary>
        public void Destroy()
        {
            this.handle.Destroy();
        }

        public T CastTo<T>() where T: struct => 
            this.handle.CastTo<T>();

        public static implicit operator Playable(GenericMixerPlayable s) => 
            new Playable { 
                m_Handle = s.handle.m_Handle,
                m_Version = s.handle.m_Version
            };
    }
}

