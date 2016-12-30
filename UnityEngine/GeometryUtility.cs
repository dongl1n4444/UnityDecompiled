namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Utility class for common geometric functions.</para>
    /// </summary>
    public sealed class GeometryUtility
    {
        /// <summary>
        /// <para>Calculates a bounding box given an array of positions and a transformation matrix.</para>
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="transform"></param>
        public static Bounds CalculateBounds(Vector3[] positions, Matrix4x4 transform)
        {
            if (positions == null)
            {
                throw new ArgumentNullException("positions");
            }
            if (positions.Length == 0)
            {
                throw new ArgumentException("Zero-sized array is not allowed.", "positions");
            }
            return Internal_CalculateBounds(positions, transform);
        }

        /// <summary>
        /// <para>Calculates frustum planes.</para>
        /// </summary>
        /// <param name="camera"></param>
        public static Plane[] CalculateFrustumPlanes(Camera camera) => 
            CalculateFrustumPlanes(camera.projectionMatrix * camera.worldToCameraMatrix);

        /// <summary>
        /// <para>Calculates frustum planes.</para>
        /// </summary>
        /// <param name="worldToProjectionMatrix"></param>
        public static Plane[] CalculateFrustumPlanes(Matrix4x4 worldToProjectionMatrix)
        {
            Plane[] planes = new Plane[6];
            Internal_ExtractPlanes(planes, worldToProjectionMatrix);
            return planes;
        }

        private static Bounds Internal_CalculateBounds(Vector3[] positions, Matrix4x4 transform)
        {
            Bounds bounds;
            INTERNAL_CALL_Internal_CalculateBounds(positions, ref transform, out bounds);
            return bounds;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_CalculateBounds(Vector3[] positions, ref Matrix4x4 transform, out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_Internal_ExtractPlanes(Plane[] planes, ref Matrix4x4 worldToProjectionMatrix);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern bool INTERNAL_CALL_TestPlanesAABB(Plane[] planes, ref Bounds bounds);
        private static void Internal_ExtractPlanes(Plane[] planes, Matrix4x4 worldToProjectionMatrix)
        {
            INTERNAL_CALL_Internal_ExtractPlanes(planes, ref worldToProjectionMatrix);
        }

        /// <summary>
        /// <para>Returns true if bounds are inside the plane array.</para>
        /// </summary>
        /// <param name="planes"></param>
        /// <param name="bounds"></param>
        public static bool TestPlanesAABB(Plane[] planes, Bounds bounds) => 
            INTERNAL_CALL_TestPlanesAABB(planes, ref bounds);
    }
}

