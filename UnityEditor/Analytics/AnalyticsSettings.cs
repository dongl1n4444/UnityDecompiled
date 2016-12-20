namespace UnityEditor.Analytics
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Editor API for the Unity Services editor feature. Normally Analytics is enabled from the Services window, but if writing your own editor extension, this API can be used.</para>
    /// </summary>
    public static class AnalyticsSettings
    {
        /// <summary>
        /// <para>This Boolean field will cause the Analytics feature in Unity to be enabled if true, or disabled if false.</para>
        /// </summary>
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Set to true for testing Analytics integration only within the Editor.</para>
        /// </summary>
        public static bool testMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

