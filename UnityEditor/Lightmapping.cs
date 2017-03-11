namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    /// <summary>
    /// <para>Allows to control the lightmapping job.</para>
    /// </summary>
    public sealed class Lightmapping
    {
        /// <summary>
        /// <para>Delegate which is called when bake job is completed.</para>
        /// </summary>
        public static OnCompletedFunction completed;

        /// <summary>
        /// <para>Stars a synchronous bake job.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool Bake();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool BakeAllReflectionProbesSnapshots();
        /// <summary>
        /// <para>Starts an asynchronous bake job.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool BakeAsync();
        /// <summary>
        /// <para>Starts a synchronous bake job, but only bakes light probes.</para>
        /// </summary>
        [Obsolete("BakeLightProbesOnly has been deprecated. Use Bake instead (UnityUpgradable) -> Bake()", true)]
        public static bool BakeLightProbesOnly() => 
            false;

        /// <summary>
        /// <para>Starts an asynchronous bake job, but only bakes light probes.</para>
        /// </summary>
        [Obsolete("BakeLightProbesOnlyAsync has been deprecated. Use BakeAsync instead (UnityUpgradable) -> BakeAsync()", true)]
        public static bool BakeLightProbesOnlyAsync() => 
            false;

        /// <summary>
        /// <para>Bakes an array of scenes.</para>
        /// </summary>
        /// <param name="paths">The path of the scenes that should be baked.</param>
        public static void BakeMultipleScenes(string[] paths)
        {
            if (paths.Length != 0)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    for (int j = i + 1; j < paths.Length; j++)
                    {
                        if (paths[i] == paths[j])
                        {
                            throw new Exception("no duplication of scenes is allowed");
                        }
                    }
                }
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    SceneSetup[] sceneManagerSetup = EditorSceneManager.GetSceneManagerSetup();
                    EditorSceneManager.OpenScene(paths[0]);
                    for (int k = 1; k < paths.Length; k++)
                    {
                        EditorSceneManager.OpenScene(paths[k], OpenSceneMode.Additive);
                    }
                    Bake();
                    EditorSceneManager.SaveOpenScenes();
                    EditorSceneManager.RestoreSceneManagerSetup(sceneManagerSetup);
                }
            }
        }

        /// <summary>
        /// <para>Starts a synchronous bake job for the probe.</para>
        /// </summary>
        /// <param name="probe">Target probe.</param>
        /// <param name="path">The location where cubemap will be saved.</param>
        /// <returns>
        /// <para>Returns true if baking was succesful.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool BakeReflectionProbe(ReflectionProbe probe, string path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool BakeReflectionProbeSnapshot(ReflectionProbe probe);
        /// <summary>
        /// <para>Starts a synchronous bake job for the selected objects.</para>
        /// </summary>
        [Obsolete("BakeSelected has been deprecated. Use Bake instead (UnityUpgradable) -> Bake()", true)]
        public static bool BakeSelected() => 
            false;

        /// <summary>
        /// <para>Starts an asynchronous bake job for the selected objects.</para>
        /// </summary>
        [Obsolete("BakeSelectedAsync has been deprecated. Use BakeAsync instead (UnityUpgradable) -> BakeAsync()", true)]
        public static bool BakeSelectedAsync() => 
            false;

        /// <summary>
        /// <para>Cancels the currently running asynchronous bake job.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Cancel();
        /// <summary>
        /// <para>Deletes all lightmap assets and makes all lights behave as if they weren't baked yet.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Clear();
        /// <summary>
        /// <para>Clears the cache used by lightmaps, reflection probes and default reflection.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ClearDiskCache();
        /// <summary>
        /// <para>Remove the lighting data asset used by the current scene.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void ClearLightingDataAsset();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void ClearPrecompSetIsDone();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void GetTerrainGIChunks(Terrain terrain, ref int numChunksX, ref int numChunksY);
        private static void Internal_CallCompletedFunctions()
        {
            if (completed != null)
            {
                completed();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void PrintStateToConsole();
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Tetrahedralize(Vector3[] positions, out int[] outIndices, out Vector3[] outPositions);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void UpdateCachePath();

        /// <summary>
        /// <para>Is baked GI enabled?</para>
        /// </summary>
        public static bool bakedGI { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Boost the albedo.</para>
        /// </summary>
        public static float bounceBoost { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns the current lightmapping build progress or 0 if Lightmapping.isRunning is false.</para>
        /// </summary>
        public static float buildProgress { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static ConcurrentJobsType concurrentJobsType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static string diskCachePath { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static long diskCacheSize { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        internal static bool enlightenForceUpdates { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static bool enlightenForceWhiteAlbedo { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal static UnityEngine.FilterMode filterMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The lightmap baking workflow mode used. Iterative mode is default, but you can switch to on demand mode which bakes only when the user presses the bake button.</para>
        /// </summary>
        public static GIWorkflowMode giWorkflowMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Scale for indirect lighting.</para>
        /// </summary>
        public static float indirectOutputScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Returns true when the bake job is running, false otherwise (Read Only).</para>
        /// </summary>
        public static bool isRunning { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The lighting data asset used by the active scene.</para>
        /// </summary>
        public static LightingDataAsset lightingDataAsset { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("lightmapSnapshot has been deprecated. Use lightingDataAsset instead (UnityUpgradable) -> lightingDataAsset", true)]
        public static LightmapSnapshot lightmapSnapshot
        {
            get => 
                null;
            set
            {
            }
        }

        internal static bool openRLEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Is realtime GI enabled?</para>
        /// </summary>
        public static bool realtimeGI { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        internal enum ConcurrentJobsType
        {
            Min,
            Low,
            High
        }

        /// <summary>
        /// <para>Workflow mode for lightmap baking. Default is Iterative.</para>
        /// </summary>
        public enum GIWorkflowMode
        {
            Iterative,
            OnDemand,
            Legacy
        }

        /// <summary>
        /// <para>Delegate used by Lightmapping.completed callback.</para>
        /// </summary>
        public delegate void OnCompletedFunction();
    }
}

