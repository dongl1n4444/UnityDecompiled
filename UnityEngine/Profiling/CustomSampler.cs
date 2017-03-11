namespace UnityEngine.Profiling
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Custom CPU Profiler label used for profiling arbitrary code blocks.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class CustomSampler : Sampler
    {
        internal static CustomSampler s_InvalidCustomSampler = new CustomSampler();

        internal CustomSampler()
        {
        }

        /// <summary>
        /// <para>Begin profiling a piece of code with a custom label defined by this instance of CustomSampler.</para>
        /// </summary>
        /// <param name="targetObject"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Conditional("ENABLE_PROFILER"), GeneratedByOldBindingsGenerator]
        public extern void Begin();
        /// <summary>
        /// <para>Begin profiling a piece of code with a custom label defined by this instance of CustomSampler.</para>
        /// </summary>
        /// <param name="targetObject"></param>
        [Conditional("ENABLE_PROFILER")]
        public void Begin(UnityEngine.Object targetObject)
        {
            this.BeginWithObject(targetObject);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void BeginWithObject(UnityEngine.Object targetObject);
        /// <summary>
        /// <para>Creates a new CustomSampler for profiling parts of your code.</para>
        /// </summary>
        /// <param name="name">Name of the Sampler.</param>
        /// <returns>
        /// <para>CustomSampler object or null if a built-in Sampler with the same name exists.</para>
        /// </returns>
        public static CustomSampler Create(string name)
        {
            CustomSampler sampler2;
            CustomSampler sampler1 = CreateInternal(name);
            if (sampler1 != null)
            {
                sampler2 = sampler1;
            }
            else
            {
                sampler2 = s_InvalidCustomSampler;
            }
            return sampler2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern CustomSampler CreateInternal(string name);
        /// <summary>
        /// <para>End profiling a piece of code with a custom label.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), Conditional("ENABLE_PROFILER"), GeneratedByOldBindingsGenerator]
        public extern void End();
    }
}

