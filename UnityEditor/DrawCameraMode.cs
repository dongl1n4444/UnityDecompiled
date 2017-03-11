namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Drawing modes for Handles.DrawCamera.</para>
    /// </summary>
    public enum DrawCameraMode
    {
        /// <summary>
        /// <para>Draw objects with the albedo component only. This value has been deprecated. Please use DrawCameraMode.RealtimeAlbedo.</para>
        /// </summary>
        [Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeAlbedo instead. (UnityUpgradable) -> RealtimeAlbedo", true)]
        Albedo = -14,
        /// <summary>
        /// <para>The camera is set to display the alpha channel of the rendering.</para>
        /// </summary>
        AlphaChannel = 5,
        /// <summary>
        /// <para>Draw objects with baked GI only. This value has been deprecated. Please use DrawCameraMode.BakedLightmap.</para>
        /// </summary>
        [Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use BakedLightmap instead. (UnityUpgradable) -> BakedLightmap", true)]
        Baked = -18,
        /// <summary>
        /// <para>Draw objects with the baked albedo component only.</para>
        /// </summary>
        BakedAlbedo = 0x19,
        /// <summary>
        /// <para>Draw objects with different colors for each baked chart (UV island).</para>
        /// </summary>
        BakedCharting = 30,
        /// <summary>
        /// <para>Draw objects with the baked directionality component only.</para>
        /// </summary>
        BakedDirectionality = 0x1b,
        /// <summary>
        /// <para>Draw objects with the baked emission component only.</para>
        /// </summary>
        BakedEmissive = 0x1a,
        /// <summary>
        /// <para>Draw objects with baked indices only.</para>
        /// </summary>
        BakedIndices = 0x1d,
        /// <summary>
        /// <para>Draw objects with the baked lightmap only.</para>
        /// </summary>
        BakedLightmap = 0x12,
        /// <summary>
        /// <para>Draw objects with baked texel validity only.</para>
        /// </summary>
        BakedTexelValidity = 0x1c,
        /// <summary>
        /// <para>Draw objects with different colors for each real-time chart (UV island).</para>
        /// </summary>
        [Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeCharting instead. (UnityUpgradable) -> RealtimeCharting", true)]
        Charting = -12,
        /// <summary>
        /// <para>Draw with different colors for each cluster.</para>
        /// </summary>
        Clustering = 0x13,
        /// <summary>
        /// <para>Draw diffuse color of Deferred Shading g-buffer.</para>
        /// </summary>
        DeferredDiffuse = 8,
        /// <summary>
        /// <para>Draw world space normal of Deferred Shading g-buffer.</para>
        /// </summary>
        DeferredNormal = 11,
        /// <summary>
        /// <para>Draw smoothness value of Deferred Shading g-buffer.</para>
        /// </summary>
        DeferredSmoothness = 10,
        /// <summary>
        /// <para>Draw specular color of Deferred Shading g-buffer.</para>
        /// </summary>
        DeferredSpecular = 9,
        /// <summary>
        /// <para>Draw objects with directionality for real-time GI. This value has been deprecated. Please use DrawCameraMode.RealtimeDirectionality.</para>
        /// </summary>
        [Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeDirectionality instead. (UnityUpgradable) -> RealtimeDirectionality", true)]
        Directionality = -17,
        /// <summary>
        /// <para>Draw objects with the emission component only. This value has been deprecated. Please use DrawCameraMode.RealtimeEmissive.</para>
        /// </summary>
        [Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeEmissive instead. (UnityUpgradable) -> RealtimeEmissive", true)]
        Emissive = -15,
        /// <summary>
        /// <para>Draw objects with real-time GI only. This value has been deprecated. Please use DrawCameraMode.RealtimeIndirect.</para>
        /// </summary>
        [Obsolete("Renamed to better distinguish this mode from new Progressive baked modes. Please use RealtimeIndirect instead. (UnityUpgradable) -> RealtimeIndirect", true)]
        Irradiance = -16,
        /// <summary>
        /// <para>The camera is set to show in red static lights that fall back to 'static' because more than four light volumes are overlapping.</para>
        /// </summary>
        LightOverlap = 0x18,
        /// <summary>
        /// <para>Draw lit clusters.</para>
        /// </summary>
        LitClustering = 20,
        /// <summary>
        /// <para>The camera is set to display the texture resolution, with a red tint indicating resolution that is too high, and a blue tint indicating texture sizes that could be higher.</para>
        /// </summary>
        Mipmaps = 7,
        /// <summary>
        /// <para>Draw the camera like it would be drawn in-game. This uses the clear flags of the camera.</para>
        /// </summary>
        Normal = -1,
        /// <summary>
        /// <para>The camera is set to display Scene overdraw, with brighter colors indicating more overdraw.</para>
        /// </summary>
        Overdraw = 6,
        /// <summary>
        /// <para>Draw objects with the real-time GI albedo component only.</para>
        /// </summary>
        RealtimeAlbedo = 14,
        /// <summary>
        /// <para>Draw objects with different colors for each real-time chart (UV island).</para>
        /// </summary>
        RealtimeCharting = 12,
        /// <summary>
        /// <para>Draw objects with the real-time GI directionality component only.</para>
        /// </summary>
        RealtimeDirectionality = 0x11,
        /// <summary>
        /// <para>Draw objects with the real-time GI emission component only.</para>
        /// </summary>
        RealtimeEmissive = 15,
        /// <summary>
        /// <para>Draw objects with the real-time GI indirect light only.</para>
        /// </summary>
        RealtimeIndirect = 0x10,
        /// <summary>
        /// <para>The camera is set to draw color coded render paths.</para>
        /// </summary>
        RenderPaths = 4,
        /// <summary>
        /// <para>The camera is set to draw directional light shadow map cascades.</para>
        /// </summary>
        ShadowCascades = 3,
        /// <summary>
        /// <para>The camera is set to display colored ShadowMasks, coloring light gizmo with the same color.</para>
        /// </summary>
        ShadowMasks = 0x17,
        /// <summary>
        /// <para>Draw objects with different color for each GI system.</para>
        /// </summary>
        Systems = 13,
        /// <summary>
        /// <para>Draw the camera textured with selection wireframe and no background clearing.</para>
        /// </summary>
        Textured = 0,
        /// <summary>
        /// <para>Draw the camera where all objects have a wireframe overlay. and no background clearing.</para>
        /// </summary>
        TexturedWire = 2,
        /// <summary>
        /// <para>The camera is set to draw a physically based, albedo validated rendering.</para>
        /// </summary>
        ValidateAlbedo = 0x15,
        /// <summary>
        /// <para>The camera is set to draw a physically based, metal or specular validated rendering.</para>
        /// </summary>
        ValidateMetalSpecular = 0x16,
        /// <summary>
        /// <para>Draw the camera in wireframe and no background clearing.</para>
        /// </summary>
        Wireframe = 1
    }
}

