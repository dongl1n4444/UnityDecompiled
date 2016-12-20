namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <para>AndroidInput provides support for off-screen touch input, such as a touchpad.</para>
    /// </summary>
    public sealed class AndroidInput
    {
        private AndroidInput()
        {
        }

        /// <summary>
        /// <para>Returns object representing status of a specific touch on a secondary touchpad (Does not allocate temporary variables).</para>
        /// </summary>
        /// <param name="index"></param>
        public static Touch GetSecondaryTouch(int index)
        {
            Touch touch;
            INTERNAL_CALL_GetSecondaryTouch(index, out touch);
            return touch;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetSecondaryTouch(int index, out Touch value);

        /// <summary>
        /// <para>Property indicating whether the system provides secondary touch input.</para>
        /// </summary>
        public static bool secondaryTouchEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Property indicating the height of the secondary touchpad.</para>
        /// </summary>
        public static int secondaryTouchHeight { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Property indicating the width of the secondary touchpad.</para>
        /// </summary>
        public static int secondaryTouchWidth { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Number of secondary touches. Guaranteed not to change throughout the frame. (Read Only).</para>
        /// </summary>
        public static int touchCountSecondary { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

