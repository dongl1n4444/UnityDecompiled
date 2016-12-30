namespace UnityEditor.Analytics
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Normally performance reporting is enabled from the Services window, but if writing your own editor extension, this API can be used.</para>
    /// </summary>
    public static class PerformanceReportingSettings
    {
        /// <summary>
        /// <para>This Boolean field causes the performance reporting feature in Unity to be enabled if true, or disabled if false.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

