namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>ProceduralMaterial loading behavior.</para>
    /// </summary>
    public enum ProceduralLoadingBehavior
    {
        DoNothing,
        Generate,
        BakeAndKeep,
        BakeAndDiscard,
        Cache,
        DoNothingAndCache
    }
}

