namespace UnityEngine.Rendering
{
    using System;

    /// <summary>
    /// <para>An enum describing the way a realtime reflection probe refreshes in the Player.</para>
    /// </summary>
    public enum ReflectionProbeRefreshMode
    {
        OnAwake,
        EveryFrame,
        ViaScripting
    }
}

