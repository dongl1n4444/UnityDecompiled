namespace UnityEngine.CrashReportHandler
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Engine API for CrashReporting Service.</para>
    /// </summary>
    public static class CrashReportHandler
    {
        /// <summary>
        /// <para>This Boolean field will cause CrashReportHandler to capture exceptions when set to true. By default enable capture exceptions is true.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public static bool enableCaptureExceptions { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

