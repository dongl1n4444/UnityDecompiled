namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The Terrain component renders the terrain.</para>
    /// </summary>
    [UsedByNativeCode]
    public sealed class Terrain : Behaviour
    {
        /// <summary>
        /// <para>Adds a tree instance to the terrain.</para>
        /// </summary>
        /// <param name="instance"></param>
        public void AddTreeInstance(TreeInstance instance)
        {
            INTERNAL_CALL_AddTreeInstance(this, ref instance);
        }

        /// <summary>
        /// <para>Update the terrain's LOD and vegetation information after making changes with TerrainData.SetHeightsDelayLOD.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ApplyDelayedHeightmapModification();
        /// <summary>
        /// <para>Creates a Terrain including collider from TerrainData.</para>
        /// </summary>
        /// <param name="assignTerrain"></param>
        [MethodImpl(MethodImplOptions.InternalCall), UsedByNativeCode]
        public static extern GameObject CreateTerrainGameObject(TerrainData assignTerrain);
        /// <summary>
        /// <para>Flushes any change done in the terrain so it takes effect.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Flush();
        public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
        {
            this.GetClosestReflectionProbesInternal(result);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void GetClosestReflectionProbesInternal(object result);
        /// <summary>
        /// <para>Get the position of the terrain.</para>
        /// </summary>
        public Vector3 GetPosition()
        {
            Vector3 vector;
            INTERNAL_CALL_GetPosition(this, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Get the previously set splat material properties by copying to the dest MaterialPropertyBlock object.</para>
        /// </summary>
        /// <param name="dest"></param>
        public void GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest)
        {
            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }
            this.Internal_GetSplatMaterialPropertyBlock(dest);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_AddTreeInstance(Terrain self, ref TreeInstance instance);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_GetPosition(Terrain self, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void INTERNAL_CALL_RemoveTrees(Terrain self, ref Vector2 position, float radius, int prototypeIndex);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern float INTERNAL_CALL_SampleHeight(Terrain self, ref Vector3 worldPosition);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_legacySpecular(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_lightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_get_realtimeLightmapScaleOffset(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void Internal_GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_legacySpecular(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_lightmapScaleOffset(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void INTERNAL_set_realtimeLightmapScaleOffset(ref Vector4 value);
        internal void RemoveTrees(Vector2 position, float radius, int prototypeIndex)
        {
            INTERNAL_CALL_RemoveTrees(this, ref position, radius, prototypeIndex);
        }

        /// <summary>
        /// <para>Samples the height at the given position defined in world space, relative to the terrain space.</para>
        /// </summary>
        /// <param name="worldPosition"></param>
        public float SampleHeight(Vector3 worldPosition) => 
            INTERNAL_CALL_SampleHeight(this, ref worldPosition);

        /// <summary>
        /// <para>Lets you setup the connection between neighboring Terrains.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom);
        /// <summary>
        /// <para>Set the additional material properties when rendering the terrain heightmap using the splat material.</para>
        /// </summary>
        /// <param name="properties"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetSplatMaterialPropertyBlock(MaterialPropertyBlock properties);

        /// <summary>
        /// <para>The active terrain. This is a convenience function to get to the main terrain in the scene.</para>
        /// </summary>
        public static Terrain activeTerrain { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>The active terrains in the scene.</para>
        /// </summary>
        public static Terrain[] activeTerrains { [MethodImpl(MethodImplOptions.InternalCall)] get; }

        /// <summary>
        /// <para>Specifies if an array of internal light probes should be baked for terrain trees. Available only in editor.</para>
        /// </summary>
        public bool bakeLightProbesForTrees { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Heightmap patches beyond basemap distance will use a precomputed low res basemap.</para>
        /// </summary>
        public float basemapDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Should terrain cast shadows?.</para>
        /// </summary>
        public bool castShadows { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Collect detail patches from memory.</para>
        /// </summary>
        public bool collectDetailPatches { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Density of detail objects.</para>
        /// </summary>
        public float detailObjectDensity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Detail objects will be displayed up to this distance.</para>
        /// </summary>
        public float detailObjectDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Specify if terrain heightmap should be drawn.</para>
        /// </summary>
        public bool drawHeightmap { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Specify if terrain trees and details should be drawn.</para>
        /// </summary>
        public bool drawTreesAndFoliage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Controls what part of the terrain should be rendered.</para>
        /// </summary>
        public TerrainRenderFlags editorRenderFlags { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Lets you essentially lower the heightmap resolution used for rendering.</para>
        /// </summary>
        public int heightmapMaximumLOD { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>An approximation of how many pixels the terrain will pop in the worst case when switching lod.</para>
        /// </summary>
        public float heightmapPixelError { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The shininess value of the terrain.</para>
        /// </summary>
        public float legacyShininess { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The specular color of the terrain.</para>
        /// </summary>
        public Color legacySpecular
        {
            get
            {
                Color color;
                this.INTERNAL_get_legacySpecular(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_legacySpecular(ref value);
            }
        }

        /// <summary>
        /// <para>The index of the baked lightmap applied to this terrain.</para>
        /// </summary>
        public int lightmapIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The UV scale &amp; offset used for a baked lightmap.</para>
        /// </summary>
        public Vector4 lightmapScaleOffset
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_lightmapScaleOffset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_lightmapScaleOffset(ref value);
            }
        }

        /// <summary>
        /// <para>The custom material used to render the terrain.</para>
        /// </summary>
        public Material materialTemplate { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The type of the material used to render the terrain. Could be one of the built-in types or custom. See Terrain.MaterialType.</para>
        /// </summary>
        public MaterialType materialType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The index of the realtime lightmap applied to this terrain.</para>
        /// </summary>
        public int realtimeLightmapIndex { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The UV scale &amp; offset used for a realtime lightmap.</para>
        /// </summary>
        public Vector4 realtimeLightmapScaleOffset
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_realtimeLightmapScaleOffset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_realtimeLightmapScaleOffset(ref value);
            }
        }

        /// <summary>
        /// <para>How reflection probes are used for terrain. See Rendering.ReflectionProbeUsage.</para>
        /// </summary>
        public ReflectionProbeUsage reflectionProbeUsage { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        [Obsolete("use basemapDistance", true)]
        public float splatmapDistance
        {
            get => 
                this.basemapDistance;
            set
            {
                this.basemapDistance = value;
            }
        }

        /// <summary>
        /// <para>The Terrain Data that stores heightmaps, terrain textures, detail meshes and trees.</para>
        /// </summary>
        public TerrainData terrainData { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Distance from the camera where trees will be rendered as billboards only.</para>
        /// </summary>
        public float treeBillboardDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Total distance delta that trees will use to transition from billboard orientation to mesh orientation.</para>
        /// </summary>
        public float treeCrossFadeLength { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The maximum distance at which trees are rendered.</para>
        /// </summary>
        public float treeDistance { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The multiplier to the current LOD bias used for rendering LOD trees (i.e. SpeedTree trees).</para>
        /// </summary>
        public float treeLODBiasMultiplier { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>Maximum number of trees rendered at full LOD.</para>
        /// </summary>
        public int treeMaximumFullLODCount { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

        /// <summary>
        /// <para>The type of the material used to render a terrain object. Could be one of the built-in types or custom.</para>
        /// </summary>
        public enum MaterialType
        {
            BuiltInStandard,
            BuiltInLegacyDiffuse,
            BuiltInLegacySpecular,
            Custom
        }
    }
}

