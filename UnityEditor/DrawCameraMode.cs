namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Drawing modes for Handles.DrawCamera.</para>
    /// </summary>
    public enum DrawCameraMode
    {
        /// <summary>
        /// <para>Draw objects with the albedo component only.</para>
        /// </summary>
        Albedo = 14,
        /// <summary>
        /// <para>Display alpha channel of the rendering.</para>
        /// </summary>
        AlphaChannel = 5,
        /// <summary>
        /// <para>Draw objects with baked GI only.</para>
        /// </summary>
        Baked = 0x12,
        /// <summary>
        /// <para>Draw objects with different color for each chart (UV island).</para>
        /// </summary>
        Charting = 12,
        /// <summary>
        /// <para>Draw with different color for each cluster.</para>
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
        /// <para>Draw objects with directionality for real-time GI.</para>
        /// </summary>
        Directionality = 0x11,
        /// <summary>
        /// <para>Draw objects with the emission component only.</para>
        /// </summary>
        Emissive = 15,
        /// <summary>
        /// <para>Draw objects with real-time GI only.</para>
        /// </summary>
        Irradiance = 0x10,
        /// <summary>
        /// <para>Draw lit clusters.</para>
        /// </summary>
        LitClustering = 20,
        /// <summary>
        /// <para>Display texture resolution, with red tint indicating too high resolution, and blue tint indicating texture sizes that could be higher.</para>
        /// </summary>
        Mipmaps = 7,
        /// <summary>
        /// <para>Draw the camera like it would be drawn in-game. This uses the clear flags of the camera.</para>
        /// </summary>
        Normal = -1,
        /// <summary>
        /// <para>Display scene overdraw, with brighter colors indicating more overdraw.</para>
        /// </summary>
        Overdraw = 6,
        /// <summary>
        /// <para>Draw color-coded render paths.</para>
        /// </summary>
        RenderPaths = 4,
        /// <summary>
        /// <para>Draw directional light shadowmap cascades.</para>
        /// </summary>
        ShadowCascades = 3,
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
        /// <para>Draw the camera in wireframe and no background clearing.</para>
        /// </summary>
        Wireframe = 1
    }
}

