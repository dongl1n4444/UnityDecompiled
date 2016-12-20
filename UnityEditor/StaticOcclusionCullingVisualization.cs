namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// <para>Used to visualize static occlusion culling at development time in scene view.</para>
    /// </summary>
    public sealed class StaticOcclusionCullingVisualization
    {
        public static bool isPreviewOcclusionCullingCameraInPVS { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static Camera previewOcclucionCamera { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static Camera previewOcclusionCamera { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        public static bool showDynamicObjectBounds { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>If set to true, culling of geometry is enabled.</para>
        /// </summary>
        public static bool showGeometryCulling { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>If set to true, visualization of target volumes is enabled.</para>
        /// </summary>
        public static bool showOcclusionCulling { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>If set to true, visualization of portals is enabled.</para>
        /// </summary>
        public static bool showPortals { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>If set to true, the visualization lines of the PVS volumes will show all cells rather than cells after culling.</para>
        /// </summary>
        public static bool showPreVisualization { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>If set to true, visualization of view volumes is enabled.</para>
        /// </summary>
        public static bool showViewVolumes { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>If set to true, visualization of portals is enabled.</para>
        /// </summary>
        public static bool showVisibilityLines { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

