namespace UnityEngine.AI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Navigation mesh builder interface.</para>
    /// </summary>
    public static class NavMeshBuilder
    {
        public static NavMeshData BuildNavMeshData(NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds, Vector3 position, Quaternion rotation)
        {
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            NavMeshBuildDebugSettings debug = new NavMeshBuildDebugSettings();
            return BuildNavMeshData(buildSettings, sources, localBounds, position, rotation, debug);
        }

        private static NavMeshData BuildNavMeshData(NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds, Vector3 position, Quaternion rotation, NavMeshBuildDebugSettings debug)
        {
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            NavMeshData data = new NavMeshData(buildSettings.agentTypeID) {
                position = position,
                rotation = rotation
            };
            UpdateNavMeshDataListInternal(data, buildSettings, sources, localBounds, debug);
            return data;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void Cancel(NavMeshData data);
        public static void CollectSources(Bounds includedWorldBounds, int includedLayerMask, NavMeshCollectGeometry geometry, int defaultArea, List<NavMeshBuildMarkup> markups, List<NavMeshBuildSource> results)
        {
            if (markups == null)
            {
                throw new ArgumentNullException("markups");
            }
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            includedWorldBounds.extents = Vector3.Max(includedWorldBounds.extents, (Vector3) (0.001f * Vector3.one));
            NavMeshBuildSource[] collection = CollectSourcesInternal(includedLayerMask, includedWorldBounds, null, true, geometry, defaultArea, markups.ToArray());
            results.Clear();
            results.AddRange(collection);
        }

        public static void CollectSources(Transform root, int includedLayerMask, NavMeshCollectGeometry geometry, int defaultArea, List<NavMeshBuildMarkup> markups, List<NavMeshBuildSource> results)
        {
            if (markups == null)
            {
                throw new ArgumentNullException("markups");
            }
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            Bounds includedWorldBounds = new Bounds();
            NavMeshBuildSource[] collection = CollectSourcesInternal(includedLayerMask, includedWorldBounds, root, false, geometry, defaultArea, markups.ToArray());
            results.Clear();
            results.AddRange(collection);
        }

        private static NavMeshBuildSource[] CollectSourcesInternal(int includedLayerMask, Bounds includedWorldBounds, Transform root, bool useBounds, NavMeshCollectGeometry geometry, int defaultArea, NavMeshBuildMarkup[] markups) => 
            INTERNAL_CALL_CollectSourcesInternal(includedLayerMask, ref includedWorldBounds, root, useBounds, geometry, defaultArea, markups);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern NavMeshBuildSource[] INTERNAL_CALL_CollectSourcesInternal(int includedLayerMask, ref Bounds includedWorldBounds, Transform root, bool useBounds, NavMeshCollectGeometry geometry, int defaultArea, NavMeshBuildMarkup[] markups);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern AsyncOperation INTERNAL_CALL_UpdateNavMeshDataAsyncListInternal(NavMeshData data, ref NavMeshBuildSettings buildSettings, object sources, ref Bounds localBounds, ref NavMeshBuildDebugSettings debug);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_UpdateNavMeshDataListInternal(NavMeshData data, ref NavMeshBuildSettings buildSettings, object sources, ref Bounds localBounds, ref NavMeshBuildDebugSettings debug);
        public static bool UpdateNavMeshData(NavMeshData data, NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            NavMeshBuildDebugSettings debug = new NavMeshBuildDebugSettings();
            return UpdateNavMeshData(data, buildSettings, sources, localBounds, debug);
        }

        private static bool UpdateNavMeshData(NavMeshData data, NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds, NavMeshBuildDebugSettings debug)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            return UpdateNavMeshDataListInternal(data, buildSettings, sources, localBounds, debug);
        }

        public static AsyncOperation UpdateNavMeshDataAsync(NavMeshData data, NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            NavMeshBuildDebugSettings debug = new NavMeshBuildDebugSettings();
            return UpdateNavMeshDataAsync(data, buildSettings, sources, localBounds, debug);
        }

        private static AsyncOperation UpdateNavMeshDataAsync(NavMeshData data, NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds, NavMeshBuildDebugSettings debug)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            return UpdateNavMeshDataAsyncListInternal(data, buildSettings, sources, localBounds, debug);
        }

        private static AsyncOperation UpdateNavMeshDataAsyncListInternal(NavMeshData data, NavMeshBuildSettings buildSettings, object sources, Bounds localBounds, NavMeshBuildDebugSettings debug) => 
            INTERNAL_CALL_UpdateNavMeshDataAsyncListInternal(data, ref buildSettings, sources, ref localBounds, ref debug);

        private static bool UpdateNavMeshDataListInternal(NavMeshData data, NavMeshBuildSettings buildSettings, object sources, Bounds localBounds, NavMeshBuildDebugSettings debug) => 
            INTERNAL_CALL_UpdateNavMeshDataListInternal(data, ref buildSettings, sources, ref localBounds, ref debug);
    }
}

