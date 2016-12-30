namespace UnityEngine.Profiling
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Records profiling data produced by a specific Sampler.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class Recorder
    {
        internal IntPtr m_Ptr;
        internal static Recorder s_InvalidRecorder = new Recorder();

        internal Recorder()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
        private extern void DisposeNative();
        ~Recorder()
        {
            if (this.m_Ptr != IntPtr.Zero)
            {
                this.DisposeNative();
            }
        }

        /// <summary>
        /// <para>Use this function to get a Recorder for the specific Profiler label.</para>
        /// </summary>
        /// <param name="samplerName">Sampler name.</param>
        /// <returns>
        /// <para>Recorder object for the specified Sampler.</para>
        /// </returns>
        public static Recorder Get(string samplerName) => 
            Sampler.Get(samplerName).GetRecorder();

        /// <summary>
        /// <para>Accumulated time of Begin/End pairs for the previous frame in nanoseconds. (Read Only)</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public long elapsedNanoseconds { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Enables recording.</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns true if Recorder is valid and can collect data. (Read Only)</para>
        /// </summary>
        public bool isValid =>
            (this.m_Ptr != IntPtr.Zero);

        /// <summary>
        /// <para>Number of time Begin/End pairs was called during the previous frame. (Read Only)</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public int sampleBlockCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

