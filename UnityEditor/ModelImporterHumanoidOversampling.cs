namespace UnityEditor
{
    using System;

    /// <summary>
    /// <para>Humanoid Oversampling available multipliers.</para>
    /// </summary>
    public enum ModelImporterHumanoidOversampling
    {
        /// <summary>
        /// <para>Default Humanoid Oversampling multiplier = 1 which is equivalent to no oversampling.</para>
        /// </summary>
        X1 = 1,
        /// <summary>
        /// <para>Humanoid Oversampling samples at 2 times the sampling rate found in the imported file.</para>
        /// </summary>
        X2 = 2,
        /// <summary>
        /// <para>Humanoid Oversampling samples at 4 times the sampling rate found in the imported file.</para>
        /// </summary>
        X4 = 4,
        /// <summary>
        /// <para>Humanoid Oversampling samples at 8 times the sampling rate found in the imported file.</para>
        /// </summary>
        X8 = 8
    }
}

