namespace UnityEngine.Tizen
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Interface into Tizen specific functionality.</para>
    /// </summary>
    public sealed class Window
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_evasGL(out IntPtr value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_get_windowHandle(out IntPtr value);

        /// <summary>
        /// <para>Get pointer to the Tizen EvasGL object..</para>
        /// </summary>
        public static IntPtr evasGL
        {
            get
            {
                IntPtr ptr;
                INTERNAL_get_evasGL(out ptr);
                return ptr;
            }
        }

        /// <summary>
        /// <para>Get pointer to the native window handle.</para>
        /// </summary>
        public static IntPtr windowHandle
        {
            get
            {
                IntPtr ptr;
                INTERNAL_get_windowHandle(out ptr);
                return ptr;
            }
        }
    }
}

