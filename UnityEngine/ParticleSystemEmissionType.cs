namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>The mode in which particles are emitted.</para>
    /// </summary>
    [Obsolete("ParticleSystemEmissionType no longer does anything. Time and Distance based emission are now both always active.")]
    public enum ParticleSystemEmissionType
    {
        Time,
        Distance
    }
}

