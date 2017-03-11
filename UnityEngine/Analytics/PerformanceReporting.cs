namespace UnityEngine.Analytics
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Unity Performace provides insight into your game performace.</para>
    /// </summary>
    public static class PerformanceReporting
    {
        /// <summary>
        /// <para>Controls whether the Performance Reporting service is enabled at runtime.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

