namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Menu class to manipulate the menu item.</para>
    /// </summary>
    public sealed class Menu
    {
        /// <summary>
        /// <para>Get the check status of the given menu.</para>
        /// </summary>
        /// <param name="menuPath"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GetChecked(string menuPath);
        /// <summary>
        /// <para>Set the check status of the given menu.</para>
        /// </summary>
        /// <param name="menuPath"></param>
        /// <param name="isChecked"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetChecked(string menuPath, bool isChecked);
    }
}

