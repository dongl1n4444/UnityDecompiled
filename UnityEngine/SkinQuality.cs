namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>The maximum number of bones affecting a single vertex.</para>
    /// </summary>
    public enum SkinQuality
    {
        /// <summary>
        /// <para>Chooses the number of bones from the number current QualitySettings. (Default)</para>
        /// </summary>
        Auto = 0,
        /// <summary>
        /// <para>Use only 1 bone to deform a single vertex. (The most important bone will be used).</para>
        /// </summary>
        Bone1 = 1,
        /// <summary>
        /// <para>Use 2 bones to deform a single vertex. (The most important bones will be used).</para>
        /// </summary>
        Bone2 = 2,
        /// <summary>
        /// <para>Use 4 bones to deform a single vertex.</para>
        /// </summary>
        Bone4 = 4
    }
}

