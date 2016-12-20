namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>Shadow resolution options for a Light.</para>
    /// </summary>
    public enum LightShadowResolution
    {
        /// <summary>
        /// <para>Use resolution from QualitySettings (default).</para>
        /// </summary>
        FromQualitySettings = -1,
        /// <summary>
        /// <para>High shadow map resolution.</para>
        /// </summary>
        High = 2,
        /// <summary>
        /// <para>Low shadow map resolution.</para>
        /// </summary>
        Low = 0,
        /// <summary>
        /// <para>Medium shadow map resolution.</para>
        /// </summary>
        Medium = 1,
        /// <summary>
        /// <para>Very high shadow map resolution.</para>
        /// </summary>
        VeryHigh = 3
    }
}

