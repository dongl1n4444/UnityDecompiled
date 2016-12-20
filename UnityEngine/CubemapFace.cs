namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Cubemap face.</para>
    /// </summary>
    public enum CubemapFace
    {
        /// <summary>
        /// <para>Left facing side (-x).</para>
        /// </summary>
        NegativeX = 1,
        /// <summary>
        /// <para>Downward facing side (-y).</para>
        /// </summary>
        NegativeY = 3,
        /// <summary>
        /// <para>Backward facing side (-z).</para>
        /// </summary>
        NegativeZ = 5,
        /// <summary>
        /// <para>Right facing side (+x).</para>
        /// </summary>
        PositiveX = 0,
        /// <summary>
        /// <para>Upwards facing side (+y).</para>
        /// </summary>
        PositiveY = 2,
        /// <summary>
        /// <para>Forward facing side (+z).</para>
        /// </summary>
        PositiveZ = 4,
        /// <summary>
        /// <para>Cubemap face is unknown or unspecified.</para>
        /// </summary>
        Unknown = -1
    }
}

