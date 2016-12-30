namespace UnityEngine.Profiling
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// <para>Controls the from script.</para>
    /// </summary>
    [MovedFrom("UnityEngine")]
    public sealed class Profiler
    {
        private Profiler()
        {
        }

        /// <summary>
        /// <para>Displays the recorded profiledata in the profiler.</para>
        /// </summary>
        /// <param name="file"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, Conditional("UNITY_EDITOR")]
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
        [MethodImpl(MethodImplOptions.InternalCall), Conditional("ENABLE_PROFILER"), GeneratedByOldBindingsGenerator]
        public static extern void BeginSample(string name, UnityEngine.Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void BeginSampleOnly(string name);
        /// <summary>
        /// <para>End profiling a piece of code with a custom label.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator, Conditional("ENABLE_PROFILER")]
        public static extern void EndSample();
        /// <summary>
        /// <para>Returns the size of the mono heap.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetMonoHeapSize();
        /// <summary>
        /// <para>Returns the used size from mono.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetMonoUsedSize();
        /// <summary>
        /// <para>Returns the runtime memory usage of the resource.</para>
        /// </summary>
        /// <param name="o"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetRuntimeMemorySize(UnityEngine.Object o);
        /// <summary>
        /// <para>Returns the size of the temp allocator.</para>
        /// </summary>
        /// <returns>
        /// <para>Size in bytes.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetTempAllocatorSize();
        /// <summary>
        /// <para>Returns the amount of allocated and used system memory.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetTotalAllocatedMemory();
        /// <summary>
        /// <para>Returns the amount of reserved system memory.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetTotalReservedMemory();
        /// <summary>
        /// <para>Returns the amount of reserved but not used system memory.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetTotalUnusedReservedMemory();
        /// <summary>
        /// <para>Sets the size of the temp allocator.</para>
        /// </summary>
        /// <param name="size">Size in bytes.</param>
        /// <returns>
        /// <para>Returns true if requested size was successfully set. Will return false if value is disallowed (too small).</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool SetTempAllocatorRequestedSize(uint size);

        /// <summary>
        /// <para>Sets profiler output file in built players.</para>
        /// </summary>
        public static bool enableBinaryLog { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Enables the Profiler.</para>
        /// </summary>
        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Sets profiler output file in built players.</para>
        /// </summary>
        public static string logFile { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Resize the profiler sample buffers to allow the desired amount of samples per thread.</para>
        /// </summary>
        [Obsolete("maxNumberOfSamplesPerFrame is no longer needed, as the profiler buffers auto expand as needed")]
        public static int maxNumberOfSamplesPerFrame { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static bool supported { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Heap size used by the program.</para>
        /// </summary>
        /// <returns>
        /// <para>Size of the used heap in bytes, (or 0 if the profiler is disabled).</para>
        /// </returns>
        public static uint usedHeapSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

