namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditorInternal;

    /// <summary>
    /// <para>MemorySnapshot is a profiling tool to help diagnose memory usage.</para>
    /// </summary>
    public static class MemorySnapshot
    {
        [field: DebuggerBrowsable(0), CompilerGenerated]
        public static  event Action<PackedMemorySnapshot> OnSnapshotReceived;

        private static void DispatchSnapshot(PackedMemorySnapshot snapshot)
        {
            Action<PackedMemorySnapshot> onSnapshotReceived = OnSnapshotReceived;
            if (onSnapshotReceived != null)
            {
                onSnapshotReceived(snapshot);
            }
        }

        /// <summary>
        /// <para>Requests a new snapshot from the currently connected target of the profiler. Currently only il2cpp-based players are able to provide memory snapshots.</para>
        /// </summary>
        public static void RequestNewSnapshot()
        {
            ProfilerDriver.RequestMemorySnapshot();
        }
    }
}

