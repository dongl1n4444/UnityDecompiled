namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>The LOD fade modes. Modes other than LODFadeMode.None will result in Unity calculating a blend factor for blending/interpolating between two neighbouring LODs and pass it to your shader.</para>
    /// </summary>
    public enum LODFadeMode
    {
        None,
        CrossFade,
        SpeedTree
    }
}

