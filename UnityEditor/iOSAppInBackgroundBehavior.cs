namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Application behavior when entering background.</para>
    /// </summary>
    public enum iOSAppInBackgroundBehavior
    {
        /// <summary>
        /// <para>Custom background behavior, see iOSBackgroundMode for specific background modes.</para>
        /// </summary>
        Custom = -1,
        /// <summary>
        /// <para>Application should exit when entering background.</para>
        /// </summary>
        Exit = 1,
        /// <summary>
        /// <para>Application should suspend execution when entering background.</para>
        /// </summary>
        Suspend = 0
    }
}

