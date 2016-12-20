namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Enum controlling whether a light affects baked lightmaps, dynamic objects or both.</para>
    /// </summary>
    public enum LightmappingMode
    {
        /// <summary>
        /// <para>The light only affects lightmap baking, not dynamic objects.</para>
        /// </summary>
        Baked = 2,
        /// <summary>
        /// <para>The light affects both lightmap baking and dynamic objects.</para>
        /// </summary>
        Mixed = 1,
        /// <summary>
        /// <para>The light affects only dynamic objects, not direct or indirect baking.</para>
        /// </summary>
        Realtime = 4
    }
}

