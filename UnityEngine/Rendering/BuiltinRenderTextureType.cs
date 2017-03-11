﻿namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>Built-in temporary render textures produced during camera's rendering.</para>
    /// </summary>
    public enum BuiltinRenderTextureType
    {
        BindableTexture = -1,
        /// <summary>
        /// <para>Target texture of currently rendering camera.</para>
        /// </summary>
        CameraTarget = 2,
        /// <summary>
        /// <para>Currently active render target.</para>
        /// </summary>
        CurrentActive = 1,
        /// <summary>
        /// <para>Camera's depth texture.</para>
        /// </summary>
        Depth = 3,
        /// <summary>
        /// <para>Camera's depth+normals texture.</para>
        /// </summary>
        DepthNormals = 4,
        /// <summary>
        /// <para>Deferred shading G-buffer #0 (typically diffuse color).</para>
        /// </summary>
        GBuffer0 = 10,
        /// <summary>
        /// <para>Deferred shading G-buffer #1 (typically specular + roughness).</para>
        /// </summary>
        GBuffer1 = 11,
        /// <summary>
        /// <para>Deferred shading G-buffer #2 (typically normals).</para>
        /// </summary>
        GBuffer2 = 12,
        /// <summary>
        /// <para>Deferred shading G-buffer #3 (typically emission/lighting).</para>
        /// </summary>
        GBuffer3 = 13,
        /// <summary>
        /// <para>Deferred shading G-buffer #4 (typically occlusion mask for static lights if any).</para>
        /// </summary>
        GBuffer4 = 0x10,
        /// <summary>
        /// <para>G-buffer #5 Available.</para>
        /// </summary>
        GBuffer5 = 0x11,
        /// <summary>
        /// <para>G-buffer #6 Available.</para>
        /// </summary>
        GBuffer6 = 0x12,
        /// <summary>
        /// <para>G-buffer #7 Available.</para>
        /// </summary>
        GBuffer7 = 0x13,
        /// <summary>
        /// <para>Motion Vectors generated when the camera has motion vectors enabled.</para>
        /// </summary>
        MotionVectors = 15,
        None = 0,
        /// <summary>
        /// <para>Deferred lighting light buffer.</para>
        /// </summary>
        PrepassLight = 8,
        /// <summary>
        /// <para>Deferred lighting HDR specular light buffer (Xbox 360 only).</para>
        /// </summary>
        PrepassLightSpec = 9,
        /// <summary>
        /// <para>Deferred lighting (normals+specular) G-buffer.</para>
        /// </summary>
        PrepassNormalsSpec = 7,
        /// <summary>
        /// <para>Reflections gathered from default reflection and reflections probes.</para>
        /// </summary>
        Reflections = 14,
        /// <summary>
        /// <para>Resolved depth buffer from deferred.</para>
        /// </summary>
        ResolvedDepth = 5
    }
}

