namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Allows to control the dynamic Global Illumination.</para>
    /// </summary>
    public sealed class DynamicGI
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetEmissive(Renderer renderer, ref Color color);
        /// <summary>
        /// <para>Allows to set an emissive color for a given renderer quickly, without the need to render the emissive input for the entire system.</para>
        /// </summary>
        /// <param name="renderer">The Renderer that should get a new color.</param>
        /// <param name="color">The emissive Color.</param>
        public static void SetEmissive(Renderer renderer, Color color)
        {
            INTERNAL_CALL_SetEmissive(renderer, ref color);
        }

        /// <summary>
        /// <para>Schedules an update of the environment texture.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public static extern void UpdateEnvironment();
        [Obsolete("DynamicGI.UpdateMaterials(Terrain) is depracated; instead, use extension method from TerrainExtensions: 'terrain.UpdateGIMaterials()' (UnityUpgradable).", true)]
        public static void UpdateMaterials(UnityEngine.Object renderer)
        {
        }

        /// <summary>
        /// <para>Schedules an update of the albedo and emissive textures of a system that contains the renderer or the terrain.</para>
        /// </summary>
        /// <param name="renderer">The Renderer to use when searching for a system to update.</param>
        /// <param name="terrain">The Terrain to use when searching for systems to update.</param>
        [Obsolete("DynamicGI.UpdateMaterials(Renderer) is depracated; instead, use extension method from RendererExtensions: 'renderer.UpdateGIMaterials()' (UnityUpgradable).", true)]
        public static void UpdateMaterials(Renderer renderer)
        {
        }

        [Obsolete("DynamicGI.UpdateMaterials(Terrain, int, int, int, int) is depracated; instead, use extension method from TerrainExtensions: 'terrain.UpdateGIMaterials(x, y, width, height)' (UnityUpgradable).", true)]
        public static void UpdateMaterials(UnityEngine.Object renderer, int x, int y, int width, int height)
        {
        }

        /// <summary>
        /// <para>Allows for scaling the contribution coming from realtime &amp; static  lightmaps.</para>
        /// </summary>
        public static float indirectScale { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>When enabled, new dynamic Global Illumination output is shown in each frame.</para>
        /// </summary>
        public static bool synchronousMode { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Threshold for limiting updates of realtime GI. The unit of measurement is "percentage intensity change".</para>
        /// </summary>
        public static float updateThreshold { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }
    }
}

