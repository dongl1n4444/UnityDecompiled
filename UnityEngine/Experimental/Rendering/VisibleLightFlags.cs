namespace UnityEngine.Experimental.Rendering
{
    using System;

    /// <summary>
    /// <para>Flags for VisibleLight.</para>
    /// </summary>
    [Flags]
    public enum VisibleLightFlags
    {
        None,
        IntersectsNearPlane,
        IntersectsFarPlane
    }
}

