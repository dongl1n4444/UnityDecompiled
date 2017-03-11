namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>Extension methods to the Terrain class, used only for the UpdateGIMaterials method used by the Global Illumination System.</para>
    /// </summary>
    public static class TerrainExtensions
    {
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_UpdateGIMaterialsForTerrain(Terrain terrain, ref Rect uvBounds);
        /// <summary>
        /// <para>Schedules an update of the albedo and emissive Textures of a system that contains the Terrain.</para>
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void UpdateGIMaterials(this Terrain terrain)
        {
            if (terrain.terrainData == null)
            {
                throw new ArgumentException("Invalid terrainData.");
            }
            UpdateGIMaterialsForTerrain(terrain, new Rect(0f, 0f, 1f, 1f));
        }

        /// <summary>
        /// <para>Schedules an update of the albedo and emissive Textures of a system that contains the Terrain.</para>
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void UpdateGIMaterials(this Terrain terrain, int x, int y, int width, int height)
        {
            if (terrain.terrainData == null)
            {
                throw new ArgumentException("Invalid terrainData.");
            }
            float alphamapWidth = terrain.terrainData.alphamapWidth;
            float alphamapHeight = terrain.terrainData.alphamapHeight;
            UpdateGIMaterialsForTerrain(terrain, new Rect(((float) x) / alphamapWidth, ((float) y) / alphamapHeight, ((float) width) / alphamapWidth, ((float) height) / alphamapHeight));
        }

        internal static void UpdateGIMaterialsForTerrain(Terrain terrain, Rect uvBounds)
        {
            INTERNAL_CALL_UpdateGIMaterialsForTerrain(terrain, ref uvBounds);
        }
    }
}

