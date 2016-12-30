namespace UnityEditor.CrashReporting
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Editor API for the Unity Services editor feature. Normally CrashReporting is enabled from the Services window, but if writing your own editor extension, this API can be used.</para>
    /// </summary>
    public static class CrashReportingSettings
    {
        /// <summary>
        /// <para>This Boolean field will cause the CrashReporting feature in Unity to capture exceptions that occur in the editor while running in Play mode if true, or ignore those errors if false.</para>
        /// </summary>
        public static bool captureEditorExceptions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>This Boolean field will cause the CrashReporting feature in Unity to be enabled if true, or disabled if false.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

