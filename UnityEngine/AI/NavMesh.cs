namespace UnityEngine.AI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting.APIUpdating;

    /// <summary>
    /// <para>Singleton class to access the baked NavMesh.</para>
    /// </summary>
    [MovedFrom("UnityEngine")]
    public sealed class NavMesh
    {
        /// <summary>
        /// <para>Area mask constant that includes all NavMesh areas.</para>
        /// </summary>
        public const int AllAreas = -1;

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("AddOffMeshLinks has no effect and is deprecated.")]
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

        internal static bool CalculatePathInternal(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path) => 
            INTERNAL_CALL_CalculatePathInternal(ref sourcePosition, ref targetPosition, areaMask, path);

        /// <summary>
        /// <para>Calculates triangulation of the current navmesh.</para>
        /// </summary>
        public static NavMeshTriangulation CalculateTriangulation() => 
            ((NavMeshTriangulation) TriangulateInternal());

        public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, int areaMask) => 
            INTERNAL_CALL_FindClosestEdge(ref sourcePosition, out hit, areaMask);

        /// <summary>
        /// <para>Gets the cost for path finding over geometry of the area type.</para>
        /// </summary>
        /// <param name="areaIndex">Index of the area to get.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern float GetAreaCost(int areaIndex);
        /// <summary>
        /// <para>Returns the area index for a named NavMesh area type.</para>
        /// </summary>
        /// <param name="areaName">Name of the area to look up.</param>
        /// <returns>
        /// <para>Index if the specified are, or -1 if no area found.</para>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetAreaFromName(string areaName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern float GetAvoidancePredictionTime();
        /// <summary>
        /// <para>Gets the cost for traversing over geometry of the layer type on all agents.</para>
        /// </summary>
        /// <param name="layer"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use GetAreaCost instead.")]
        public static extern float GetLayerCost(int layer);
        /// <summary>
        /// <para>Returns the layer index for a named layer.</para>
        /// </summary>
        /// <param name="layerName"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use GetAreaFromName instead.")]
        public static extern int GetNavMeshLayerFromName(string layerName);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int GetPathfindingIterationsPerFrame();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_CalculatePathInternal(ref Vector3 sourcePosition, ref Vector3 targetPosition, int areaMask, NavMeshPath path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_FindClosestEdge(ref Vector3 sourcePosition, out NavMeshHit hit, int areaMask);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_Raycast(ref Vector3 sourcePosition, ref Vector3 targetPosition, out NavMeshHit hit, int areaMask);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern bool INTERNAL_CALL_SamplePosition(ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask);
        public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int areaMask) => 
            INTERNAL_CALL_Raycast(ref sourcePosition, ref targetPosition, out hit, areaMask);

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("RestoreNavMesh has no effect and is deprecated.")]
        public static extern void RestoreNavMesh();
        public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask) => 
            INTERNAL_CALL_SamplePosition(ref sourcePosition, out hit, maxDistance, areaMask);

        /// <summary>
        /// <para>Sets the cost for finding path over geometry of the area type on all agents.</para>
        /// </summary>
        /// <param name="areaIndex">Index of the area to set.</param>
        /// <param name="cost">New cost.</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetAreaCost(int areaIndex, float cost);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetAvoidancePredictionTime(float t);
        /// <summary>
        /// <para>Sets the cost for traversing over geometry of the layer type on all agents.</para>
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="cost"></param>
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use SetAreaCost instead.")]
        public static extern void SetLayerCost(int layer, float cost);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void SetPathfindingIterationsPerFrame(int iter);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use NavMesh.CalculateTriangulation() instead.")]
        public static extern void Triangulate(out Vector3[] vertices, out int[] indices);
        [MethodImpl(MethodImplOptions.InternalCall)]
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
    }
}

