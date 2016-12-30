namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Enum describing what part of a light contribution can be baked.</para>
    /// </summary>
    public enum LightmapBakeType
    {
        /// <summary>
        /// <para>Dynamic lights cast dynamic light and shadows. They can also change position, orientation, color, brightness, and many other properties at run time. No lighting gets baked into lightmaps.</para>
        /// </summary>
        Dynamic = 4,
        /// <summary>
        /// <para>Static lights cannot move or change in any way during run time.  All lighting for static objects gets baked into lightmaps. Lighting and shadows for dynamic objects gets baked into Light Probes.</para>
        /// </summary>
        Static = 2,
        /// <summary>
        /// <para>Stationary lights allow a mix of dynamic and baked lighting, based on the Stationary Lighting Mode used. These lights cannot move, but can change color and intensity at run time. Changes to color and intensity only affect direct lighting as indirect lighting gets baked into lightmaps.  If using Subtractive mode, changes to color or intensity are not calculated at run time.</para>
        /// </summary>
        Stationary = 1
    }
}

