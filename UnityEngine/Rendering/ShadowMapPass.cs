namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>Allows precise control over which shadow map passes to execute Rendering.CommandBuffer objects attached using Light.AddCommandBuffer.</para>
    /// </summary>
    [Flags]
    public enum ShadowMapPass
    {
        /// <summary>
        /// <para>All shadow map passes.</para>
        /// </summary>
        All = 0x7ff,
        /// <summary>
        /// <para>All directional shadow map passes.</para>
        /// </summary>
        Directional = 960,
        /// <summary>
        /// <para>First directional shadow map cascade.</para>
        /// </summary>
        DirectionalCascade0 = 0x40,
        /// <summary>
        /// <para>Second directional shadow map cascade.</para>
        /// </summary>
        DirectionalCascade1 = 0x80,
        /// <summary>
        /// <para>Third directional shadow map cascade.</para>
        /// </summary>
        DirectionalCascade2 = 0x100,
        /// <summary>
        /// <para>Fourth directional shadow map cascade.</para>
        /// </summary>
        DirectionalCascade3 = 0x200,
        /// <summary>
        /// <para>All point light shadow passes.</para>
        /// </summary>
        Pointlight = 0x3f,
        /// <summary>
        /// <para>-X point light shadow cubemap face.</para>
        /// </summary>
        PointlightNegativeX = 2,
        /// <summary>
        /// <para>-Y point light shadow cubemap face.</para>
        /// </summary>
        PointlightNegativeY = 8,
        /// <summary>
        /// <para>-Z point light shadow cubemap face.</para>
        /// </summary>
        PointlightNegativeZ = 0x20,
        /// <summary>
        /// <para>+X point light shadow cubemap face.</para>
        /// </summary>
        PointlightPositiveX = 1,
        /// <summary>
        /// <para>+Y point light shadow cubemap face.</para>
        /// </summary>
        PointlightPositiveY = 4,
        /// <summary>
        /// <para>+Z point light shadow cubemap face.</para>
        /// </summary>
        PointlightPositiveZ = 0x10,
        /// <summary>
        /// <para>Spotlight shadow pass.</para>
        /// </summary>
        Spotlight = 0x400
    }
}

