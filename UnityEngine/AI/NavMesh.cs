namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// <para>Singleton class to access the baked NavMesh.</para>
    /// </summary>
    [MovedFrom("UnityEngine")]
    public static class NavMesh
    {
        /// <summary>
        /// <para>Area mask constant that includes all NavMesh areas.</para>
        /// </summary>
        public const int AllAreas = -1;
        /// <summary>
        /// <para>Set a function to be called before the NavMesh is updated during the frame update execution.</para>
        /// </summary>
        public static OnNavMeshPreUpdate onPreUpdate;

        /// <summary>
        /// <para>Adds a link to the NavMesh. The link is described by the NavMeshLinkData struct.</para>
        /// </summary>
        /// <param name="link">Describing the properties of the link.</param>
        /// <returns>
        /// <para>Representing the added link.</para>
        /// </returns>
        public static NavMeshLinkInstance AddLink(NavMeshLinkData link) => 
            new NavMeshLinkInstance { id = AddLinkInternal(link, Vector3.zero, Quaternion.identity) };

        /// <summary>
        /// <para>Adds a link to the NavMesh. The link is described by the NavMeshLinkData struct.</para>
        /// </summary>
        /// <param name="link">Describing the properties of the link.</param>
        /// <param name="position">Translate the link to this position.</param>
        /// <param name="rotation">Rotate the link to this orientation.</param>
        /// <returns>
        /// <para>Representing the added link.</para>
        /// </returns>
        public static NavMeshLinkInstance AddLink(NavMeshLinkData link, Vector3 position, Quaternion rotation) => 
            new NavMeshLinkInstance { id = AddLinkInternal(link, position, rotation) };

        internal static int AddLinkInternal(NavMeshLinkData link, Vector3 position, Quaternion rotation) => 
            INTERNAL_CALL_AddLinkInternal(ref link, ref position, ref rotation);

        /// <summary>
        /// <para>Adds the specified NavMeshData to the game.</para>
        /// </summary>
        /// <param name="navMeshData">Contains the data for the navmesh.</param>
        /// <returns>
        /// <para>Representing the added navmesh.</para>
        /// </returns>
        public static NavMeshDataInstance AddNavMeshData(NavMeshData navMeshData)
        {
            if (navMeshData == null)
            {
                throw new ArgumentNullException("navMeshData");
            }
            return new NavMeshDataInstance { id = AddNavMeshDataInternal(navMeshData) };
        }

        /// <summary>
        /// <para>Adds the specified NavMeshData to the game.</para>
        /// </summary>
        /// <param name="navMeshData">Contains the data for the navmesh.</param>
        /// <param name="position">Translate the navmesh to this position.</param>
        /// <param name="rotation">Rotate the navmesh to this orientation.</param>
        /// <returns>
        /// <para>Representing the added navmesh.</para>
        /// </returns>
        public static NavMeshDataInstance AddNavMeshData(NavMeshData navMeshData, Vector3 position, Quaternion rotation)
        {
            if (navMeshData == null)
            {
                throw new ArgumentNullException("navMeshData");
            }
            return new NavMeshDataInstance { id = AddNavMeshDataTransformedInternal(navMeshData, position, rotation) };
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int AddNavMeshDataInternal(NavMeshData navMeshData);
        internal static int AddNavMeshDataTransformedInternal(NavMeshData navMeshData, Vector3 position, Quaternion rotation) => 
            INTERNAL_CALL_AddNavMeshDataTransformedInternal(navMeshData, ref position, ref rotation);

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("AddOffMeshLinks has no effect and is deprecated."), GeneratedByOldBindingsGenerator]
        public static extern void AddOffMeshLinks();
        /// <summary>
        /// <para>Calculate a path between two points and store the resulting path.</para>
        /// </summary>
        /// <param name="sourcePosition">The initial position of the path requested.</param>
        /// <param name="targetPosition">The final position of the path requested.</param>
        /// <param name="areaMask">A bitfield mask specifying which NavMesh areas can be passed when calculating a path.</param>
        /// <param name="path">The resulting path.</param>
        /// <returns>
        /// <para>True if a either a complete or partial path is found and false otherwise.</para>
        /// </returns>
        public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path)
        {
            path.ClearCorners();
            return CalculatePathInternal(sourcePosition, targetPosition, areaMask, path);
        }

        /// <summary>
        /// <para>Calculates a path between two positions mapped to the NavMesh, subject to the constraints and costs defined by the filter argument.</para>
        /// </summary>
        /// <param name="sourcePosition">The initial position of the path requested.</param>
        /// <param name="targetPosition">The final position of the path requested.</param>
        /// <param name="filter">A filter specifying the cost of NavMesh areas that can be passed when calculating a path.</param>
        /// <param name="path">The resulting path.</param>
        /// <returns>
        /// <para>True if a either a complete or partial path is found and false otherwise.</para>
        /// </returns>
        public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, NavMeshQueryFilter filter, NavMeshPath path)
        {
            path.ClearCorners();
            return CalculatePathFilterInternal(sourcePosition, targetPosition, path, filter.agentTypeID, filter.areaMask, filter.costs);
        }

        internal static bool CalculatePathFilterInternal(Vector3 sourcePosition, Vector3 targetPosition, NavMeshPath path, int type, int mask, float[] costs) => 
            INTERNAL_CALL_CalculatePathFilterInternal(ref sourcePosition, ref targetPosition, path, type, mask, costs);

        internal static bool CalculatePathInternal(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path) => 
            INTERNAL_CALL_CalculatePathInternal(ref sourcePosition, ref targetPosition, areaMask, path);

        /// <summary>
        /// <para>Calculates triangulation of the current navmesh.</para>
        /// </summary>
        public static NavMeshTriangulation CalculateTriangulation() => 
            ((NavMeshTriangulation) TriangulateInternal());

        /// <summary>
        /// <para>Creates and returns a new entry of NavMesh build settings available for runtime NavMesh building.</para>
        /// </summary>
        /// <returns>
        /// <para>The created settings.</para>
        /// </returns>
        public static NavMeshBuildSettings CreateSettings()
        {
            NavMeshBuildSettings settings;
            INTERNAL_CALL_CreateSettings(out settings);
            return settings;
        }

        public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, int areaMask) => 
            INTERNAL_CALL_FindClosestEdge(ref sourcePosition, out hit, areaMask);

        public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, NavMeshQueryFilter filter) => 
            FindClosestEdgeFilter(sourcePosition, out hit, filter.agentTypeID, filter.areaMask);

        private static bool FindClosestEdgeFilter(Vector3 sourcePosition, out NavMeshHit hit, int type, int mask) => 
            INTERNAL_CALL_FindClosestEdgeFilter(ref sourcePosition, out hit, type, mask);

        /// <summary>
        /// <para>Gets the cost for path finding over geometry of the area type.</para>
        /// </summary>
        /// <param name="areaIndex">Index of the area to get.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern float GetAreaCost(int areaIndex);
        /// <summary>
        /// <para>Returns the area index for a named NavMesh area type.</para>
        /// </summary>
        /// <param name="areaName">Name of the area to look up.</param>
        /// <returns>
        /// <para>Index if the specified are, or -1 if no area found.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetAreaFromName(string areaName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern float GetAvoidancePredictionTime();
        /// <summary>
        /// <para>Gets the cost for traversing over geometry of the layer type on all agents.</para>
        /// </summary>
        /// <param name="layer"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use GetAreaCost instead."), GeneratedByOldBindingsGenerator]
        public static extern float GetLayerCost(int layer);
        /// <summary>
        /// <para>Returns the layer index for a named layer.</para>
        /// </summary>
        /// <param name="layerName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use GetAreaFromName instead."), GeneratedByOldBindingsGenerator]
        public static extern int GetNavMeshLayerFromName(string layerName);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern int GetPathfindingIterationsPerFrame();
        /// <summary>
        /// <para>Returns an existing entry of NavMesh build settings.</para>
        /// </summary>
        /// <param name="agentTypeID">The ID to look for.</param>
        /// <returns>
        /// <para>The settings found.</para>
        /// </returns>
        public static NavMeshBuildSettings GetSettingsByID(int agentTypeID)
        {
            NavMeshBuildSettings settings;
            INTERNAL_CALL_GetSettingsByID(agentTypeID, out settings);
            return settings;
        }

        /// <summary>
        /// <para>Returns an existing entry of NavMesh build settings by its ordered index.</para>
        /// </summary>
        /// <param name="index">The index to retrieve from.</param>
        /// <returns>
        /// <para>The found settings.</para>
        /// </returns>
        public static NavMeshBuildSettings GetSettingsByIndex(int index)
        {
            NavMeshBuildSettings settings;
            INTERNAL_CALL_GetSettingsByIndex(index, out settings);
            return settings;
        }

        /// <summary>
        /// <para>Returns the number of registered NavMesh build settings.</para>
        /// </summary>
        /// <returns>
        /// <para>The number of registered entries.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern int GetSettingsCount();
        /// <summary>
        /// <para>Returns the name associated with the NavMesh build settings matching the provided agent type ID.</para>
        /// </summary>
        /// <param name="agentTypeID">The ID to look for.</param>
        /// <returns>
        /// <para>The name associated with the ID found.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern string GetSettingsNameFromID(int agentTypeID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_AddLinkInternal(ref NavMeshLinkData link, ref Vector3 position, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_AddNavMeshDataTransformedInternal(NavMeshData navMeshData, ref Vector3 position, ref Quaternion rotation);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_CalculatePathFilterInternal(ref Vector3 sourcePosition, ref Vector3 targetPosition, NavMeshPath path, int type, int mask, float[] costs);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_CalculatePathInternal(ref Vector3 sourcePosition, ref Vector3 targetPosition, int areaMask, NavMeshPath path);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_CreateSettings(out NavMeshBuildSettings value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_FindClosestEdge(ref Vector3 sourcePosition, out NavMeshHit hit, int areaMask);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_FindClosestEdgeFilter(ref Vector3 sourcePosition, out NavMeshHit hit, int type, int mask);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSettingsByID(int agentTypeID, out NavMeshBuildSettings value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetSettingsByIndex(int index, out NavMeshBuildSettings value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_Raycast(ref Vector3 sourcePosition, ref Vector3 targetPosition, out NavMeshHit hit, int areaMask);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_RaycastFilter(ref Vector3 sourcePosition, ref Vector3 targetPosition, out NavMeshHit hit, int type, int mask);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_SamplePosition(ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_SamplePositionFilter(ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int type, int mask);
        [RequiredByNativeCode]
        private static void Internal_CallOnNavMeshPreUpdate()
        {
            if (onPreUpdate != null)
            {
                onPreUpdate();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern UnityEngine.Object InternalGetLinkOwner(int linkID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern UnityEngine.Object InternalGetOwner(int dataID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool InternalSetLinkOwner(int linkID, int ownerID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool InternalSetOwner(int dataID, int ownerID);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsValidLinkHandle(int handle);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool IsValidNavMeshDataHandle(int handle);
        public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int areaMask) => 
            INTERNAL_CALL_Raycast(ref sourcePosition, ref targetPosition, out hit, areaMask);

        public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, NavMeshQueryFilter filter) => 
            RaycastFilter(sourcePosition, targetPosition, out hit, filter.agentTypeID, filter.areaMask);

        private static bool RaycastFilter(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int type, int mask) => 
            INTERNAL_CALL_RaycastFilter(ref sourcePosition, ref targetPosition, out hit, type, mask);

        /// <summary>
        /// <para>Removes a link from the NavMesh.</para>
        /// </summary>
        /// <param name="handle">The instance of a link to remove.</param>
        public static void RemoveLink(NavMeshLinkInstance handle)
        {
            RemoveLinkInternal(handle.id);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void RemoveLinkInternal(int handle);
        /// <summary>
        /// <para>Removes the specified NavMeshDataInstance from the game, making it unavailable for agents and queries.</para>
        /// </summary>
        /// <param name="handle">The instance of a NavMesh to remove.</param>
        public static void RemoveNavMeshData(NavMeshDataInstance handle)
        {
            RemoveNavMeshDataInternal(handle.id);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void RemoveNavMeshDataInternal(int handle);
        /// <summary>
        /// <para>Removes the build settings matching the agent type ID.</para>
        /// </summary>
        /// <param name="agentTypeID">The ID of the entry to remove.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void RemoveSettings(int agentTypeID);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("RestoreNavMesh has no effect and is deprecated."), GeneratedByOldBindingsGenerator]
        public static extern void RestoreNavMesh();
        public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask) => 
            INTERNAL_CALL_SamplePosition(ref sourcePosition, out hit, maxDistance, areaMask);

        public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, NavMeshQueryFilter filter) => 
            SamplePositionFilter(sourcePosition, out hit, maxDistance, filter.agentTypeID, filter.areaMask);

        private static bool SamplePositionFilter(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int type, int mask) => 
            INTERNAL_CALL_SamplePositionFilter(ref sourcePosition, out hit, maxDistance, type, mask);

        /// <summary>
        /// <para>Sets the cost for finding path over geometry of the area type on all agents.</para>
        /// </summary>
        /// <param name="areaIndex">Index of the area to set.</param>
        /// <param name="cost">New cost.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void SetAreaCost(int areaIndex, float cost);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetAvoidancePredictionTime(float t);
        /// <summary>
        /// <para>Sets the cost for traversing over geometry of the layer type on all agents.</para>
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="cost"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use SetAreaCost instead."), GeneratedByOldBindingsGenerator]
        public static extern void SetLayerCost(int layer, float cost);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern void SetPathfindingIterationsPerFrame(int iter);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use NavMesh.CalculateTriangulation () instead."), GeneratedByOldBindingsGenerator]
        public static extern void Triangulate(out Vector3[] vertices, out int[] indices);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern object TriangulateInternal();

        /// <summary>
        /// <para>Describes how far in the future the agents predict collisions for avoidance.</para>
        /// </summary>
        public static float avoidancePredictionTime
        {
            get => 
                GetAvoidancePredictionTime();
            set
            {
                SetAvoidancePredictionTime(value);
            }
        }

        /// <summary>
        /// <para>The maximum amount of nodes processed each frame in the asynchronous pathfinding process.</para>
        /// </summary>
        public static int pathfindingIterationsPerFrame
        {
            get => 
                GetPathfindingIterationsPerFrame();
            set
            {
                SetPathfindingIterationsPerFrame(value);
            }
        }

        /// <summary>
        /// <para>A delegate which can be used to register callback methods to be invoked before the NavMesh system updates.</para>
        /// </summary>
        public delegate void OnNavMeshPreUpdate();
    }
}

