namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>The TerrainData class stores heightmaps, detail mesh positions, tree instances, and terrain texture alpha maps.</para>
    /// </summary>
    public sealed class TerrainData : UnityEngine.Object
    {
        private static readonly int kMaximumAlphamapResolution = Internal_GetMaximumAlphamapResolution();
        private static readonly int kMaximumBaseMapResolution = Internal_GetMaximumBaseMapResolution();
        private static readonly int kMaximumDetailPatchCount = Internal_GetMaximumDetailPatchCount();
        private static readonly int kMaximumDetailResolutionPerPatch = Internal_GetMaximumDetailResolutionPerPatch();
        private static readonly int kMaximumResolution = Internal_GetMaximumResolution();
        private static readonly int kMinimumAlphamapResolution = Internal_GetMinimumAlphamapResolution();
        private static readonly int kMinimumBaseMapResolution = Internal_GetMinimumBaseMapResolution();
        private static readonly int kMinimumDetailResolutionPerPatch = Internal_GetMinimumDetailResolutionPerPatch();

        public TerrainData()
        {
            this.Internal_Create(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void AddTree(out TreeInstance tree);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void AddUser(GameObject user);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern int GetAdjustedSize(int size);
        [MethodImpl(MethodImplOptions.InternalCall), RequiredByNativeCode, GeneratedByOldBindingsGenerator]
        internal extern float GetAlphamapResolutionInternal();
        /// <summary>
        /// <para>Returns the alpha map at a position x, y given a width and height.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern float[,,] GetAlphamaps(int x, int y, int width, int height);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern Texture2D GetAlphamapTexture(int index);
        /// <summary>
        /// <para>Returns a 2D array of the detail object density in the specific location.</para>
        /// </summary>
        /// <param name="xBase"></param>
        /// <param name="yBase"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="layer"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int[,] GetDetailLayer(int xBase, int yBase, int width, int height, int layer);
        /// <summary>
        /// <para>Gets the height at a certain point x,y.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern float GetHeight(int x, int y);
        /// <summary>
        /// <para>Get an array of heightmap samples.</para>
        /// </summary>
        /// <param name="xBase">First x index of heightmap samples to retrieve.</param>
        /// <param name="yBase">First y index of heightmap samples to retrieve.</param>
        /// <param name="width">Number of samples to retrieve along the heightmap's x axis.</param>
        /// <param name="height">Number of samples to retrieve along the heightmap's y axis.</param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern float[,] GetHeights(int xBase, int yBase, int width, int height);
        /// <summary>
        /// <para>Gets an interpolated height at a point x,y.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern float GetInterpolatedHeight(float x, float y);
        /// <summary>
        /// <para>Get an interpolated normal at a given location.</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector3 GetInterpolatedNormal(float x, float y)
        {
            Vector3 vector;
            INTERNAL_CALL_GetInterpolatedNormal(this, x, y, out vector);
            return vector;
        }

        /// <summary>
        /// <para>Gets the gradient of the terrain at point (x,y).</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern float GetSteepness(float x, float y);
        /// <summary>
        /// <para>Returns an array of all supported detail layer indices in the area.</para>
        /// </summary>
        /// <param name="xBase"></param>
        /// <param name="yBase"></param>
        /// <param name="totalWidth"></param>
        /// <param name="totalHeight"></param>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern int[] GetSupportedLayers(int xBase, int yBase, int totalWidth, int totalHeight);
        /// <summary>
        /// <para>Get the tree instance at the specified index. It is used as a faster version of treeInstances[index] as this function doesn't create the entire tree instances array.</para>
        /// </summary>
        /// <param name="index">The index of the tree instance.</param>
        public TreeInstance GetTreeInstance(int index)
        {
            TreeInstance instance;
            INTERNAL_CALL_GetTreeInstance(this, index, out instance);
            return instance;
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool HasUser(GameObject user);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetInterpolatedNormal(TerrainData self, float x, float y, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_GetTreeInstance(TerrainData self, int index, out TreeInstance value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern int INTERNAL_CALL_RemoveTrees(TerrainData self, ref Vector2 position, float radius, int prototypeIndex);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private static extern void INTERNAL_CALL_SetTreeInstance(TerrainData self, int index, ref TreeInstance instance);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void Internal_Create([Writable] TerrainData terrainData);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_heightmapScale(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_size(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_get_wavingGrassTint(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetMaximumAlphamapResolution();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetMaximumBaseMapResolution();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetMaximumDetailPatchCount();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetMaximumDetailResolutionPerPatch();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetMaximumResolution();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetMinimumAlphamapResolution();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetMinimumBaseMapResolution();
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, GeneratedByOldBindingsGenerator]
        private static extern int Internal_GetMinimumDetailResolutionPerPatch();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_size(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void INTERNAL_set_wavingGrassTint(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetAlphamaps(int x, int y, int width, int height, float[,,] map);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetDetailLayer(int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, int[,] data);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetDetailResolution(int patchCount, int resolutionPerPatch);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetHeights(int xBase, int yBase, int width, int height, float[,] heights);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        private extern void Internal_SetHeightsDelayLOD(int xBase, int yBase, int width, int height, float[,] heights);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern bool NeedUpgradeScaledTreePrototypes();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void RecalculateBasemapIfDirty();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void RecalculateTreePositions();
        /// <summary>
        /// <para>Reloads all the values of the available prototypes (ie, detail mesh assets) in the TerrainData Object.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        public extern void RefreshPrototypes();
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void RemoveDetailPrototype(int index);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void RemoveTreePrototype(int index);
        internal int RemoveTrees(Vector2 position, float radius, int prototypeIndex) => 
            INTERNAL_CALL_RemoveTrees(this, ref position, radius, prototypeIndex);

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void RemoveUser(GameObject user);
        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void ResetDirtyDetails();
        public void SetAlphamaps(int x, int y, float[,,] map)
        {
            if (map.GetLength(2) != this.alphamapLayers)
            {
                object[] args = new object[] { this.alphamapLayers };
                throw new Exception(UnityString.Format("Float array size wrong (layers should be {0})", args));
            }
            this.Internal_SetAlphamaps(x, y, map.GetLength(1), map.GetLength(0), map);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void SetBasemapDirty(bool dirty);
        public void SetDetailLayer(int xBase, int yBase, int layer, int[,] details)
        {
            this.Internal_SetDetailLayer(xBase, yBase, details.GetLength(1), details.GetLength(0), layer, details);
        }

        /// <summary>
        /// <para>Set the resolution of the detail map.</para>
        /// </summary>
        /// <param name="detailResolution">Specifies the number of pixels in the detail resolution map. A larger detailResolution, leads to more accurate detail object painting.</param>
        /// <param name="resolutionPerPatch">Specifies the size in pixels of each individually rendered detail patch. A larger number reduces draw calls, but might increase triangle count since detail patches are culled on a per batch basis. A recommended value is 16. If you use a very large detail object distance and your grass is very sparse, it makes sense to increase the value.</param>
        public void SetDetailResolution(int detailResolution, int resolutionPerPatch)
        {
            if (detailResolution < 0)
            {
                Debug.LogWarning("detailResolution must not be negative.");
                detailResolution = 0;
            }
            if ((resolutionPerPatch < kMinimumDetailResolutionPerPatch) || (resolutionPerPatch > kMaximumDetailResolutionPerPatch))
            {
                Debug.LogWarning(string.Concat(new object[] { "resolutionPerPatch is clamped to the range of [", kMinimumDetailResolutionPerPatch, ", ", kMaximumDetailResolutionPerPatch, "]." }));
                resolutionPerPatch = Math.Min(kMaximumDetailResolutionPerPatch, Math.Max(resolutionPerPatch, kMinimumDetailResolutionPerPatch));
            }
            int num = detailResolution / resolutionPerPatch;
            if (num > kMaximumDetailPatchCount)
            {
                Debug.LogWarning("Patch count (detailResolution / resolutionPerPatch) is clamped to the range of [0, " + kMaximumDetailPatchCount + "].");
                num = Math.Min(kMaximumDetailPatchCount, Math.Max(num, 0));
            }
            this.Internal_SetDetailResolution(num, resolutionPerPatch);
        }

        public void SetHeights(int xBase, int yBase, float[,] heights)
        {
            if (heights == null)
            {
                throw new NullReferenceException();
            }
            if (((((xBase + heights.GetLength(1)) > this.heightmapWidth) || ((xBase + heights.GetLength(1)) < 0)) || (((yBase + heights.GetLength(0)) < 0) || (xBase < 0))) || ((yBase < 0) || ((yBase + heights.GetLength(0)) > this.heightmapHeight)))
            {
                object[] args = new object[] { xBase + heights.GetLength(1), yBase + heights.GetLength(0), this.heightmapWidth, this.heightmapHeight };
                throw new ArgumentException(UnityString.Format("X or Y base out of bounds. Setting up to {0}x{1} while map size is {2}x{3}", args));
            }
            this.Internal_SetHeights(xBase, yBase, heights.GetLength(1), heights.GetLength(0), heights);
        }

        public void SetHeightsDelayLOD(int xBase, int yBase, float[,] heights)
        {
            if (heights == null)
            {
                throw new ArgumentNullException("heights");
            }
            int length = heights.GetLength(0);
            int width = heights.GetLength(1);
            if (((xBase < 0) || ((xBase + width) < 0)) || ((xBase + width) > this.heightmapWidth))
            {
                object[] args = new object[] { xBase, xBase + width, this.heightmapWidth };
                throw new ArgumentException(UnityString.Format("X out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", args));
            }
            if (((yBase < 0) || ((yBase + length) < 0)) || ((yBase + length) > this.heightmapHeight))
            {
                object[] objArray2 = new object[] { yBase, yBase + length, this.heightmapHeight };
                throw new ArgumentException(UnityString.Format("Y out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", objArray2));
            }
            this.Internal_SetHeightsDelayLOD(xBase, yBase, width, length, heights);
        }

        /// <summary>
        /// <para>Set the tree instance with new parameters at the specified index. However, TreeInstance.prototypeIndex and TreeInstance.position can not be changed otherwise an ArgumentException will be thrown.</para>
        /// </summary>
        /// <param name="index">The index of the tree instance.</param>
        /// <param name="instance">The new TreeInstance value.</param>
        public void SetTreeInstance(int index, TreeInstance instance)
        {
            INTERNAL_CALL_SetTreeInstance(this, index, ref instance);
        }

        [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator]
        internal extern void UpgradeScaledTreePrototype();

        /// <summary>
        /// <para>Height of the alpha map.</para>
        /// </summary>
        public int alphamapHeight =>
            this.alphamapResolution;

        /// <summary>
        /// <para>Number of alpha map layers.</para>
        /// </summary>
        public int alphamapLayers { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Resolution of the alpha map.</para>
        /// </summary>
        public int alphamapResolution
        {
            get => 
                this.Internal_alphamapResolution;
            set
            {
                int num = value;
                if ((value < kMinimumAlphamapResolution) || (value > kMaximumAlphamapResolution))
                {
                    Debug.LogWarning(string.Concat(new object[] { "alphamapResolution is clamped to the range of [", kMinimumAlphamapResolution, ", ", kMaximumAlphamapResolution, "]." }));
                    num = Math.Min(kMaximumAlphamapResolution, Math.Max(value, kMinimumAlphamapResolution));
                }
                this.Internal_alphamapResolution = num;
            }
        }

        private int alphamapTextureCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Alpha map textures used by the Terrain. Used by Terrain Inspector for undo.</para>
        /// </summary>
        public Texture2D[] alphamapTextures
        {
            get
            {
                Texture2D[] texturedArray = new Texture2D[this.alphamapTextureCount];
                for (int i = 0; i < texturedArray.Length; i++)
                {
                    texturedArray[i] = this.GetAlphamapTexture(i);
                }
                return texturedArray;
            }
        }

        /// <summary>
        /// <para>Width of the alpha map.</para>
        /// </summary>
        public int alphamapWidth =>
            this.alphamapResolution;

        /// <summary>
        /// <para>Resolution of the base map used for rendering far patches on the terrain.</para>
        /// </summary>
        public int baseMapResolution
        {
            get => 
                this.Internal_baseMapResolution;
            set
            {
                int num = value;
                if ((value < kMinimumBaseMapResolution) || (value > kMaximumBaseMapResolution))
                {
                    Debug.LogWarning(string.Concat(new object[] { "baseMapResolution is clamped to the range of [", kMinimumBaseMapResolution, ", ", kMaximumBaseMapResolution, "]." }));
                    num = Math.Min(kMaximumBaseMapResolution, Math.Max(value, kMinimumBaseMapResolution));
                }
                this.Internal_baseMapResolution = num;
            }
        }

        /// <summary>
        /// <para>The local bounding box of the TerrainData object.</para>
        /// </summary>
        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
        }

        /// <summary>
        /// <para>Detail height of the TerrainData.</para>
        /// </summary>
        public int detailHeight { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Contains the detail texture/meshes that the terrain has.</para>
        /// </summary>
        public DetailPrototype[] detailPrototypes { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Detail Resolution of the TerrainData.</para>
        /// </summary>
        public int detailResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        internal int detailResolutionPerPatch { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Detail width of the TerrainData.</para>
        /// </summary>
        public int detailWidth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Height of the terrain in samples (Read Only).</para>
        /// </summary>
        public int heightmapHeight { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Resolution of the heightmap.</para>
        /// </summary>
        public int heightmapResolution
        {
            get => 
                this.Internal_heightmapResolution;
            set
            {
                int num = value;
                if ((value < 0) || (value > kMaximumResolution))
                {
                    Debug.LogWarning("heightmapResolution is clamped to the range of [0, " + kMaximumResolution + "].");
                    num = Math.Min(kMaximumResolution, Math.Max(value, 0));
                }
                this.Internal_heightmapResolution = num;
            }
        }

        /// <summary>
        /// <para>The size of each heightmap sample.</para>
        /// </summary>
        public Vector3 heightmapScale
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_heightmapScale(out vector);
                return vector;
            }
        }

        /// <summary>
        /// <para>Width of the terrain in samples (Read Only).</para>
        /// </summary>
        public int heightmapWidth { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        private int Internal_alphamapResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        private int Internal_baseMapResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        private int Internal_heightmapResolution { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The total size in world units of the terrain.</para>
        /// </summary>
        public Vector3 size
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_size(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_size(ref value);
            }
        }

        /// <summary>
        /// <para>Splat texture used by the terrain.</para>
        /// </summary>
        public SplatPrototype[] splatPrototypes { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The thickness of the terrain used for collision detection.</para>
        /// </summary>
        public float thickness { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Returns the number of tree instances.</para>
        /// </summary>
        public int treeInstanceCount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; }

        /// <summary>
        /// <para>Contains the current trees placed in the terrain.</para>
        /// </summary>
        public TreeInstance[] treeInstances { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>The list of tree prototypes this are the ones available in the inspector.</para>
        /// </summary>
        public TreePrototype[] treePrototypes { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Amount of waving grass in the terrain.</para>
        /// </summary>
        public float wavingGrassAmount { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Speed of the waving grass.</para>
        /// </summary>
        public float wavingGrassSpeed { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Strength of the waving grass in the terrain.</para>
        /// </summary>
        public float wavingGrassStrength { [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] get; [MethodImpl(MethodImplOptions.InternalCall), GeneratedByOldBindingsGenerator] set; }

        /// <summary>
        /// <para>Color of the waving grass that the terrain has.</para>
        /// </summary>
        public Color wavingGrassTint
        {
            get
            {
                Color color;
                this.INTERNAL_get_wavingGrassTint(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_wavingGrassTint(ref value);
            }
        }
    }
}

