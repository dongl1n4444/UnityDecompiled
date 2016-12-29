namespace UnityEngine.VR.WSA.Input
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Represents one detected instance of a hand, controller, or user's voice that can cause interactions and gestures.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct InteractionSource
    {
        internal uint m_id;
        internal InteractionSourceKind m_kind;
        /// <summary>
        /// <para>The identifier for the hand, controller, or user's voice.</para>
        /// </summary>
        public uint id =>
            this.m_id;
        /// <summary>
        /// <para>The kind of the interaction source.</para>
        /// </summary>
        public InteractionSourceKind kind =>
            this.m_kind;
    }
}

