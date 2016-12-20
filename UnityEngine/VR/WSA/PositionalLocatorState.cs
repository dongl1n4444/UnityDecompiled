namespace UnityEngine.VR.WSA
{
    using System;

    /// <summary>
    /// <para>Indicates the lifecycle state of the device's spatial location system.</para>
    /// </summary>
    public enum PositionalLocatorState
    {
        Unavailable,
        OrientationOnly,
        Activating,
        Active,
        Inhibited
    }
}

