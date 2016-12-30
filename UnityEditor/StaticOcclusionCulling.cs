namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>StaticOcclusionCulling lets you perform static occlusion culling operations.</para>
    /// </summary>
    public sealed class StaticOcclusionCulling
    {
        /// <summary>
        /// <para>Used to cancel asynchronous generation of static occlusion culling data.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Cancel();
        /// <summary>
        /// <para>Clears the PVS of the opened scene.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Clear();
        /// <summary>
        /// <para>Used to generate static occlusion culling data.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool Compute();
        /// <summary>
        /// <para>Used to compute static occlusion culling data asynchronously.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern bool GenerateInBackground();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void InvalidatePrevisualisationData();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetDefaultOcclusionBakeSettings();

        public static float backfaceThreshold { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Does the scene contain any occlusion portals that were added manually rather than automatically?</para>
        /// </summary>
        public static bool doesSceneHaveManualPortals { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Used to check if asynchronous generation of static occlusion culling data is still running.</para>
        /// </summary>
        public static bool isRunning { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        public static float smallestHole { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static float smallestOccluder { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns the size in bytes that the PVS data is currently taking up in this scene on disk.</para>
        /// </summary>
        public static int umbraDataSize { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }
    }
}

