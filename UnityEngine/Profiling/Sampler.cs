namespace UnityEngine.Profiling
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Provides control over a CPU Profiler label.</para>
    /// </summary>
    [UsedByNativeCode]
    public class Sampler
    {
        internal IntPtr m_Ptr;
        internal static Sampler s_InvalidSampler = new Sampler();

        internal Sampler()
        {
        }

        /// <summary>
        /// <para>Returns Sampler object for the specific CPU Profiler label.</para>
        /// </summary>
        /// <param name="name">Profiler Sampler name.</param>
        /// <returns>
        /// <para>Sampler object which represents specific profiler label.</para>
        /// </returns>
        public static Sampler Get(string name)
        {
            Sampler sampler2;
            Sampler samplerInternal = GetSamplerInternal(name);
            if (samplerInternal != null)
            {
                sampler2 = samplerInternal;
            }
            else
            {
                sampler2 = s_InvalidSampler;
            }
            return sampler2;
        }

        public static int GetNames(List<string> names) => 
            GetSamplerNamesInternal(names);

        /// <summary>
        /// <para>Returns Recorder associated with the Sampler.</para>
        /// </summary>
        /// <returns>
        /// <para>Recorder object associated with the Sampler.</para>
        /// </returns>
        public Recorder GetRecorder()
        {
            Recorder recorder2;
            Recorder recorderInternal = this.GetRecorderInternal();
            if (recorderInternal != null)
            {
                recorder2 = recorderInternal;
            }
            else
            {
                recorder2 = Recorder.s_InvalidRecorder;
            }
            return recorder2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Recorder GetRecorderInternal();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern Sampler GetSamplerInternal(string name);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int GetSamplerNamesInternal(object namesScriptingPtr);

        /// <summary>
        /// <para>Returns true if Sampler is valid. (Read Only)</para>
        /// </summary>
        public bool isValid =>
            (this.m_Ptr != IntPtr.Zero);

        /// <summary>
        /// <para>Sampler name. (Read Only)</para>
        /// </summary>
        [ThreadAndSerializationSafe]
        public string name { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

