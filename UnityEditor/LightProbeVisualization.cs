namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    internal sealed class LightProbeVisualization
    {
        internal static void DrawPointCloud(Vector3[] unselectedPositions, Vector3[] selectedPositions, Color baseColor, Color selectedColor, float scale, Transform cloudTransform)
        {
            INTERNAL_CALL_DrawPointCloud(unselectedPositions, selectedPositions, ref baseColor, ref selectedColor, scale, cloudTransform);
        }

        internal static void DrawTetrahedra(bool shouldRecalculateTetrahedra, Vector3 cameraPosition)
        {
            INTERNAL_CALL_DrawTetrahedra(shouldRecalculateTetrahedra, ref cameraPosition);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_DrawPointCloud(Vector3[] unselectedPositions, Vector3[] selectedPositions, ref Color baseColor, ref Color selectedColor, float scale, Transform cloudTransform);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_DrawTetrahedra(bool shouldRecalculateTetrahedra, ref Vector3 cameraPosition);

        public static bool dynamicUpdateLightProbes { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static LightProbeVisualizationMode lightProbeVisualizationMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static bool showInterpolationWeights { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        public static bool showOcclusions { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        internal enum LightProbeVisualizationMode
        {
            OnlyProbesUsedBySelection,
            AllProbesNoCells,
            AllProbesWithCells,
            None
        }
    }
}

