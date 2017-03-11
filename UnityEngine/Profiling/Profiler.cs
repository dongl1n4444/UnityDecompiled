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
        [MethodImpl(MethodImplOptions.InternalCall), Conditional("UNITY_EDITOR"), GeneratedByOldBindingsGenerator]
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
        [MethodImpl(MethodImplOptions.InternalCall), Conditional("ENABLE_PROFILER"), GeneratedByOldBindingsGenerator]
        public static extern void EndSample();
        /// <summary>
        /// <para>Returns the size of the mono heap.</para>
        /// </summary>
        [Obsolete("GetMonoHeapSize has been deprecated since it is limited to 4GB. Please use GetMonoHeapSizeLong() instead.")]
        public static uint GetMonoHeapSize() => 
            ((uint) GetMonoHeapSizeLong());

        /// <summary>
        /// <para>Returns the size of the reserved space for managed-memory.</para>
        /// </summary>
        /// <returns>
        /// <para>The size of the managed heap. This returns 0 if the Profiler is not available.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetMonoHeapSizeLong();
        /// <summary>
        /// <para>Returns the used size from mono.</para>
        /// </summary>
        [Obsolete("GetMonoUsedSize has been deprecated since it is limited to 4GB. Please use GetMonoUsedSizeLong() instead.")]
        public static uint GetMonoUsedSize() => 
            ((uint) GetMonoUsedSizeLong());

        /// <summary>
        /// <para>The allocated managed-memory for live objects and non-collected objects.</para>
        /// </summary>
        /// <returns>
        /// <para>A long integer value of the memory in use. This returns 0 if the Profiler is not available.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetMonoUsedSizeLong();
        /// <summary>
        /// <para>Returns the runtime memory usage of the resource.</para>
        /// </summary>
        /// <param name="o"></param>
        [Obsolete("GetRuntimeMemorySize has been deprecated since it is limited to 2GB. Please use GetRuntimeMemorySizeLong() instead.")]
        public static int GetRuntimeMemorySize(UnityEngine.Object o) => 
            ((int) GetRuntimeMemorySizeLong(o));

        /// <summary>
        /// <para>Gathers the native-memory used by a Unity object.</para>
        /// </summary>
        /// <param name="o">The target Unity object.</param>
        /// <returns>
        /// <para>The amount of native-memory used by a Unity object. This returns 0 if the Profiler is not available.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetRuntimeMemorySizeLong(UnityEngine.Object o);
        /// <summary>
        /// <para>Returns the size of the temp allocator.</para>
        /// </summary>
        /// <returns>
        /// <para>Size in bytes.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern uint GetTempAllocatorSize();
        /// <summary>
        /// <para>Returns the amount of allocated and used system memory.</para>
        /// </summary>
        [Obsolete("GetTotalAllocatedMemory has been deprecated since it is limited to 4GB. Please use GetTotalAllocatedMemoryLong() instead.")]
        public static uint GetTotalAllocatedMemory() => 
            ((uint) GetTotalAllocatedMemoryLong());

        /// <summary>
        /// <para>The total memory allocated by the internal allocators in Unity. Unity reserves large pools of memory from the system. This function returns the amount of used memory in those pools.</para>
        /// </summary>
        /// <returns>
        /// <para>The amount of memory allocated by Unity. This returns 0 if the Profiler is not available.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetTotalAllocatedMemoryLong();
        /// <summary>
        /// <para>Returns the amount of reserved system memory.</para>
        /// </summary>
        [Obsolete("GetTotalReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalReservedMemoryLong() instead.")]
        public static uint GetTotalReservedMemory() => 
            ((uint) GetTotalReservedMemoryLong());

        /// <summary>
        /// <para>The total memory Unity has reserved.</para>
        /// </summary>
        /// <returns>
        /// <para>Memory reserved by Unity in bytes. This returns 0 if the Profiler is not available.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetTotalReservedMemoryLong();
        /// <summary>
        /// <para>Returns the amount of reserved but not used system memory.</para>
        /// </summary>
        [Obsolete("GetTotalUnusedReservedMemory has been deprecated since it is limited to 4GB. Please use GetTotalUnusedReservedMemoryLong() instead.")]
        public static uint GetTotalUnusedReservedMemory() => 
            ((uint) GetTotalUnusedReservedMemoryLong());

        /// <summary>
        /// <para>Unity allocates memory in pools for usage when unity needs to allocate memory. This function returns the amount of unused memory in these pools.</para>
        /// </summary>
        /// <returns>
        /// <para>The amount of unused memory in the reserved pools. This returns 0 if the Profiler is not available.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern long GetTotalUnusedReservedMemoryLong();
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
        [Obsolete("usedHeapSize has been deprecated since it is limited to 4GB. Please use usedHeapSizeLong instead.")]
        public static uint usedHeapSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Returns the number of bytes that Unity has allocated. This does not include bytes allocated by external libraries or drivers.</para>
        /// </summary>
        /// <returns>
        /// <para>Size of the memory allocated by Unity (or 0 if the profiler is disabled).</para>
        /// </returns>
        public static long usedHeapSizeLong { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

