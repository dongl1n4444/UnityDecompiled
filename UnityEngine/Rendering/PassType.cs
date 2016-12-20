namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>Shader pass type for Unity's lighting pipeline.</para>
    /// </summary>
    public enum PassType
    {
        /// <summary>
        /// <para>Deferred Shading shader pass.</para>
        /// </summary>
        Deferred = 10,
        /// <summary>
        /// <para>Forward rendering additive pixel light pass.</para>
        /// </summary>
        ForwardAdd = 5,
        /// <summary>
        /// <para>Forward rendering base pass.</para>
        /// </summary>
        ForwardBase = 4,
        /// <summary>
        /// <para>Legacy deferred lighting (light pre-pass) base pass.</para>
        /// </summary>
        LightPrePassBase = 6,
        /// <summary>
        /// <para>Legacy deferred lighting (light pre-pass) final pass.</para>
        /// </summary>
        LightPrePassFinal = 7,
        /// <summary>
        /// <para>Shader pass used to generate the albedo and emissive values used as input to lightmapping.</para>
        /// </summary>
        Meta = 11,
        /// <summary>
        /// <para>Motion vector render pass.</para>
        /// </summary>
        MotionVectors = 12,
        /// <summary>
        /// <para>Regular shader pass that does not interact with lighting.</para>
        /// </summary>
        Normal = 0,
        /// <summary>
        /// <para>Shadow caster &amp; depth texure shader pass.</para>
        /// </summary>
        ShadowCaster = 8,
        /// <summary>
        /// <para>Legacy vertex-lit shader pass.</para>
        /// </summary>
        Vertex = 1,
        /// <summary>
        /// <para>Legacy vertex-lit shader pass, with mobile lightmaps.</para>
        /// </summary>
        VertexLM = 2,
        /// <summary>
        /// <para>Legacy vertex-lit shader pass, with desktop (RGBM) lightmaps.</para>
        /// </summary>
        VertexLMRGBM = 3
    }
}

