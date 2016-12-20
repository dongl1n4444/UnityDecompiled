namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <para>Allows to control the dynamic Global Illumination.</para>
    /// </summary>
    public sealed class DynamicGI
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_SetEmissive(Renderer renderer, ref Color color);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_UpdateMaterialsForTerrain(Terrain terrain, ref Rect uvBounds);
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
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void UpdateEnvironment();
        /// <summary>
        /// <para>Schedules an update of the albedo and emissive textures of a system that contains the renderer or the terrain.</para>
        /// </summary>
        /// <param name="renderer">The Renderer to use when searching for a system to update.</param>
        /// <param name="terrain">The Terrain to use when searching for systems to update.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void UpdateMaterials(Renderer renderer)
        {
            UpdateMaterialsForRenderer(renderer);
        }

        /// <summary>
        /// <para>Schedules an update of the albedo and emissive textures of a system that contains the renderer or the terrain.</para>
        /// </summary>
        /// <param name="renderer">The Renderer to use when searching for a system to update.</param>
        /// <param name="terrain">The Terrain to use when searching for systems to update.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void UpdateMaterials(Terrain terrain)
        {
            if (terrain == null)
            {
                throw new ArgumentNullException("terrain");
            }
            if (terrain.terrainData == null)
            {
                throw new ArgumentException("Invalid terrainData.");
            }
            UpdateMaterialsForTerrain(terrain, new Rect(0f, 0f, 1f, 1f));
        }

        /// <summary>
        /// <para>Schedules an update of the albedo and emissive textures of a system that contains the renderer or the terrain.</para>
        /// </summary>
        /// <param name="renderer">The Renderer to use when searching for a system to update.</param>
        /// <param name="terrain">The Terrain to use when searching for systems to update.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void UpdateMaterials(Terrain terrain, int x, int y, int width, int height)
        {
            if (terrain == null)
            {
                throw new ArgumentNullException("terrain");
            }
            if (terrain.terrainData == null)
            {
                throw new ArgumentException("Invalid terrainData.");
            }
            float alphamapWidth = terrain.terrainData.alphamapWidth;
            float alphamapHeight = terrain.terrainData.alphamapHeight;
            UpdateMaterialsForTerrain(terrain, new Rect(((float) x) / alphamapWidth, ((float) y) / alphamapHeight, ((float) width) / alphamapWidth, ((float) height) / alphamapHeight));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void UpdateMaterialsForRenderer(Renderer renderer);
        internal static void UpdateMaterialsForTerrain(Terrain terrain, Rect uvBounds)
        {
            INTERNAL_CALL_UpdateMaterialsForTerrain(terrain, ref uvBounds);
        }

        /// <summary>
        /// <para>Allows for scaling the contribution coming from realtime &amp; static  lightmaps.</para>
        /// </summary>
        public static float indirectScale { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>When enabled, new dynamic Global Illumination output is shown in each frame.</para>
        /// </summary>
        public static bool synchronousMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Threshold for limiting updates of realtime GI. The unit of measurement is "percentage intensity change".</para>
        /// </summary>
        public static float updateThreshold { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }
    }
}

