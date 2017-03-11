namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>How the material interacts with lightmaps and lightprobes.</para>
    /// </summary>
    [Flags]
    public enum MaterialGlobalIlluminationFlags
    {
        None,
        RealtimeEmissive,
        BakedEmissive,
        AnyEmissive,
        EmissiveIsBlack
    }
}

