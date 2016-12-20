namespace UnityEngine.Profiling
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// <para>Controls the from script.</para>
    /// </summary>
    [MovedFrom("UnityEngine")]
    public sealed class Profiler
    {
        /// <summary>
        /// <para>Displays the recorded profiledata in the profiler.</para>
        /// </summary>
        /// <param name="file"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Conditional("UNITY_EDITOR")]
        public static extern void AddFramesFromFile(string file);
        /// <summary>
        /// <para>Begin profiling a piece of code with a custom label.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="targetObject"></param>
        [Conditional("ENABLE_PROFILER")]
        public static void BeginSample(string name)
        {
            BeginSampleOnly(name);
        }

        /// <summary>
        /// <para>Begin profiling a piece of code with a custom label.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="targetObject"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Conditional("ENABLE_PROFILER")]
        public static extern void BeginSample(string name, UnityEngine.Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void BeginSampleOnly(string name);
        /// <summary>
        /// <para>End profiling a piece of code with a custom label.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), Conditional("ENABLE_PROFILER")]
        public static extern void EndSample();
        /// <summary>
        /// <para>Returns the size of the mono heap.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern uint GetMonoHeapSize();
        /// <summary>
        /// <para>Returns the used size from mono.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern uint GetMonoUsedSize();
        /// <summary>
        /// <para>Returns the runtime memory usage of the resource.</para>
        /// </summary>
        /// <param name="o"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetRuntimeMemorySize(UnityEngine.Object o);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Sampler GetSampler(string name);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string[] GetSamplerNames();
        /// <summary>
        /// <para>Returns the size of the temp allocator.</para>
        /// </summary>
        /// <returns>
        /// <para>Size in bytes.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern uint GetTempAllocatorSize();
        /// <summary>
        /// <para>Returns the amount of allocated and used system memory.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern uint GetTotalAllocatedMemory();
        /// <summary>
        /// <para>Returns the amount of reserved system memory.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern uint GetTotalReservedMemory();
        /// <summary>
        /// <para>Returns the amount of reserved but not used system memory.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern uint GetTotalUnusedReservedMemory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetSamplersEnabled(bool enabled);
        /// <summary>
        /// <para>Sets the size of the temp allocator.</para>
        /// </summary>
        /// <param name="size">Size in bytes.</param>
        /// <returns>
        /// <para>Returns true if requested size was successfully set. Will return false if value is disallowed (too small).</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool SetTempAllocatorRequestedSize(uint size);

        /// <summary>
        /// <para>Sets profiler output file in built players.</para>
        /// </summary>
        public static bool enableBinaryLog { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Enables the Profiler.</para>
        /// </summary>
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Sets profiler output file in built players.</para>
        /// </summary>
        public static string logFile { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Resize the profiler sample buffers to allow the desired amount of samples per thread.</para>
        /// </summary>
        [Obsolete("maxNumberOfSamplesPerFrame is no longer needed, as the profiler buffers auto expand as needed")]
        public static int maxNumberOfSamplesPerFrame { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        public static bool supported { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Heap size used by the program.</para>
        /// </summary>
        /// <returns>
        /// <para>Size of the used heap in bytes, (or 0 if the profiler is disabled).</para>
        /// </returns>
        public static uint usedHeapSize { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}

