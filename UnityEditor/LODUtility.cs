namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>LOD Utility Helpers.</para>
    /// </summary>
    public sealed class LODUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern float CalculateDistance(Camera camera, float relativeScreenHeight, LODGroup group);
        /// <summary>
        /// <para>Recalculate the bounding region for the given LODGroup.</para>
        /// </summary>
        /// <param name="group"></param>
        public static void CalculateLODGroupBoundingBox(LODGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            group.RecalculateBounds();
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern LODVisualizationInformation CalculateVisualizationData(Camera camera, LODGroup group, int lodLevel);
        internal static Vector3 CalculateWorldReferencePoint(LODGroup group)
        {
            Vector3 vector;
            INTERNAL_CALL_CalculateWorldReferencePoint(group, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_CalculateWorldReferencePoint(LODGroup group, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal static extern bool NeedUpdateLODGroupBoundingBox(LODGroup group);
    }
}

